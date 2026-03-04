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

		[SerializeField] private bool _clearHistoryOnSceneChange = false;
		[SerializeField] private bool _ignoreEmptySelections = false;

		private HistoryBuffer<Object[]> _history = new(25);
		private bool _ignoreCallbackOnce;

		[Shortcut("Selection History/Previous Selection", KeyCode.Minus, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
		static void HandlePreviousShortcut(ShortcutArguments args) => instance.SelectPrevious();

		[Shortcut("Selection History/Next Selection", KeyCode.Equals, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
		static void HandleNextShortcut(ShortcutArguments args) => instance.SelectNext();

		[InitializeOnLoadMethod]
		private static void InitializeInstance()
		{
			instance.Initialize();
		}

		private void Initialize()
		{
			_history = new HistoryBuffer<Object[]>(_capacity);
		}

		private void OnEnable()
		{
			// Start with a selection clear
			_history.Push(Array.Empty<Object>());

			// Push current selection
			if (Selection.count != 0)
			{
				_history.Push(Selection.objects);
			}

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
				_history.Clear();
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

		private void SelectObjects(Object[] objectsToSelect)
		{
			Debug.Log($"Selecting objects:");
			for (int i = 0; i < objectsToSelect.Length; i++)
			{
				var o = objectsToSelect[i];

				Debug.Log($"	> {o}");
			}

			_ignoreCallbackOnce = true;
			Selection.objects = objectsToSelect;
		}

		private static string GetSelectionName(Object[] objects)
		{
			return string.Join(", ", Array.ConvertAll(objects, o => o.name));
		}

		private static bool SequenceEquals<T>(T[] a, T[] b) where T : Object
		{
			if (a.Length != b.Length) return false;

			for (var i = 0; i < a.Length; i++)
			{
				if (a != b) return false;
			}

			return true;
		}
	}
}