using UnityEngine;
using System.Collections;
using BoardGameKit;

public class SpritePlayer : Player{
	int id = 0;
	int actionNum = 0;
	bool isHuman = true;
	bool missNextTurn = false;
	bool hasFinished = false;
	bool canDiceAgain = false;
	bool isAction = false;
	bool isFinal = false;
	string playerName = "Player";
	Field field;
	int fieldID;

	public Sprite[] color;

	void Update ()
	{
		this.GetComponent<SpriteRenderer>().sprite = color[id];
	}

	#region implemented abstract members of Player

	public override int ID 
	{
		get 
		{
			return id;
		}
		set 
		{
			id = value;
		}
	}

	public override int ActionNum {
		get {
			return actionNum;
		}
		set {
			actionNum = value;
		}
	}
	public override bool IsHuman 
	{
		get 
		{
			return isHuman;
		}
		set 
		{
			isHuman = value;
		}
	}

	public override bool MissNextTurn 
	{
		get 
		{
			return missNextTurn;
		}
		set 
		{
			missNextTurn = value;
		}
	}

	public override string Name 
	{
		get 
		{
			return playerName;
		}
		set 
		{
			playerName = value;
		}
	}

	public override bool HasFinished 
	{
		get 
		{
			return hasFinished;
		}
		set 
		{
			hasFinished = value;
		}
	}

	public override bool IsAction {
		get {
			return isAction;
		}
		set {
			isAction = value;
		}
	}

	public override bool IsFianl {
		get {
			return isFinal;
		}
		set {
			isFinal = value;
		}
	}
	public override bool CanDiceAgain 
	{
		get 
		{
			return canDiceAgain;
		}
		set 
		{
			canDiceAgain = value;
		}
	}

	public override Field CurrentField 
	{
		get 
		{
			return field;
		}
		set 
		{
			field = value;
		}
	}

	public override int CurrentFieldID 
	{
		get 
		{
			return fieldID;
		}
		set 
		{
			fieldID = value;
		}
	}

	#endregion
}

