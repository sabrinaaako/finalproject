using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public GameObject cubePrefab;
	int numCubesX = 8;
	int numCubesY = 5;
	int pusherOneLocation;
	int pusherTwoLocation;
	GameObject[,] grid;
	public Vector3[] pusherLocations;
	public GameObject pusherOne;
	public GameObject pusherTwo;
	Color[] colors = {Color.black, Color.blue, Color.green, Color.red, Color.yellow, Color.white};
	float score = 0f;
	//float turnTime = 4f;
	//int numTurns = 0;


	// Use this for initialization
	void Start () {

	}

	void OnLevelWasLoaded (int level) {
		if (level == 1) {
			//instantiating cubes
			grid = new GameObject[numCubesX, numCubesY];
			for (int x = 0; x < numCubesX; x++) {
				for (int y = 0; y < numCubesY; y++) {
					grid [x, y] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * 2 - 7, y * 2 - 4, 10), Quaternion.identity);
					grid [x, y].GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
				}
			}

			//setting pusher locations
			pusherLocations = new Vector3[numCubesX*2 + numCubesY*2];
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

			//instantiating pushers
			pusherOne = (GameObject)Instantiate (cubePrefab, pusherLocations [0], Quaternion.identity);
			pusherOne.GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
			pusherOneLocation = 0;
			pusherTwo = (GameObject)Instantiate (cubePrefab, pusherLocations [1], Quaternion.identity);
			pusherTwo.GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
			pusherTwoLocation = 0;

		}
	}

	void PusherOnePlanning () {
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
	}

	void PusherTwoPlanning () {
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

	/*void PusherOneMovement () {
		//moves into grid separate for LRUP
		if (pusherOneLocation < numCubesX) {
			pusherOne.transform.position = new Vector3 (x * 2 - 7, 6, 10);
		}
		else if () {

		}
		else if () {

		}
		else if () {

		}
		//pusher object enters the grid array
		//moves other cubes in or out of grid and array
	}

	void PusherTwoMovement () {

	}

	public float TimeKeeping () {

	}*/
	
	void FindMatches () {
		//checks rows for matches & destroys matches
		for (int y = 0; y < numCubesY; y++) {
			for (int x = 1; x < numCubesX-1; x++) {
				if (grid [x, y] != null && grid [x, y].GetComponent<Renderer>().material.color == grid [x-1, y].GetComponent<Renderer>().material.color && grid [x, y].GetComponent<Renderer>().material.color == grid [x+1, y].GetComponent<Renderer>().material.color) {
					Destroy (grid[x,y].gameObject);
					Destroy (grid[x-1,y].gameObject);
					Destroy (grid[x+1,y].gameObject);
					score+=10;
					print ("Score: " + score);
				}
			}
		}
		
		//checks columns for matches & destroys matches
		for (int x = 0; x < numCubesX; x++) {
			for (int y = 1; y < numCubesY-1; y++) {
				if (grid [x, y] != null && grid [x, y].GetComponent<Renderer>().material.color == grid [x, y-1].GetComponent<Renderer>().material.color && grid [x, y].GetComponent<Renderer>().material.color == grid [x, y+1].GetComponent<Renderer>().material.color) {
					Destroy (grid[x,y].gameObject);
					Destroy (grid[x,y-1].gameObject);
					Destroy (grid[x,y+1].gameObject);
					score+=10;
					print ("Score: " + score);
				}
			}
		}
	}
	
	void MoveCubesDown () {
		//move cubes down
		for (int y = 1; y < numCubesY; y++) {
			for (int x = 0; x < numCubesX; x++) {
				if (grid[x,y-1] == null && grid[x,y] != null) {
					grid [x,y-1] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * 2 - 7, (y-1) * 2 - 4, 10), Quaternion.identity);
					grid [x,y-1].GetComponent<Renderer> ().material.color = grid[x,y].GetComponent<Renderer> ().material.color;
					Destroy (grid[x,y].gameObject);
				}
			}
		}
	}
	
	void MakeNewCubes () {
		//instantiate new cubes
		for (int y = 0; y < numCubesY; y++) {
			for (int x = 0; x < numCubesX; x++) {
				if (grid[x,y] == null) {
					grid [x,y] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * 2 - 7, y * 2 - 4, 10), Quaternion.identity);
					grid [x,y].GetComponent<Renderer> ().material.color = colors [Random.Range (0, colors.Length)];
				}
			}
		}
	}

	void LootScoring () {
		//score cubes outside of grid
		print ("LOOT SCORED");
	}

	// Update is called once per frame
	void Update () {
		PusherOnePlanning ();
		PusherTwoPlanning ();
	
		if (Time.time > 5 && Time.time < 6) {
			FindMatches ();
		}
		if (Time.time > 6 && Time.time < 7) {
			MoveCubesDown ();
		}
		if (Time.time > 7 && Time.time < 8) {
			MakeNewCubes ();
		}
	}
}