using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreStats : MonoBehaviour {
	GameController aGameController;
	public float finalScore;
	public Text finalText;

	// Use this for initialization
	void Start () {
		aGameController = GameObject.Find("GameObject").GetComponent<GameController>();
		finalText.text = "Score: " + finalScore;
	}
	
	// Update is called once per frame
	void Update () {
		finalText.text = "Score: " + finalScore;
	}
}
