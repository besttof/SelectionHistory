using System.Collections.Generic;
using UnityEditor;

namespace Besttof.SelectionHistory
{
	[CustomEditor(typeof(SelectionHistoryManager))]
	public class SelectionHistoryManagerEditor : Editor
	{
		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			return new SettingsProvider("Preferences/besttof/", SettingsScope.User)
			       {
				       label = "Selection History",
				       keywords = new HashSet<string>(new[] { "selection", "history" }),
				       guiHandler = _ =>
				       {
					       EditorGUIUtility.labelWidth = 200f;
					       var serializedObject = new SerializedObject(SelectionHistoryManager.instance);
					       
					       var capacity = serializedObject.FindProperty("_capacity");
					       var clearHistoryOnSceneChange = serializedObject.FindProperty("_clearHistoryOnSceneChange");
					       var ignoreEmptySelections = serializedObject.FindProperty("_ignoreEmptySelections");

					       using var changeCheck = new EditorGUI.ChangeCheckScope();
					       
					       EditorGUILayout.PropertyField(capacity);
					       EditorGUILayout.PropertyField(clearHistoryOnSceneChange);
					       EditorGUILayout.PropertyField(ignoreEmptySelections);

					       if (changeCheck.changed && serializedObject.ApplyModifiedProperties())
					       {
						       SelectionHistoryManager.instance.Save();
					       }
				       },
			       };
		}
	}
}