using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DefaultField))]
public class DefaultFieldEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		DefaultField field = (DefaultField)target;

		GUI.changed = false;

		field.ID = EditorGUILayout.IntField("Id", field.ID);
		field.Type = (FieldType)EditorGUILayout.EnumPopup("Type", field.Type);

		if(field.Type == FieldType.Action)
		{
			field.Action = (ActionType)EditorGUILayout.EnumPopup("Action", field.Action);

			if(field.Action == ActionType.GoAhead)
				field.GoAheadNumSteps = EditorGUILayout.IntField("Go Ahead", field.GoAheadNumSteps);
			else if(field.Action == ActionType.GoBack)
				field.GoBackNumSteps = EditorGUILayout.IntField("Go Ahead", field.GoBackNumSteps);
			else if(field.Action == ActionType.GoToField)
				field.GotoField = EditorGUILayout.IntField("Goto Field ID", field.GotoField);
			else if(field.Action == ActionType.GoToJail)
				field.GotoField = EditorGUILayout.IntField("Goto Jail ID", field.GotoField);
		}

		if (GUI.changed)
			EditorUtility.SetDirty(field);
	}
}

