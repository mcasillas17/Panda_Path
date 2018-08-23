using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMethods5x5 : MonoBehaviour {

	public void LoadLevel(){
		SceneManager.LoadScene ("GenericLevel5x5");
	}

	public void LoadNextLevel(){
		GameSettings5x5.incrementCurrentLevel ();
		LoadLevel ();
	}

	public void ZoneSelection(){
		SceneManager.LoadScene ("LevelMap");
	}

}
