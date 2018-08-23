using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings5x5{

	public static int currentLevel = 0;
	public static int maxLevel = 0;

	public static void incrementCurrentLevel(){
		currentLevel++;
		maxLevel = Mathf.Max (currentLevel, maxLevel);
	}

}
