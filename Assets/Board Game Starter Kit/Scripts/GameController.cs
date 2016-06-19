using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardGameKit;

public class GameController : MonoBehaviour
{
	[Range(1, 2)]
	public int playerCount; 
	[Range(0, 1)]
	public int cpuCount;

	// the 4 Player Positions on the Fields
	Vector3[] playerSlots = new Vector3[]{
		new Vector3(-0.4f,  0.4f), // upper left
		new Vector3( 0.4f,  0.4f), // upper right
		new Vector3(-0.4f, -0.4f), // lower left
		new Vector3( 0.4f, -0.4f)  // lower right
	};

	int currentPlayer = 0;
	int selectedPlayer = -1;  							// for the Player selection Screen if one can choose a Player to send back to start
	int totalDicedNumber = 0; 							// the total number of all Dices
	int dicesFinished = 0; 								// this have to match dice.Count before the Player can Move
	List<Dice> dice = new List<Dice>();					// a List holding all the Dices
	List<Player> player = new List<Player>();			// a List holding all the Players
	List<Field> field = new List<Field>();				// a List holding all Fields
	List<Player> winner = new List<Player>();
	List<Player> selectablePlayer = new List<Player>();	// a List holding all possible Players to choose from, matches the players List excluding the current Player

	public bool stopWhenFirstPlayerHasFinished = true;	// should the Game stop when the first Player reaches the finish?
	public bool diceAgainWhenDicedSix = true;			// can the player dice again when he diced a six?

	bool waitForYourTurn = false;						// dont allow input until its your turn
	bool isGamerMoving = false;							// dont allow input as long as a player is moving
	bool isSpawningPlayers = false;						// dont allow input as long as the players are spawning
	bool isGameOver = false;							// is the Game Over yet?

	public GUISkin skin;
	string statusText = "";

	bool isTouching = false;							// for Input

	bool showPlayerSelectionWindow = false;				
	Rect playerSelectionWindowRect = new Rect(64, 64, 128, 128);

