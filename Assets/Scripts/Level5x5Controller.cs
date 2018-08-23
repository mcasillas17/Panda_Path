using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Level5x5Controller : MonoBehaviour {

	public GameObject [,] grid;
	public int [,] states;
	int [,] marked;
	string level;
	GameObject panda;
	PandaController panda_Cont;
	public GameObject panda_pre;
	public GameObject flag_pre;
	public GameObject bamboo_pre;
	public GameObject cell_mark;
	public GameObject [,] grid_objects;
	bool isPandaSelected = false;
	Vector2 pandaPosition;
	int collected_bamboo = 0;
	int total_bamboo;
	int size = 5;
	int [] r = {-2,-1,1,2,2,1,-1,-2};
	int [] c = {1,2,2,1,-1,-2,-2,-1};
	GameObject [] neighbors;
	int moves = 0;
	bool playing = true;
	string [] levels;
	public Image msgWin, msgLose, msgBamboo;

	public Text txtMov;
	public Text txtBamboo;

	// Method that sets the matrix grid with the GameObjects in the level
	void set_Cells_Grid(){
		grid = new GameObject[size, size];
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				grid [i, j] = GameObject.Find ("Cell_" + i + "_" + j);
			}
		}
	}

	// Method that reads a level from the txt files
	// TO_DO: determine the txt to read and the level using a GameSettings class
	void read_Level(){
		levels = new string[20];
		StreamReader inp_stm = new StreamReader("Assets/Resources/5x5.txt");
		int index = 0;
		while(!inp_stm.EndOfStream){
			string inp_ln = inp_stm.ReadLine( );
			levels [index++] = inp_ln;
		}
		inp_stm.Close( );
		int levelToLoad = GameSettings5x5.currentLevel;
		string temp = levels[levelToLoad];
		int i = 0;
		for (; i < temp.Length && temp [i] != '_'; i++) {
			level += temp [i];
		}
		i++;
		string num = "";
		for (; i < temp.Length; i++) {
			num += temp [i];
		}
		moves = int.Parse(num);
	}

	// Method that sets the state for the board using the string read from the txt files
	void set_States_Grid(){
		read_Level ();
		int level_index = 0;
		states = new int[size,size];
		marked = new int[size, size];
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				states [i, j] = level [level_index++] - '0';
				marked [i, j] = 0;
			}
		}
	}

	private Vector2 check_Input_In_Grid(Vector2 inputPosition){
		for (int i = 0; i < size; i++){
			for (int j = 0; j < size; j++){
				Ray ray = Camera.main.ScreenPointToRay(inputPosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
				//check the hit
				if (hit.collider != null && hit.collider.transform == grid[i, j].transform){
					return new Vector2(i, j);
				}
			}
		}
		return new Vector2(-1, -1);
	}


	// Method that sets the objects inside the grid
	void instantiate_Level(){
		grid_objects = new GameObject[size, size];
		neighbors = new GameObject[8];
		for (int i = 0; i < neighbors.Length; i++)
			neighbors [i] = null;
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				if (states [i, j] == 1) {
					grid_objects [i, j] = (GameObject)Instantiate (panda_pre, grid [i, j].transform.position, Quaternion.identity);
					pandaPosition = new Vector2 (i, j);
					panda = grid_objects [i, j];
					panda_Cont = panda.GetComponent<PandaController> ();
					panda_Cont.isMoving = false;
					panda_Cont.target = grid[i,j].transform;
				} else if (states [i, j] == 2) {
					grid_objects [i, j] = (GameObject)Instantiate (flag_pre, grid [i, j].transform.position, Quaternion.identity);
				} else if (states [i, j] == 3) {
					grid_objects [i, j] = (GameObject)Instantiate (bamboo_pre, grid [i, j].transform.position, bamboo_pre.transform.rotation);
					total_bamboo++;
				} else {
					grid_objects [i, j] = null;
				}
			}
		}
	}

	bool isValid(int x, int y){
		return x >= 0 && x < size && y >= 0 && y < size;
	}

	void highlight_Neighbors(int x, int y){
		int index = 0;
		for (int i = 0; i < r.Length; i++) {
			if (isValid (x + r [i], y + c [i])) {
				neighbors [index++] = (GameObject)Instantiate (cell_mark, grid [x + r [i], y + c [i]].transform.position, Quaternion.identity);
				marked [x + r [i], y + c [i]] = 1;
			}
		}
	}

	void unhighlight_Neighbors(){
		for (int i = 0; i < neighbors.Length; i++) {
			if (neighbors [i] == null)
				break;
			else {
				Destroy (neighbors [i]);
			}
		}
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				marked [i, j] = 0;
	}

	bool won_Cond(){
		return total_bamboo == collected_bamboo;
	}

	void check_Won(){
		if (won_Cond ()) {
			Debug.Log ("You WON!!!");
			Invoke ("showMsgWin", 0.7f);
		} else {
			Debug.Log ("You need to collect all the bamboo");
			Invoke ("showMsgBamboo", 0.7f);
		}
		playing = false;
	}

	void make_Changes(int x, int y){
		if (moves == 0) {
			if (states [x, y] == 2) {
				check_Won ();
			} else {
				Debug.Log ("You lost...");
				Invoke ("showMsgLose", 0.7f);
			}
			playing = false;
		} else {
			// If there is a bamboo
			if (states [x, y] == 3) {
				collected_bamboo++;
				txtBamboo.text = "x " + collected_bamboo;
				if (grid_objects [x, y] != null) {
					Destroy (grid_objects [x, y]);
				}
			} else if (states [x, y] == 2) {
				check_Won ();
			}
			states [x, y] = 1;
			states [(int)pandaPosition.x, (int)pandaPosition.y] = 0;
			pandaPosition = new Vector2 (x, y);
		}
	}

	void showMsgWin(){
		msgWin.gameObject.SetActive(true);
	}

	void showMsgLose(){
		msgLose.gameObject.SetActive(true);
	}

	void showMsgBamboo(){
		msgBamboo.gameObject.SetActive(true);
	}

	void Start () {
		set_Cells_Grid ();
		set_States_Grid ();
		instantiate_Level ();
		txtMov.text = "Moves: "+moves;
		txtBamboo.text = "x 0";
	}

	void Update () {
		Vector2 inputPosition = new Vector2(0,0);
		#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
		if (Input.GetMouseButtonUp (0) && playing){
			inputPosition = Input.mousePosition;
			if (isPandaSelected) {
				Vector2 tile = check_Input_In_Grid (inputPosition);
				if (!panda_Cont.isMoving && tile.x >= 0) {
					int x = (int)tile.x; int y = (int)tile.y;
					if(marked[x, y]==1){
						moves--;
						txtMov.text = "Moves: "+moves;
						panda_Cont.isMoving = true;
						panda_Cont.target = grid[x,y].transform;
						make_Changes(x,y);
					}
					unhighlight_Neighbors();
					isPandaSelected = false;
				}
			} else {
				Vector2 tile = check_Input_In_Grid (inputPosition);
				if (!panda_Cont.isMoving && tile.x >= 0 && states [(int)tile.x, (int)tile.y] == 1) {
					isPandaSelected = true;
					highlight_Neighbors ((int)tile.x,(int)tile.y);
				}
			}
		}
		#endif
		#if UNITY_ANDROID
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended && playing) {
			inputPosition = Input.GetTouch (0).position;
			if (isPandaSelected) {
				Vector2 tile = check_Input_In_Grid (inputPosition);
				if (!panda_Cont.isMoving && tile.x >= 0) {
					int x = (int)tile.x; int y = (int)tile.y;
					if(marked[x, y]==1){
						moves--;
						txtMov.text = "Moves: "+moves;
						panda_Cont.isMoving = true;
						panda_Cont.target = grid[x,y].transform;
						make_Changes(x,y);
					}
					unhighlight_Neighbors();
					isPandaSelected = false;
				}
			} else {
				Vector2 tile = check_Input_In_Grid (inputPosition);
				if (!panda_Cont.isMoving && tile.x >= 0 && states [(int)tile.x, (int)tile.y] == 1) {
					isPandaSelected = true;
					highlight_Neighbors ((int)tile.x,(int)tile.y);
				}
			}
		}
		#endif
	}
}
