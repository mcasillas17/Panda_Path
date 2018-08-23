using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaController : MonoBehaviour {

	public Transform target;
	public float speed;
	public bool isMoving;

	void Start(){
		isMoving = false;
	}

	float distance(Vector3 a, Vector3 b){
		return Mathf.Sqrt ((b.x - a.x) * (b.x - a.x) - (b.y - a.y) * (b.y - a.y));
	}

	void Update () {
		if (isMoving) {
			float step = speed * Time.deltaTime;
			Vector3 target2DPos = new Vector3 (target.position.x, target.position.y, transform.position.z);
			transform.position = Vector3.MoveTowards (transform.position, target2DPos, step);
			if (distance(transform.position, target2DPos) < 0.05f) {
				isMoving = false;
				transform.position = target2DPos;
			}
		}
	}
}
