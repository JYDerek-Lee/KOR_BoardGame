Board Game - Starter Kit - v1.1

Author: Eikester
EMail: eikester@hotmail.de

Board Game Starter Kit allows you to make your own Board Games.

Features:
-8 Predefined Actions
--Go Forwards n Fields
--Go Backwards n Fields
--Go to specific Field
--Go to Jail
--Go back to Start
--Miss the next Turn
--Send a chosen Player back to Start
--Roll the Dice
-Simple to setup a new Boardgame
--Create as many Fields as you want
--adjust their FieldProperties
--Set the Player/CPU count
--Create as many Dices as you want
--Play

Changelog:
1.1:
	-Made class Dice abstract to allow custom Dices
	-Added SpriteDice, an example Dice inherits Dice
	-class Player is now abstract
	-Added SpritePlayer, an example Player inherits Player
	-class Field is now abstract
	-added CustomField and DefaultField, example classes inherits Field
	-You can now do Custom Actions
1.0: 
	-initial Release

Step by Step Setup:
1. Go to Menu->GameObject->Board Game Starter Kit->Create Sprite Dice
	-SpriteDice is a custom Dice inherited from the base class Dice
	-You could create your own Dice this way, i.e. 3D Dice
2. Go to Menu->GameObject->Board Game Starter Kit->Create Start Field
3. Go to Menu->GameObject->Board Game Starter Kit->Create (Non) Action Field
  -repeat this until you have enough Fields for your Board
  -Setup their Properties, make sure the IDs are in order
  -You can, of course, replace the SpriteRenderer with whatever you want
4. Go to Menu->GameObject->Board Game Starter Kit->Create Finish Field
5. Create an Empty GameObject and add the Script "GameController.cs"
6. Click Play

Credits:
kenney.nl for Fonts, Sounds and Sprites