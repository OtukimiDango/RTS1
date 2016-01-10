using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class serchBlock : MonoBehaviour {
	public static List<GameObject> hitAlly = new List<GameObject> ();
	void OnTriggerEnter(Collider col){
		hitAlly.Add (col.gameObject);
	}
	void OnTriggerExit(Collider col){
		hitAlly.Remove (col.gameObject);
	}
}