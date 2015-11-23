using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {
	
	public void SceneChange (string NextScene) {
		Application.LoadLevel (NextScene);
	}
}
