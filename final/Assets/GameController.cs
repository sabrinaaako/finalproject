using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	public Text instructions;
	public Text scoreText;
	public Text lootText;
	public GameObject cubePrefab;
	public Button actionStart;
	int numCubesX = 8;
	int numCubesY = 5;
	int pusherOneLocation;
	int pusherTwoLocation;
	int startX;
	int startY;
	public GameObject[,] grid;
	Vector3[] pusherLocations;
	GameObject[] matchList;
	int[] numLoot;
	GameObject pusherOne;
	GameObject pusherTwo;
	Color[] colors = {Color.black, Color.blue, Color.green, Color.red, Color.yellow, Color.white};
	float score = 0f;
	float theTimer = 0.0f;
	float turnTime = 4.0f;
	int numTurns = 1;
	bool pusherOneMoved;
	bool pusherTwoMoved;
	bool cubesHaveFallen;
	bool lootCounted;
	bool pushersSpawned;
	public bool planningPhase;
	public bool actionPhase;
	public bool resolutionPhase;
	StoreStats sS;


	// Use this for initialization
	void Start () {

		sS = GameObject.Find("GameObject").GetComponent<StoreStats>();

		//setting size of loot info
		numLoot = new int[colors.Length];

		//setting to planning phase
		planningPhase = true;
		actionPhase = false;
		resolutionPhase = false;
		
		//instantiating cubes
		grid = new GameObject[numCubesX, numCubesY];

		for (int x = 0; x < numCubesX; x++) {
			for (int y = 0; y < numCubesY; y++) {
				grid [x, y] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * 2 - 7, y * 2 - 4, 10), Quaternion.identity);
				grid [x, y].GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
			}
		}
		
		//setting pusher locations
		pusherLocations = new Vector3[numCubesX * 2 + numCubesY * 2];
		int pusherIndex = 0;
		
		for (int x = 0; x < numCubesX; x++) {
			pusherLocations [pusherIndex++] = new Vector3 (x * 2 - 7, 6, 10);
		}
		for (int y = numCubesY-1; y >= 0; y--) {
			pusherLocations [pusherIndex++] = new Vector3 (9, y * 2 - 4, 10);
		}
		for (int x = numCubesX-1; x >= 0; x--) {
			pusherLocations [pusherIndex++] = new Vector3 (x * 2 - 7, -6, 10);
		} 
		for (int y = 0; y < numCubesY; y++) {
			pusherLocations [pusherIndex++] = new Vector3 (-9, y * 2 - 4, 10);
		}
		
		//clears 3+ matches
		FirstClear ();
		Invoke ("FirstGridSpawn", 0);

		//set text
		lootText.text = "BK: " + numLoot [0] + " BL: " + numLoot [1] + " G: " + numLoot [2] + " R: " + numLoot [3] + " Y: " + numLoot [4] + " W: " + numLoot [5];
		scoreText.text = "Turn: " + numTurns + " Score: " + score;
	}

	void FirstClear () {
		//checks rows for matches & destroys matches when the game starts
		for (int y = 0; y < numCubesY; y++) {
			for (int x = 1; x < numCubesX-1; x++) {
				if (grid [x,y] != null && grid [x,y].GetComponent<Renderer>().material.color == grid [x-1,y].GetComponent<Renderer>().material.color && grid [x,y].GetComponent<Renderer>().material.color == grid [x+1,y].GetComponent<Renderer>().material.color) {
					Destroy (grid[x,y].gameObject);
					Destroy (grid[x-1,y].gameObject);
					Destroy (grid[x+1,y].gameObject);
				}
			}
		}
		
		//checks columns for matches & destroys matches when the game starts
		for (int x = 0; x < numCubesX; x++) {
			for (int y = 1; y < numCubesY-1; y++) {
				if (grid [x,y] != null && grid [x, y].GetComponent<Renderer>().material.color == grid [x,y-1].GetComponent<Renderer>().material.color && grid [x,y].GetComponent<Renderer>().material.color == grid [x,y+1].GetComponent<Renderer>().material.color) {
					Destroy (grid[x,y].gameObject);
					Destroy (grid[x,y-1].gameObject);
					Destroy (grid[x,y+1].gameObject);
				}
			}
		}
	}
	
	void FirstGridSpawn () {
		//instantiate new cubes in blanks when the game starts
		for (int y = 0; y < numCubesY; y++) {
			for (int x = 0; x < numCubesX; x++) {
				if (grid[x,y] == null) {
					grid [x,y] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * 2 - 7, y * 2 - 4, 10), Quaternion.identity);
					grid [x,y].GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
				}
			}
		}
	}

	void MakePushers () {
		//instantiating pushers
		pusherOne = (GameObject)Instantiate (cubePrefab, pusherLocations [0], Quaternion.identity);
		pusherOne.GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
		pusherOneLocation = 0;
		pusherTwo = (GameObject)Instantiate (cubePrefab, pusherLocations [1], Quaternion.identity);
		pusherTwo.GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
		pusherTwoLocation = 1;
	}

	//this function updates the text at the top depending on which phase
	void SwitchPhases () {
		if (planningPhase == true) {
			instructions.text = "Move pushers with A/D and LEFT/RIGHT";
			actionStart.gameObject.SetActive(true);
			scoreText.text = "Turn: " + numTurns + " Score: " + score;
			if (pushersSpawned == false) {
				MakePushers ();
				pushersSpawned = true;
			}
		} else if (actionPhase == true) {
			instructions.text = "Timer: " + theTimer.ToString("F2") + "    Click cubes to destroy them";
		} else if (resolutionPhase == true) {
			instructions.text = null;
		}
	}

	//action button
	public void StartActionPhase () {
		CancelInvoke ("FirstGridSpawn");
		theTimer = turnTime;
		actionPhase = true;
		planningPhase = false;
		resolutionPhase = false;
		actionStart.gameObject.SetActive(false);
	}

	void PusherPlanning () {
		//movement for pusher one a goes ccw d goes cw
		if (Input.GetKeyDown (KeyCode.A)) {
			if (pusherOneLocation == 0 && pusherTwoLocation != (numCubesX * 2 + numCubesY * 2)-1) {
				pusherOne.transform.position = pusherLocations[(numCubesX * 2 + numCubesY * 2)-1];
				pusherOneLocation = (numCubesX * 2 + numCubesY * 2)-1;
			}
			else if (pusherOneLocation == 0 && pusherTwoLocation == (numCubesX * 2 + numCubesY * 2)-1) {
				pusherOne.transform.position = pusherLocations[(numCubesX * 2 + numCubesY * 2)-2];
				pusherOneLocation = (numCubesX * 2 + numCubesY * 2) - 2;
			}
			else if (pusherOneLocation != 0 && pusherTwoLocation != pusherOneLocation - 1) {
				pusherOne.transform.position = pusherLocations[pusherOneLocation-1];
				pusherOneLocation--;
			}
			else if (pusherOneLocation != 0 && pusherTwoLocation == pusherOneLocation - 1 && pusherTwoLocation != 0) {
				pusherOne.transform.position = pusherLocations[pusherOneLocation-2];
				pusherOneLocation = pusherOneLocation - 2;
			}
			else if (pusherOneLocation != 0 && pusherTwoLocation == pusherOneLocation - 1 && pusherTwoLocation == 0) {
				pusherOne.transform.position = pusherLocations[(numCubesX * 2 + numCubesY * 2)-1];
				pusherOneLocation =  (numCubesX * 2 + numCubesY * 2)-1;
			}
		}
		
		if (Input.GetKeyDown (KeyCode.D)) {
			if (pusherOneLocation == (numCubesX * 2 + numCubesY * 2)-1 && pusherTwoLocation != 0) {
				pusherOne.transform.position = pusherLocations[0];
				pusherOneLocation = 0;
			}
			else if (pusherOneLocation == (numCubesX * 2 + numCubesY * 2)-1 && pusherTwoLocation == 0) {
				pusherOne.transform.position = pusherLocations[1];
				pusherOneLocation = 1;
			}
			else if (pusherOneLocation != (numCubesX * 2 + numCubesY * 2)-1 && pusherTwoLocation != pusherOneLocation + 1) {
				pusherOne.transform.position = pusherLocations[pusherOneLocation+1];
				pusherOneLocation++;
			}
			else if (pusherOneLocation != (numCubesX * 2 + numCubesY * 2)-1 && pusherTwoLocation == pusherOneLocation + 1 && pusherTwoLocation != (numCubesX * 2 + numCubesY * 2)-1) {
				pusherOne.transform.position = pusherLocations[pusherOneLocation+2];
				pusherOneLocation = pusherOneLocation + 2;
			}
			else if (pusherOneLocation != (numCubesX * 2 + numCubesY * 2)-1 && pusherTwoLocation == pusherOneLocation + 1 && pusherTwoLocation == (numCubesX * 2 + numCubesY * 2)-1) {
				pusherOne.transform.position = pusherLocations[0];
				pusherOneLocation = 0;
			}
		}

		//movement for pusher two left goes ccw right goes cw
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (pusherTwoLocation == 0 && pusherOneLocation != (numCubesX*2 + numCubesY*2)-1) {
				pusherTwo.transform.position = pusherLocations[(numCubesX * 2 + numCubesY * 2)-1];
				pusherTwoLocation = (numCubesX * 2 + numCubesY * 2)-1;
			}
			else if (pusherTwoLocation == 0 && pusherOneLocation == (numCubesX * 2 + numCubesY * 2)-1) {
				pusherTwo.transform.position = pusherLocations[(numCubesX * 2 + numCubesY * 2)-2];
				pusherTwoLocation = (numCubesX*2 + numCubesY*2)-2;
			}
			else if (pusherTwoLocation != 0 && pusherOneLocation != pusherTwoLocation-1) {
				pusherTwo.transform.position = pusherLocations[pusherTwoLocation-1];
				pusherTwoLocation--;
			}
			else if (pusherTwoLocation != 0 && pusherOneLocation == pusherTwoLocation-1 && pusherOneLocation != 0) {
				pusherTwo.transform.position = pusherLocations[pusherTwoLocation-2];
				pusherTwoLocation = pusherTwoLocation - 2;
			}
			else if (pusherTwoLocation != 0 && pusherOneLocation == pusherTwoLocation-1 && pusherOneLocation == 0) {
				pusherTwo.transform.position = pusherLocations[(numCubesX * 2 + numCubesY * 2)-1];
				pusherTwoLocation = (numCubesX * 2 + numCubesY * 2)-1;
			}
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			if (pusherTwoLocation == (numCubesX*2 + numCubesY*2)-1 && pusherOneLocation != 0) {
				pusherTwo.transform.position = pusherLocations[0];
				pusherTwoLocation = 0;
			}
			else if (pusherTwoLocation == (numCubesX*2 + numCubesY*2)-1 && pusherOneLocation == 0) {
				pusherTwo.transform.position = pusherLocations[1];
				pusherTwoLocation = 1;
			}
			else if (pusherTwoLocation != (numCubesX*2 + numCubesY*2)-1 && pusherOneLocation != pusherTwoLocation+1) {
				pusherTwo.transform.position = pusherLocations[pusherTwoLocation+1];
				pusherTwoLocation++;
			}
			else if (pusherTwoLocation != (numCubesX*2 + numCubesY*2)-1 && pusherOneLocation == pusherTwoLocation+1 && pusherOneLocation != (numCubesX*2 + numCubesY*2)-1) {
				pusherTwo.transform.position = pusherLocations[pusherTwoLocation+2];
				pusherTwoLocation = pusherTwoLocation + 2;
			}
			else if (pusherTwoLocation != (numCubesX*2 + numCubesY*2)-1 && pusherOneLocation == pusherTwoLocation+1 && pusherOneLocation == (numCubesX*2 + numCubesY*2)-1){
				pusherTwo.transform.position = pusherLocations[0];
				pusherTwoLocation = 0;
			}
		}
	}

	IEnumerator PusherOneMovement () {
		//moves down into grid if on top of grid
		if (pusherOneLocation < numCubesX) {
			//isolate pusher x
			int x = (int)(pusherLocations [pusherOneLocation].x + 7) / 2;
		
			//checks # of blank spaces
			int numBlanks = 0;
			for (int y = 0; y < numCubesY; y++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherOne, new Vector3 (x * 2 - 7, (numCubesY - numBlanks) * 2 - 4, 10), 1.5f);
		
			//move cubes down to fill spaces if there are blanks
			for (int y = 0; y < numCubesY-1; y++) {
				if (grid[x,y] == null && grid[x,y+1] != null) {
					iTween.MoveTo(grid[x,y+1], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x,y+1];
					grid[x,y+1] = null;
				}
			}	

			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [x, numCubesY - numBlanks] = pusherOne;
				pusherOne = null;
			}

			//if there are no blanks, all of the cubes move
			if (grid [x, 4] != null && grid [x, 3] != null && grid [x, 2] != null && grid [x, 1] != null && grid [x, 0] != null) {
				iTween.MoveTo (pusherOne, new Vector3 (x * 2 - 7, 4, 10), 1.5f);
				for (int y = 0; y < numCubesY; y++) {
					iTween.MoveTo (grid [x, y], new Vector3 (x * 2 - 7, (y - 1) * 2 - 4, 10), 1.5f);
					//marks the bottom cube as loot to get destroyed
					if (y == 0) {
						grid [x, y].gameObject.tag = "Loot";
					}

					//pusher becomes top, the rest move down
					if (y == 4) {
						grid [x, y] = pusherOne;
						pusherOne = null;
					} else {
						grid [x, y] = grid [x, y + 1];
					}
				}
			}
		}
		//moves left into the grid if on the right of the grid
		else if (pusherOneLocation >= numCubesX && pusherOneLocation < numCubesX + numCubesY) {
			//isolate pusher y
			int y = (int)(pusherLocations [pusherOneLocation].y + 4) / 2;
			
			//checks # of blank spaces
			int numBlanks = 0;
			for (int x = 0; x < numCubesX; x++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherOne, new Vector3 ((numCubesX- numBlanks) * 2 - 7, y * 2 - 4, 10), 1.5f);
			
			//move cubes left to fill spaces if there are blanks
			for (int x = 0; x < numCubesX-1; x++) {
				if (grid[x,y] == null && grid[x+1,y] != null) {
					iTween.MoveTo(grid[x+1,y], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x+1,y];
					grid[x+1,y] = null;
				}
			}	
			
			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [numCubesX - numBlanks, y] = pusherOne;
				pusherOne = null;
			}
			
			//if there are no blanks, all of the cubes move
			if (grid [7, y] != null && grid [6, y] != null && grid [5, y] != null && grid [4, y] != null && grid [3, y] != null && grid[2, y] != null && grid[1,y] != null && grid[0,y] != null) {
				iTween.MoveTo (pusherOne, new Vector3 (7, y * 2 - 4, 10), 1.5f);
				for (int x = 0; x < numCubesX; x++) {
					iTween.MoveTo (grid [x, y], new Vector3 ((x-1) * 2 - 7, y * 2 - 4, 10), 1.5f);
					//marks the left cube as loot to get destroyed
					if (x == 0) {
						grid [x, y].gameObject.tag = "Loot";
					}
					
					//pusher becomes right, the rest move left
					if (x == 7) {
						grid [x, y] = pusherOne;
						pusherOne = null;
					} else {
						grid [x, y] = grid [x, y + 1];
					}
				}
			}
			}
		//moves up into the grid if on the bottom of the grid
		else if (pusherOneLocation >= numCubesX + numCubesY && pusherOneLocation < numCubesX * 2 + numCubesY) {
			//isolate pusher x
			int x = (int)(pusherLocations [pusherOneLocation].x + 7) / 2;
			
			//checks # of blank spaces
			int numBlanks = 0;
			for (int y = 0; y < numCubesY; y++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherOne, new Vector3 (x * 2 - 7, (-1 + numBlanks) * 2 - 4, 10), 1.5f);
			
			//move cubes up to fill spaces if there are blanks
			for (int y = 4; y > -1; y--) {
				if (grid[x,y] == null && grid[x,y-1] != null) {
					iTween.MoveTo(grid[x,y-1], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x,y-1];
					grid[x,y-1] = null;
				}
			}	
			
			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [x, -1 + numBlanks] = pusherOne;
				pusherOne = null;
			}
			
			//if there are no blanks, all of the cubes move
			if (grid [x, 4] != null && grid [x, 3] != null && grid [x, 2] != null && grid [x, 1] != null && grid [x, 0] != null) {
				iTween.MoveTo (pusherOne, new Vector3 (x * 2 - 7, -4, 10), 1.5f);
				for (int y = 4; y > -1; y--) {
					iTween.MoveTo (grid [x, y], new Vector3 (x * 2 - 7, (y + 1) * 2 - 4, 10), 1.5f);
						//marks the top cube as loot to get destroyed
						if (y == 4) {
							grid [x, y].gameObject.tag = "Loot";
						}
					
						//pusher becomes bottom, the rest move up
						if (y == 0) {
							grid [x, y] = pusherOne;
							pusherOne = null;
						} else {
							grid [x, y] = grid [x, y - 1];
						}
					}
				}
			}
		//moves right into the grid if on the left of the grid
		else if (pusherOneLocation >= numCubesX * 2 + numCubesY && pusherOneLocation < numCubesX * 2 + numCubesY * 2) {
			//isolate pusher y
			int y = (int)(pusherLocations [pusherOneLocation].y + 4) / 2;
			
			//checks # of blank spaces
			int numBlanks = 0;
			for (int x = 0; x < numCubesX; x++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherOne, new Vector3 ((-1 + numBlanks) * 2 - 7, y * 2 - 4, 10), 1.5f);
			
			//move cubes right to fill spaces if there are blanks
			for (int x = 7; x > 0; x--) {
				if (grid[x,y] == null && grid[x-1,y] != null) {
					iTween.MoveTo(grid[x-1,y], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x-1,y];
					grid[x-1,y] = null;
				}
			}	
			
			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [-1 + numBlanks, y] = pusherOne;
				pusherOne = null;
			}
			
			//if there are no blanks, all of the cubes move
			if (grid [7, y] != null && grid [6, y] != null && grid [5, y] != null && grid [4, y] != null && grid [3, y] != null && grid[2, y] != null && grid[1,y] != null && grid[0,y] != null) {
				iTween.MoveTo (pusherOne, new Vector3 (-7, y * 2 - 4, 10), 1.5f);
				for (int x = 7; x > -1; x--) {
					iTween.MoveTo (grid [x, y], new Vector3 ((x+1) * 2 - 7, y * 2 - 4, 10), 1.5f);
					//marks the right cube as loot to get destroyed
					if (x == 7) {
						grid [x, y].gameObject.tag = "Loot";
					}
					
					//pusher becomes left, the rest move right
					if (x == 0) {
						grid [x, y] = pusherOne;
						pusherOne = null;
					} else {
						grid [x, y] = grid [x, y - 1];
					}
				}
			}
			}
		pusherOneMoved = true;
		yield break;
	}

	IEnumerator PusherTwoMovement () {
		yield return new WaitForSeconds(2.5f);
		//moves down into grid if on top of grid
		if (pusherTwoLocation < numCubesX) {
			//isolate pusher x
			int x = (int)(pusherLocations [pusherTwoLocation].x + 7) / 2;
			
			//checks # of blank spaces
			int numBlanks = 0;
			for (int y = 0; y < numCubesY; y++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherTwo, new Vector3 (x * 2 - 7, (numCubesY - numBlanks) * 2 - 4, 10), 1.5f);
			
			//move cubes down to fill spaces if there are blanks
			for (int y = 0; y < numCubesY-1; y++) {
				if (grid[x,y] == null && grid[x,y+1] != null) {
					iTween.MoveTo(grid[x,y+1], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x,y+1];
					grid[x,y+1] = null;
				}
			}	
			
			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [x, numCubesY - numBlanks] = pusherTwo;
				pusherTwo = null;
			}
			
			//if there are no blanks, all of the cubes move
			if (grid [x, 4] != null && grid [x, 3] != null && grid [x, 2] != null && grid [x, 1] != null && grid [x, 0] != null) {
				iTween.MoveTo (pusherTwo, new Vector3 (x * 2 - 7, 4, 10), 1.5f);
				for (int y = 0; y < numCubesY; y++) {
					iTween.MoveTo (grid [x, y], new Vector3 (x * 2 - 7, (y - 1) * 2 - 4, 10), 1.5f);
					//marks the bottom cube as loot to get destroyed
					if (y == 0) {
						grid [x, y].gameObject.tag = "Loot";
					}
					
					//pusher becomes top, the rest move down
					if (y == 4) {
						grid [x, y] = pusherTwo;
						pusherTwo = null;
					} else {
						grid [x, y] = grid [x, y + 1];
					}
				}
			}
		}
		//moves left into the grid if on the right of the grid
		else if (pusherTwoLocation >= numCubesX && pusherTwoLocation < numCubesX + numCubesY) {
			//isolate pusher y
			int y = (int)(pusherLocations [pusherTwoLocation].y + 4) / 2;
			
			//checks # of blank spaces
			int numBlanks = 0;
			for (int x = 0; x < numCubesX; x++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherTwo, new Vector3 ((numCubesX- numBlanks) * 2 - 7, y * 2 - 4, 10), 1.5f);
			
			//move cubes left to fill spaces if there are blanks
			for (int x = 0; x < numCubesX-1; x++) {
				if (grid[x,y] == null && grid[x+1,y] != null) {
					iTween.MoveTo(grid[x+1,y], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x+1,y];
					grid[x+1,y] = null;
				}
			}	
			
			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [numCubesX - numBlanks, y] = pusherTwo;
				pusherTwo = null;
			}
			
			//if there are no blanks, all of the cubes move
			if (grid [7, y] != null && grid [6, y] != null && grid [5, y] != null && grid [4, y] != null && grid [3, y] != null && grid[2, y] != null && grid[1,y] != null && grid[0,y] != null) {
				iTween.MoveTo (pusherTwo, new Vector3 (7, y * 2 - 4, 10), 1.5f);
				for (int x = 0; x < numCubesX; x++) {
					iTween.MoveTo (grid [x, y], new Vector3 ((x-1) * 2 - 7, y * 2 - 4, 10), 1.5f);
					//marks the left cube as loot to get destroyed
					if (x == 0) {
						grid [x, y].gameObject.tag = "Loot";
					}
					
					//pusher becomes right, the rest move left
					if (x == 7) {
						grid [x, y] = pusherTwo;
						pusherTwo = null;
					} else {
						grid [x, y] = grid [x, y + 1];
					}
				}
			}
		}
		//moves up into the grid if on the bottom of the grid
		else if (pusherTwoLocation >= numCubesX + numCubesY && pusherTwoLocation < numCubesX * 2 + numCubesY) {
			//isolate pusher x
			int x = (int)(pusherLocations [pusherTwoLocation].x + 7) / 2;
			
			//checks # of blank spaces
			int numBlanks = 0;
			for (int y = 0; y < numCubesY; y++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherTwo, new Vector3 (x * 2 - 7, (-1 + numBlanks) * 2 - 4, 10), 1.5f);
			
			//move cubes up to fill spaces if there are blanks
			for (int y = 4; y > 0; y--) {
				if (grid[x,y] == null && grid[x,y-1] != null) {
					iTween.MoveTo(grid[x,y-1], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x,y-1];
					grid[x,y-1] = null;
				}
			}	
			
			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [x, -1 + numBlanks] = pusherTwo;
				pusherTwo = null;
			}
			
			//if there are no blanks, all of the cubes move
			if (grid [x, 4] != null && grid [x, 3] != null && grid [x, 2] != null && grid [x, 1] != null && grid [x, 0] != null) {
				iTween.MoveTo (pusherTwo, new Vector3 (x * 2 - 7, -4, 10), 1.5f);
				for (int y = 4; y > -1; y--) {
					iTween.MoveTo (grid [x, y], new Vector3 (x * 2 - 7, (y + 1) * 2 - 4, 10), 1.5f);
					//marks the top cube as loot to get destroyed
					if (y == 4) {
						grid [x, y].gameObject.tag = "Loot";
					}
					
					//pusher becomes bottom, the rest move up
					if (y == 0) {
						grid [x, y] = pusherTwo;
						pusherTwo = null;
					} else {
						grid [x, y] = grid [x, y - 1];
					}
				}
			}
		}
		//moves right into the grid if on the left of the grid
		else if (pusherTwoLocation >= numCubesX * 2 + numCubesY && pusherTwoLocation < numCubesX * 2 + numCubesY * 2) {
			//isolate pusher y
			int y = (int)(pusherLocations [pusherTwoLocation].y + 4) / 2;
			
			//checks # of blank spaces
			int numBlanks = 0;
			for (int x = 0; x < numCubesX; x++) {
				if (grid [x, y] == null) {
					numBlanks++;
				}
			}
			iTween.MoveTo (pusherTwo, new Vector3 ((-1 + numBlanks) * 2 - 7, y * 2 - 4, 10), 1.5f);
			
			//move cubes right to fill spaces if there are blanks
			for (int x = 7; x > 0; x--) {
				if (grid[x,y] == null && grid[x-1,y] != null) {
					iTween.MoveTo(grid[x-1,y], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
					grid [x,y] = grid [x-1,y];
					grid[x-1,y] = null;
				}
			}	
			
			yield return new WaitForSeconds(.1f);
			if (numBlanks != 0) {
				grid [-1 + numBlanks, y] = pusherTwo;
				pusherTwo = null;
			}
			
			//if there are no blanks, all of the cubes move
			if (grid [7, y] != null && grid [6, y] != null && grid [5, y] != null && grid [4, y] != null && grid [3, y] != null && grid[2, y] != null && grid[1,y] != null && grid[0,y] != null) {
				iTween.MoveTo (pusherTwo, new Vector3 (-7, y * 2 - 4, 10), 1.5f);
				for (int x = 7; x > -1; x--) {
					iTween.MoveTo (grid [x, y], new Vector3 ((x+1) * 2 - 7, y * 2 - 4, 10), 1.5f);
					//marks the right cube as loot to get destroyed
					if (x == 7) {
						grid [x, y].gameObject.tag = "Loot";
					}
					
					//pusher becomes left, the rest move right
					if (x == 0) {
						grid [x, y] = pusherTwo;
						pusherTwo = null;
					} else {
						grid [x, y] = grid [x, y - 1];
					}
				}
			}
		}
		pusherTwoMoved = true;
		yield break;
	}

	void LootScoring () {
		//fade cubes outside of grid and add to array
		for (int i = 0; i < colors.Length; i++) {
			if (GameObject.FindWithTag ("Loot").gameObject.GetComponent<Renderer>().material.color.Equals(colors[i]) && lootCounted == false) {
				numLoot[i]++;
				lootText.text = "BK: " + numLoot [0] + " BL: " + numLoot [1] + " G: " + numLoot [2] + " R: " + numLoot [3] + " Y: " + numLoot [4] + " W: " + numLoot [5];
				lootCounted = true;
			}
			else {
				lootCounted = false;
			}
		}
		iTween.FadeTo (GameObject.FindWithTag ("Loot"), 0.0f, 1.0f);
		Destroy (GameObject.FindWithTag ("Loot"), 1.0f);
	}

	IEnumerator FindMatches () {
		yield return new WaitForSeconds(4.5f);
		//checks rows for matches
		for (int y = 0; y < numCubesY; y++) {
			for (int x = 1; x < numCubesX-1; x++) {
				if (grid[x,y] != null && grid[x,y].gameObject.GetComponent<Renderer> ().material.color == grid[x-1,y].gameObject.GetComponent<Renderer> ().material.color && grid[x,y].gameObject.GetComponent<Renderer> ().material.color == grid[x+1,y].gameObject.GetComponent<Renderer> ().material.color) {
					grid[x,y].gameObject.tag = "Match";
					grid[x-1,y].gameObject.tag = "Match";
					grid[x+1,y].gameObject.tag = "Match";
					if (x < numCubesX - 2 && grid[x+2,y].gameObject.GetComponent<Renderer>().material.color == grid[x,y].gameObject.GetComponent<Renderer>().material.color) {
						grid[x+2,y].gameObject.tag = "Match";
						if (x < numCubesX - 3 && grid[x+3,y].gameObject.GetComponent<Renderer>().material.color == grid[x,y].gameObject.GetComponent<Renderer>().material.color) {
							grid[x+3,y].gameObject.tag = "Match";
							if (x < numCubesX - 4 && grid[x+4,y].gameObject.GetComponent<Renderer>().material.color == grid[x,y].gameObject.GetComponent<Renderer>().material.color) {
								grid[x+4,y].gameObject.tag = "Match";
								if (grid[x+5,y].gameObject.GetComponent<Renderer>().material.color == grid[x,y].gameObject.GetComponent<Renderer>().material.color && x < numCubesX - 5) {
									grid[x+4,y].gameObject.tag = "Match";
									if (grid[x+6,y].gameObject.GetComponent<Renderer>().material.color == grid[x,y].gameObject.GetComponent<Renderer>().material.color && x < numCubesX - 6) {
										grid[x+6,y].gameObject.tag = "Match";
									}
								}
							}
						}
					}
				}
			}
		}

		//checks columns for matches
		for (int x = 0; x < numCubesX; x++) {
			for (int y = 1; y < numCubesY-1; y++) {
				if (grid [x, y] != null && grid [x, y].gameObject.GetComponent<Renderer> ().material.color == grid [x, y - 1].gameObject.GetComponent<Renderer> ().material.color && grid [x, y].gameObject.GetComponent<Renderer> ().material.color == grid [x, y + 1].gameObject.GetComponent<Renderer> ().material.color) {
					grid[x,y].gameObject.tag = "Match";
					grid[x,y-1].gameObject.tag = "Match";
					grid[x,y+1].gameObject.tag = "Match";
					if (y < numCubesY-2 && grid[x,y+2].gameObject.GetComponent<Renderer>().material.color == grid[x,y].gameObject.GetComponent<Renderer>().material.color) {
						grid[x,y+2].gameObject.tag = "Match";
						if (y < numCubesY -3 && grid[x,y+3].gameObject.GetComponent<Renderer>().material.color == grid[x,y].gameObject.GetComponent<Renderer>().material.color) {
							grid[x,y+3].gameObject.tag = "Match";
						}
					}
				}
			}
		}
		yield break;
	}
	
	IEnumerator ClearMatches () {
		yield return new WaitForSeconds(5f);
		//make array of match cubes
		matchList = GameObject.FindGameObjectsWithTag ("Match");

		//deletes match cubes
		for (int i = 0; i < matchList.Length; i++) {
			iTween.FadeUpdate (matchList[i], 0.0f, 1.0f);
			yield return new WaitForSeconds (1.0f);
			Destroy (matchList[i].gameObject);
			matchList[i] = null;
		}

		
	/*			//checks rows for matches & destroys matches
		for (int y = 0; y < numCubesY; y++) {
			for (int x = 1; x < numCubesX-1; x++) {
				if (grid [x, y] != null && grid [x, y].GetComponent<Renderer> ().material.color == grid [x - 1, y].GetComponent<Renderer> ().material.color && grid [x, y].GetComponent<Renderer> ().material.color == grid [x + 1, y].GetComponent<Renderer> ().material.color) {
					iTween.FadeUpdate (grid [x, y], 0.0f, 0.5f);
					iTween.FadeUpdate (grid [x - 1, y], 0.0f, 0.5f);
					iTween.FadeUpdate (grid [x + 1, y], 0.0f, 0.5f);
					yield return new WaitForSeconds (.5f);
					grid [x, y] = null;
					grid [x - 1, y] = null;
					grid [x + 1, y] = null;
					score += 10;
					scoreText.text = "Turn: " + numTurns + " Score: " + score;
				}
			}
		}

		//checks columns for matches & destroys matches
		for (int x = 0; x < numCubesX; x++) {
			for (int y = 1; y < numCubesY-1; y++) {
				int numCubesInMatch = 0;
				if (grid [x, y] != null && grid [x, y].GetComponent<Renderer> ().material.color == grid [x, y - 1].GetComponent<Renderer> ().material.color && grid [x, y].GetComponent<Renderer> ().material.color == grid [x, y + 1].GetComponent<Renderer> ().material.color) {
					iTween.FadeUpdate (grid [x, y], 0.0f, 0.5f);
					iTween.FadeUpdate (grid [x, y - 1], 0.0f, 0.5f);
					iTween.FadeUpdate (grid [x, y + 1], 0.0f, 0.5f);
					yield return new WaitForSeconds(.5f);
					grid[x,y] = null;
					grid[x-1,y] = null;
					grid[x+1,y] = null;
					numCubesInMatch += 3;
					score += (numCubesInMatch-2)*10;
					scoreText.text = "Turn: " + numTurns + " Score: " + score;
				}
			}
		}*/
		yield break;
	}
	
	IEnumerator MoveCubesDown () {
		yield return new WaitForSeconds(9);
		//move cubes down
		for (int y = 1; y < numCubesY; y++) {
			for (int x = 0; x < numCubesX; x++) {
				if (grid[x,y-1] == null && grid[x,y] != null) {
					iTween.MoveTo(grid[x,y], new Vector3(x * 2 - 7, (y-1) * 2 - 4, 10), 1.5f);
					grid [x,y-1] = grid [x,y];
					grid[x,y] = null;
				}
			}
		}
		yield break;
	}

	IEnumerator NewCubesFall () {
		yield return new WaitForSeconds(11);
		//instantiate new cubes
		for (int y = 0; y < numCubesY; y++) {
			for (int x = 0; x < numCubesX; x++) {
				if (grid[x,y] == null) {
					grid [x,y] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * 2 - 7, 6, 10), Quaternion.identity);
					grid [x,y].GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
					iTween.MoveTo(grid[x,y], new Vector3(x * 2 - 7, y * 2 - 4, 10), 1.5f);
				}
			}
		}
		cubesHaveFallen = true;
		yield break;
	}

	void StartPlanningPhase () {
		resolutionPhase = false;
		actionPhase = false;
		planningPhase = true;
		numTurns++;
	}

	// Update is called once per frame
	void Update () {
		SwitchPhases ();

		//planning phase: clears matches, allows pushers to move around border
		if (planningPhase == true) {
			pusherOneMoved = false;
			pusherTwoMoved = false;
			cubesHaveFallen = false;
			lootCounted = false;
			StopAllCoroutines ();
			CancelInvoke ("LootScoring");
			PusherPlanning ();
			resolutionPhase = false;
			actionPhase = false;
		}

		//action phase: in cubebehavior it lets you destroy cubes, timer is going
		if (actionPhase == true) {
			theTimer -= Time.deltaTime;
			if (theTimer <= 0) {
				actionPhase = false;
				resolutionPhase = true;
			}
		}

		//resolution phase: the pushers move into grid, cubes either move out or into empty spaces,
		//matches of 3+ are cleared, gravity works on cubes above empty slots, new cubes generated,
		//loops until there are no more matches
		if (resolutionPhase == true) {
			theTimer = 0;
			if (pusherOneMoved == false) {
				StartCoroutine (PusherOneMovement ());
			}
			if (pusherTwoMoved == false) {
				StartCoroutine (PusherTwoMovement ());
			}
			Invoke ("LootScoring", 5.0f);
			StartCoroutine (FindMatches ());
			StartCoroutine (ClearMatches ());
			StartCoroutine (MoveCubesDown ());
			StartCoroutine (NewCubesFall ());
			pushersSpawned = false;
			
			if (matchList.Length == 0 && cubesHaveFallen == true) {
				StartPlanningPhase ();
			}
		}

		if (numTurns > 15) {
			sS.finalScore = score;
			Application.LoadLevel("Scene03");
		}
	}
}