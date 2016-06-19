using UnityEngine;
using System.Collections;

namespace BoardGameKit
{
	public abstract class Player : MonoBehaviour
	{
		public abstract int ID 				{ get; set; }
		public abstract bool IsHuman 		{ get; set; }
		public abstract bool MissNextTurn 	{ get; set; }
		public abstract bool IsAction { get; set; }
		public abstract bool IsFianl { get; set; }
		public abstract string Name 		{ get; set; }	
		public abstract bool HasFinished 	{ get; set; }
		public abstract bool CanDiceAgain 	{ get; set; }
		public abstract Field CurrentField  { get; set; }
		public abstract int CurrentFieldID  { get; set; }
	}
}

