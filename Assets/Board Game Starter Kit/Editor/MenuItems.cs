using UnityEngine;
using UnityEditor;
using System.Collections;
using BoardGameKit;

public class MenuItems : MonoBehaviour 
{
	[MenuItem ("GameObject/Board Game Starter Kit/Create Dice")]
	static void CreateDice()
	{
		GameObject dice = Instantiate(Resources.Load("Prefabs/Dice")) as GameObject;
		dice.name = "Dice";
	}

	[MenuItem ("GameObject/Board Game Starter Kit/Create Start Field")]
	static void CreateStart()
	{
		GameObject start = Instantiate(Resources.Load("Prefabs/Start")) as GameObject;
		start.name = "Start";
		start.tag = "Field";
		start.GetComponent<Field>().ID = (GameObject.FindGameObjectsWithTag("Field").Length - 1);
	}

	[MenuItem ("GameObject/Board Game Starter Kit/Create Finish Field")]
	static void CreateFinish()
	{
		GameObject finish = Instantiate(Resources.Load("Prefabs/Finish")) as GameObject;
		finish.name = "Finish";
		finish.tag = "Field";
		finish.GetComponent<Field>().ID = (GameObject.FindGameObjectsWithTag("Field").Length - 1);
	}

	[MenuItem ("GameObject/Board Game Starter Kit/Create Non Action Field")]
	static void CreateField()
	{
		GameObject field = Instantiate(Resources.Load("Prefabs/Field")) as GameObject;

		int num = GameObject.FindGameObjectsWithTag("Field").Length - 1;
		field.name = "Field_" + num;
		field.tag = "Field";
		field.GetComponent<Field>().ID = (GameObject.FindGameObjectsWithTag("Field").Length - 1);
		field.GetComponent<Field>().Action = ActionType.None;
		field.GetComponent<Field>().Type = FieldType.Normal;

		field.GetComponentInChildren<TextMesh>().text = "" + num;
	}

	[MenuItem ("GameObject/Board Game Starter Kit/Create Action Field")]
	static void CreateActionField()
	{
		GameObject field = Instantiate(Resources.Load("Prefabs/Field")) as GameObject;
		
		int num = GameObject.FindGameObjectsWithTag("Field").Length - 1;
		field.name = "Field_";
		field.tag = "Field";
		field.GetComponent<Field>().ID = (GameObject.FindGameObjectsWithTag("Field").Length - 1);
		field.GetComponent<Field>().Action = ActionType.None;
		field.GetComponent<Field>().Type = FieldType.Action;

		field.GetComponentInChildren<TextMesh>().text = "" + num;
	}
}
