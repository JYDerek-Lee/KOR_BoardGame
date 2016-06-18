using UnityEngine;
using System.Collections;

public enum ActionType
{
	None,
	DiceAgain,
	MissATurn,
	GoAhead,
	BackToStart,
	GoBack,
	SendPlayerToStart,
	GoToField,
	GoToJail,
	Custom
}

public enum FieldType
{
	Start,
	Finish,
	Normal,
	Action
}