	public AudioClip moveSound;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine(reset ());
	}

	// Update is called once per frame
	void Update ()
	{
		if(isGameOver)
		{
			UpdateStatusText ("Game Over!\n" + winner[0].Name + " is the Winner!");
			return;
		}

		// dont allow any input as long as a player is moving
		if(isGamerMoving)
			return;

		// dont allow any input until its your turn
		if(waitForYourTurn)
			return;
		
		UpdateStatusText("Your turn '" + player[currentPlayer].Name + "'" + (player[currentPlayer].IsHuman ? "\nClick Left Mousebutton to roll the Dice" : ""));

		// if the current player is a human controlled player
		if(player.Count > 0 && player[currentPlayer].IsHuman)
		{
			// wait for input to start Coroutine
			if(Input.GetMouseButtonDown(0) || Input.touchCount > 0 && !isTouching)
			{
				isTouching = true;
				StartCoroutine(playersTurn());
			}
			else
			{
				isTouching = false;
			}
		}
		else
		{
			// if its a cpu controlled player just call the Coroutine
			StartCoroutine(playersTurn());
		}
	}

	void UpdateStatusText(string text)
	{
		statusText = text;
	}

	void OnGUI()
	{
		if(skin != null)
			GUI.skin = skin;
		if(showPlayerSelectionWindow)
		{
			playerSelectionWindowRect = GUI.Window(0, playerSelectionWindowRect, PlayerSelectionWindow, "Choose a Player");
		}

		GUI.Label (new Rect(0, 0, Screen.width, 80), statusText);
	}

	// shows a window where you select a player to be send back to start
	// could be extended to show a nobody button
	void PlayerSelectionWindow(int windowID)
	{
		for(int i = 0; i < selectablePlayer.Count; i++)
		{
			if(GUI.Button (new Rect(16, 32 + (i * 32), 160, 28), selectablePlayer[i].Name))
			{
				selectedPlayer = selectablePlayer[i].ID;
				showPlayerSelectionWindow = false;
				break;
			}
		}

		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}

	// Resets the Board
	IEnumerator reset()
	{
		UpdateStatusText("");
		winner.Clear();

		waitForYourTurn = true;

		// search all field Objects and sort by id
		field.Clear();
		dice.Clear();

		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Dice"))
		{
			dice.Add (g.GetComponent<Dice>());

			yield return null;
		}

		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Field"))
		{
			field.Add (g.GetComponent<Field>());
			
			yield return null;
		}
		field.Sort((a, b) => a.ID.CompareTo(b.ID));

		if(cpuCount >= playerCount)
			cpuCount = playerCount - 1; // minimum 1 Human Player

		// choose who starts
		currentPlayer = Random.Range (0, playerCount);

		// spawn the players
		SpawnPlayer();

		isGamerMoving = false;
		totalDicedNumber = 0;
		dicesFinished = 0;

		playerSelectionWindowRect = new Rect(64, 64, 192, (playerCount - 1) * 32 + 32 + 16);

		// wait while the players are spawning
		while(isSpawningPlayers)
			yield return null;

		yield return new WaitForSeconds(1f);
		waitForYourTurn = false;

		yield return 0;
	}

	/// <summary>
	/// Players turn.
	/// First roll the dice, after that move the player to the target Field
	/// This Method is called e.g. after the current player pushes a Button or Presses a Key
	/// </summary>
	IEnumerator playersTurn()
	{
		waitForYourTurn = true;

		// first roll the Dices
		dicesFinished = 0;
		totalDicedNumber = 0;
		for(int i = 0; i < dice.Count; i++)
		{
			StartCoroutine(rollTheDice(i));
		}

		// wait until all dices stopped rolling
		while(dicesFinished < dice.Count)
			yield return new WaitForSeconds(0.25f);

		// Move to the target field
		Move(player[currentPlayer], totalDicedNumber);

		yield return new WaitForSeconds(0.25f);
	}

	/// <summary>
	/// Rolls the dice.
	/// This Method is called inside the playersTurn Method for each Dice
	/// </summary>
	/// <param name="diceID">Dice ID</param>
	IEnumerator rollTheDice(int diceID)
	{
		dice[diceID].RollTheDice();

		while(dice[diceID].IsDiceRolling())
			yield return new WaitForSeconds(0.01f);

		totalDicedNumber += dice[diceID].DicedNumber();

		dicesFinished++;

		yield return 0;
	}

	// Remove old Players and Start Coroutine 'spawnPlayer'
	// normally called at the beginning of the Game or when resetting the current Scene
	void SpawnPlayer()
	{
		player.Clear();
		/*
		while(GameObject.FindGameObjectWithTag("Gamer") != null)
		{
			DestroyImmediate(GameObject.FindGameObjectWithTag("Gamer"));
		}
		*/

		StartCoroutine(spawnPlayer());
	}

	// Spawns the players and moves them to the Start field.
	IEnumerator spawnPlayer()
	{
		isSpawningPlayers = true;
		Debug.Log("player Count" + playerCount);
		// spawn the players
		for(int i = 0; i < playerCount; i++)
		{
			float t = 0f;

			bool isHuman = true;

			if(playerCount - cpuCount <= i)
				isHuman = false;

			GameObject gamer = Instantiate(Resources.Load("Prefabs/Gamer")) as GameObject;
			gamer.name = "GamerFigure_" + (i + 1); // Gameobjects name doesn't really matter
			gamer.tag = "Gamer";
			gamer.GetComponent<Player>().ID = i;
			gamer.GetComponent<Player>().Name = (isHuman ? "Player" : "CPU") 
				+ " " + (isHuman ? (i + 1) : ((i + 1) - (playerCount - cpuCount)));
			gamer.GetComponent<Player>().IsHuman = isHuman;
			gamer.GetComponent<Player>().HasFinished = false;
			
			Vector3 startPosition = gamer.transform.position;
			Vector3 endPosition = field[0].transform.position + playerSlots[i];

			// lerp the player to the starting field
			while(t < 1f)
			{
				t += Time.deltaTime * 4f;

				gamer.transform.position = Vector3.Lerp(startPosition, endPosition, t);

				yield return null;
			}

			// add to players list
			player.Add(gamer.GetComponent<SpritePlayer>());

			yield return null;
		}

		isSpawningPlayers = false;

		yield return 0;
	}

	// Move the current player by the diced Number.
	public void Move(Player gamer, int dicedNumber){
		StartCoroutine(moveForwards(gamer, dicedNumber));
		StartCoroutine(performAction());
	}

	// Move the current Player to the target Position field by field
	IEnumerator moveForwardsWant (Player gamer, int dicedNumber){
		isGamerMoving = true;

		// get the field the player is currently on
		int currentField = gamer.CurrentFieldID;

		if (currentField + dicedNumber < field.Count) {
			// if diced a 4 loop 4 times
			for (int i = 0; i < dicedNumber; i++) {
				if (moveSound != null)
					GetComponent<AudioSource> ().PlayOneShot (moveSound);
				/*
				float t = 0f;
				
				// increase the start- and endposition each iteration
				// startposition represents the field the player is on
				// endposition is always one field ahead of the players field
				// to make it look like the player moves one field at the time
				Vector3 startPosition = field[(currentField + i)].transform.position + playerSlots[gamer.ID];
				Vector3 endPosition = field[(currentField + i + 1)].transform.position + playerSlots[gamer.ID];

				gamer.transform.position = Vector3.Lerp (startPosition, endPosition, 0);

				
				// lerp to the next field
				while (t < 1f) {
					t += Time.deltaTime * 4f;

					gamer.transform.position = Vector3.Lerp (startPosition, endPosition, t);

					yield return null;
				}
				 
				yield return null;
				*/
			}
			// update the players current field

			gamer.CurrentFieldID = currentField + dicedNumber;
			Debug.Log ("ForwardsWant is " + gamer.CurrentFieldID);

			if (field[gamer.CurrentFieldID].Type == FieldType.Finish) {
				gamer.HasFinished = true;
				winner.Add (gamer);

				if (stopWhenFirstPlayerHasFinished) {
					isGameOver = true;
				}
				else {
					isGameOver = IsGameOver ();
				}
				yield return new WaitForSeconds (0.25f);
			}
		}

		// wait a little
		yield return new WaitForSeconds (0.1f);
		isGamerMoving = false;

		yield return 0;
	}

	// Move the current Player to the target Position field by field
	IEnumerator moveForwards(Player gamer, int dicedNumber){
		isGamerMoving = true;

		// get the field the player is currently on
		int currentField = gamer.CurrentFieldID;

		//if(currentField + dicedNumber < field.Count){
			float t = 2f;
			
			// if diced a 4 loop 4 times
			/*
			for (int i = 0; i < dicedNumber; i++) {
				if (moveSound != null)
					GetComponent<AudioSource> ().PlayOneShot (moveSound);
				float t = 0f;

				// increase the start- and endposition each iteration
				// startposition represents the field the player is on
				// endposition is always one field ahead of the players field
				// to make it look like the player moves one field at the time
				Vector3 startPosition = field[(currentField + i)].transform.position + playerSlots[gamer.ID];
				Vector3 endPosition = field[(currentField + i + 1)].transform.position + playerSlots[gamer.ID];

				// lerp to the next field
				//while (t < 1f) {
					t += Time.deltaTime * 4f;
					//Debug.Log ("GOGO");

					gamer.transform.position = Vector3.Lerp (startPosition, endPosition, t);

				//	yield return null;
				//}
				//yield return null;
				}
				*/

				// update the players current field
				Vector3 startPosition = field[gamer.CurrentFieldID].transform.position;

				gamer.CurrentFieldID = currentField + dicedNumber;
				Debug.Log ("Frist your pos is " + gamer.CurrentFieldID);

				//3사에서 지름길 벗어남
				if ((gamer.CurrentFieldID >= 44) && (gamer.CurrentFieldID <= 49)) {
					gamer.CurrentFieldID = gamer.CurrentFieldID - 43 + 24;
					Debug.Log ("1 Case" + gamer.CurrentFieldID);
					//Debug.Log ("move new" + gamer.CurrentFieldID); 
				}
				//2사에서 중앙에 도착
				else if (gamer.CurrentFieldID == 53) {
					gamer.CurrentFieldID = 40;
					Debug.Log ("2 Case" + gamer.CurrentFieldID);
				}
				//2사에서 중앙 도착 실패
				else if ((gamer.CurrentFieldID >= 53) && (gamer.CurrentFieldID <= 58)) {
					gamer.CurrentFieldID = gamer.CurrentFieldID - 52 + 42;
					Debug.Log ("3 Case" + gamer.CurrentFieldID);
				}
				//4사에서 게임 종료 / 그냥 종료
				else if ((gamer.CurrentFieldID >= 63) || ((gamer.CurrentFieldID >= 31) && (gamer.CurrentFieldID <=36)) ) {
					Debug.Log ("4 Case" + gamer.CurrentFieldID);
					gamer.HasFinished = true;
					winner.Add (gamer);

					if (stopWhenFirstPlayerHasFinished) {
						isGameOver = true;
					}
					else {
						isGameOver = IsGameOver ();
					}
					yield return new WaitForSeconds (0.25f);
				}

				if (field[gamer.CurrentFieldID].Type == FieldType.Finish) {
					gamer.HasFinished = true;
					winner.Add (gamer);

					if (stopWhenFirstPlayerHasFinished) {
						isGameOver = true;
					}
					else {
						isGameOver = IsGameOver ();
					}
					yield return new WaitForSeconds (0.25f);
				}
				Vector3 endPosition = field[gamer.CurrentFieldID].transform.position;
				t += Time.deltaTime * 4f;
				gamer.transform.position = Vector3.Lerp (startPosition, endPosition, t);
				Debug.Log ("Now your pos is " + gamer.CurrentFieldID + gamer.transform.position);
				Debug.Log ("start end" + startPosition + endPosition);



			//} //for
		//}

		// wait a little
		yield return new WaitForSeconds(0.1f);
		isGamerMoving = false;

		yield return 0;
	}

	// Move the current Player backwards n fields
	IEnumerator moveBackwards(Player gamer, int backSteps)
	{
		isGamerMoving = true;

		// get the field the player is currently on
		int currentField = gamer.CurrentFieldID;

		for(int i = 0; i > backSteps; i--)
		{
			if(moveSound != null)
				GetComponent<AudioSource>().PlayOneShot(moveSound);

			float t = 0f;
			
			Vector3 startPosition = field[(currentField + i)].transform.position + playerSlots[gamer.ID];
			Vector3 endPosition = field[(currentField + i - 1)].transform.position + playerSlots[gamer.ID];
			
			while(t < 1f)
			{
				t += Time.deltaTime * 4f;
				
				gamer.transform.position = Vector3.Lerp (startPosition, endPosition, t);
				
				yield return null;
			}
			yield return null;
		}
		// update the players current field
		gamer.CurrentFieldID = currentField + backSteps;

		isGamerMoving = false;

		yield return 0;
	}

	// Perform an Action like Dice again, go ahead n fields, go to jail etc.
	IEnumerator performAction(){
		while(isGamerMoving)
			yield return new WaitForSeconds(0.01f);

		// wait a little before performing the action
		yield return new WaitForSeconds(1f);

		bool diceAgain = false;
		int currentField = player[currentPlayer].CurrentFieldID;
		Field f = field[currentField];
		Player p = player[currentPlayer].GetComponent<Player>();

		if(totalDicedNumber == 6 * dice.Count){
			if(diceAgainWhenDicedSix){
				diceAgain = true;
			}
		}
		//else if(f.Type == FieldType.Action)
		
		if(f.Type == FieldType.Action){
			// perform the action of the field
			switch(f.Action){
				// go back to start
			case ActionType.BackToStart:
				StartCoroutine(moveBackwards(p, p.CurrentFieldID));
				break;
				
				// go forwards n fields
			case ActionType.GoAhead:
				StartCoroutine(moveForwards(p, f.GoAheadNumSteps));
				break;
				
				// go backwards n fields
			case ActionType.GoBack:
				StartCoroutine(moveBackwards(p, -f.GoBackNumSteps));
				break;
				
				// goto specific field/jail
			case ActionType.GoToField:
			case ActionType.GoToJail:
				
				int target = f.GotoField;
				
				// determine the direction
				if(p.CurrentFieldID > target){
					// go backwards
					int c = p.CurrentFieldID - target;
					StartCoroutine(moveBackwards(p, -c));
				}
				else{
					// go forwards
					int c = target - p.CurrentFieldID;
					StartCoroutine(moveForwards(p, c));
					//StartCoroutine (moveForwardsWant (p, c));
				}
				break;
				
				// miss next turn
			case ActionType.MissATurn:
				p.MissNextTurn = true;
				break;
				
			case ActionType.DiceAgain:
				p.CanDiceAgain = true;
				break;
				
				// the current player can choose a player he wants to send back to the start
			case ActionType.SendPlayerToStart:
				StartCoroutine(sendPlayerBackToStart());
				break;

				// perform custom action
			case ActionType.Custom:
				f.DoAction(p);
				break;
			}

			diceAgain = p.CanDiceAgain;

			while(isGamerMoving)
				yield return new WaitForSeconds(0.01f);
		}

		// if can dice again
		if(diceAgain){
			// player can dice again, simply decrease currentPlayer by one so if 
			// NextPlayer() is called currentPlayer will be increased to be the current Player
			currentPlayer--;
		}

		while(isGamerMoving)
			yield return new WaitForSeconds(0.01f);

		// wait a little before switching to next player
		yield return new WaitForSeconds(0.25f);
		NextPlayer();
	}

	// shows a selection Window and sends the selected Player back to the Start Field
	IEnumerator sendPlayerBackToStart()
	{
		isGamerMoving = true;

		selectedPlayer = -1;
		selectablePlayer.Clear();

		foreach(Player g in player)
		{
			// dont allow players who sit at the start or has finished 
			// also dont allow to send yourself back to start
			if(g.ID != player[currentPlayer].ID && g.CurrentFieldID != 0 && !g.HasFinished)
				selectablePlayer.Add (g);

			yield return null;
		}

		yield return new WaitForSeconds(0.1f);

		if(selectablePlayer.Count > 0)
		{
			// if its a Human Player show selection Screen otherwise just choose a random Player
			if(player[currentPlayer].IsHuman)
			{
				playerSelectionWindowRect = new Rect(64, 64, 192, (selectablePlayer.Count) * 32 + 32 + 16);
				showPlayerSelectionWindow = true;
			}
			else
			{
				selectedPlayer = selectablePlayer[Random.Range (0, selectablePlayer.Count)].ID;
			}

			UpdateStatusText("Choose a Player you want\nto send back to the Start!");

			// choose a Player
			// wait while he is choosing
			while(showPlayerSelectionWindow || selectedPlayer == -1)
				yield return new WaitForSeconds(0.01f);

			UpdateStatusText("");

			int currentField = player[selectedPlayer].CurrentFieldID;

			// now send the selected player back
			StartCoroutine(moveBackwards(player[selectedPlayer], -currentField));

			while(isGamerMoving)
				yield return new WaitForSeconds(0.1f);
		}
		else
		{
			yield return new WaitForSeconds(0.1f);
			isGamerMoving = false;
		}
	}

	bool IsGameOver()
	{
		foreach(Player g in player)
		{
			if(!g.HasFinished)
				return false;
		}

		return true;
	}

	// Simply swaps to the next Player
	void NextPlayer()
	{
		if(isGameOver)
			return;

		currentPlayer++;
		if(currentPlayer >= playerCount)
		{
			currentPlayer = 0;
		}

		CheckNextPlayerHasFinished();
		CheckNextPlayerMissNextTurn();

		waitForYourTurn = false;
	}

	void CheckNextPlayerHasFinished(){
		if(player[currentPlayer].HasFinished)	{
			currentPlayer++;
		}
		
		if(currentPlayer >= playerCount){
			currentPlayer = 0;
		}
	}

	// Checks if the next Player misses the next turn and if so swaps to the next player
	void CheckNextPlayerMissNextTurn()
	{
		if(player[currentPlayer].MissNextTurn)
		{
			player[currentPlayer].MissNextTurn = false;
			currentPlayer++;
		}
		
		if(currentPlayer >= playerCount)
		{
			currentPlayer = 0;
		}
	}
}

