using UnityEngine;
using System.Collections;
using BoardGameKit;

/// <summary>
/// Example Dice using Sprites
/// </summary>
public class SpriteDice : Dice
{
	public Sprite[] diceSites = new Sprite[6]; 	// a normal Dice has 6 Sites so 6 Graphics
	public int minDiceRuns = 2;					// how long the dices roll
	public int maxDiceRuns = 6;					// the actual value is chosen randomly between the 2 variables

	int dicedNumber = 0;
	bool isDiceRolling = false;

	// Use this for initialization
	void Start () 
	{
		this.GetComponent<SpriteRenderer>().sprite = diceSites[Random.Range (0, diceSites.Length)];
	}

	IEnumerator rollTheDice()
	{
		isDiceRolling = true;
		
		float diceTime = 0.1f;
		int diceRuns = Random.Range(minDiceRuns, maxDiceRuns) * diceSites.Length;
		
		int lastNumber = 0;
		for(int i = 0; i < diceRuns; i++)
		{
			int c;
			
			// this is to prevent the same number twice in a row
			while((c = Random.Range(0, diceSites.Length)) == lastNumber)
				yield return null;
			
			lastNumber = c;
			
			// increase the time to make it look like the dices getting slower over time
			//diceTime += 0.25f/diceRuns; // 즉시 주사위 시작
			
			if(GetComponent<AudioSource>().clip != null)
				GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
			yield return new WaitForSeconds(diceTime);
			
			// update the graphic to show the correct number
			this.GetComponent<SpriteRenderer>().sprite = diceSites[lastNumber];
			
			yield return null;
		}
		
		// add plus 1 because the Dice Numbers start at 1 not 0
		dicedNumber = lastNumber + 1;
		
		yield return new WaitForSeconds(0.1f);
		
		isDiceRolling = false;
		
		yield return 0;
	}

	/// <summary>
	/// Rolls the dice.
	/// </summary>
	public override void RollTheDice ()
	{
		StartCoroutine(rollTheDice());
	}

	/// <summary>
	/// The diced Number.
	/// </summary>
	/// <returns>The number.</returns>
	public override int DicedNumber ()
	{
		return dicedNumber;
	}

	/// <summary>
	/// Determines whether this dice is rolling.
	/// </summary>
	/// <returns><c>true</c> if this dice is rolling; otherwise, <c>false</c>.</returns>
	public override bool IsDiceRolling ()
	{
		return isDiceRolling;
	}
}

