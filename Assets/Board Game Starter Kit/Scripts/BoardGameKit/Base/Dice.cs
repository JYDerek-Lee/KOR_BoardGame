using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BoardGameKit
{
	/// <summary>
	/// Base class for the Dice
	/// </summary>
	public abstract class Dice : MonoBehaviour
	{
		public abstract bool IsDiceRolling();  	// wether or not the dice is rolling
		public abstract int DicedNumber();		// returns the diced Number		
		public abstract void RollTheDice();		// roll the dice
	}
}

