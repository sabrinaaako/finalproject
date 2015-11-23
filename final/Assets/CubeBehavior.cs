using UnityEngine;
using System.Collections;

public class CubeBehavior : MonoBehaviour {
	GameController aGameController;

	void Start () {
		aGameController = GameObject.Find("GameObject").GetComponent<GameController>();
	}

	void OnMouseDown () {
	
	}

}
