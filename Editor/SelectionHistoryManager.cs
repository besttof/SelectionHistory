using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Besttof.SelectionHistory
{
	[FilePath("SelectionHistory.asset", FilePathAttribute.Location.PreferencesFolder)]
	public class SelectionHistoryManager : ScriptableSingleton<SelectionHistoryManager>
	{
		[Min(5)]
		[Delayed]
		[SerializeField] private int _capacity = 25;

		[SerializeField] private bool _clearHistoryOnSceneChange = true;
		[SerializeField] private bool _ignoreEmptySelections = false;
		[SerializeField] private SelectionMode _selectionMode = SelectionMode.FastAndNaive;

		[SerializeReference] private IHistoryBuffer<Object[]> _history;
		private bool _ignoreCallbackOnce;

		[Shortcut("Selection History/Previous Selection", KeyCode.Minus, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
		static void HandlePreviousShortcut(ShortcutArguments args) => instance.SelectPrevious();

		[Shortcut("Selection History/Next Selection", KeyCode.Equals, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
		static void HandleNextShortcut(ShortcutArguments args) => instance.SelectNext();

		[Shortcut("Selection History/Clear History", KeyCode.Backslash, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
		static void HandleClearShortcut(ShortcutArguments args) => instance.Clear();

		[InitializeOnLoadMethod]
		private static void EnsureInstanceIsCreated()
		{
			// Enforce the side effects of calling the instance.
			_ = instance;
		}

		private void Initialize()
		{
			if (_history != null) return;

			_history = HistoryBufferFactory.Create(_capacity, _selectionMode);
			Clear();
		}

		private void OnEnable()
		{
			Initialize();

			Selection.selectionChanged += OnSelectionChanged;
			EditorSceneManager.sceneOpening += OnSceneOpening;
		}

		private void OnDisable()
		{
			Selection.selectionChanged -= OnSelectionChanged;
			EditorSceneManager.sceneOpening -= OnSceneOpening;
		}

		internal void Save()
		{
			if (_capacity != _history.Capacity)
			{
				Initialize();
			}

			Save(true);
			Debug.Log("Saved to: " + GetFilePath());
		}

		/// <summary>
		/// Checks if two sequences are equal, also considers empty and all null sequences equal
		/// </summary>
		internal static bool SequenceEquals<T>(T[] a, T[] b) where T : Object
		{
			for (var i = 0; i < Math.Max(a.Length, b.Length); i++)
			{
				var ai = i < a.Length ? a[i] : null;
				var bi = i < b.Length ? b[i] : null;
				if (ai != bi) return false;
			}

			return true;
		}

		private void OnSelectionChanged()
		{
			var ignoreCallback = _ignoreCallbackOnce;
			_ignoreCallbackOnce = false;
			if (ignoreCallback) return;
			if (_ignoreEmptySelections && Selection.count == 0) return;

			_history.Push(Selection.objects);
		}

		private void OnSceneOpening(string path, OpenSceneMode mode)
		{
			if (mode == OpenSceneMode.Single && _clearHistoryOnSceneChange)
			{
				Clear();
			}
		}

		private void SelectPrevious()
		{
			var couldSelectPrevious = false;
			Object[] selectedObjects = null;

			// The selected objects can resolve to null when scenes are deactivated or unloaded
			// we skip the selection as long as it resolves to an identical result
			while (_history.TryGoBack(out selectedObjects))
			{
				if (SequenceEquals(selectedObjects, Selection.objects)) continue;

				couldSelectPrevious = true;
				break;
			}

			if (couldSelectPrevious)
			{
				SelectObjects(selectedObjects);
			}
			else
			{
				EditorApplication.Beep();
			}
		}


		private void SelectNext()
		{
			var couldSelectNext = false;
			Object[] selectedObjects = null;

			// The selected objects can resolve to null when scenes are deactivated or unloaded
			// we skip the selection as long as it resolves to an identical result
			while (_history.TryGoForward(out selectedObjects))
			{
				if (SequenceEquals(selectedObjects, Selection.objects)) continue;

				couldSelectNext = true;
				break;
			}

			if (couldSelectNext)
			{
				SelectObjects(selectedObjects);
			}
			else
			{
				EditorApplication.Beep();
			}
		}

		private void Clear()
		{
			_history.Clear();

			// Start with nothing selected
			_history.Push(Array.Empty<Object>());

			// Push the active selection if it exists
			if (Selection.count != 0)
			{
				_history.Push(Selection.objects);
			}
		}

		private void SelectObjects(Object[] objectsToSelect)
		{
			_ignoreCallbackOnce = true;
			Selection.objects = objectsToSelect;
		}

		private static string GetSelectionName(Object[] objects)
		{
			return string.Join(", ", Array.ConvertAll(objects, o => o.name));
		}
	}
}