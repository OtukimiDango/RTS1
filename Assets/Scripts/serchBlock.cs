using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class serchBlock : MonoBehaviour {
	public List<GameObject> hitAlly = new List<GameObject> ();
	void OnTriggerEnter(Collider col){
		if(col.gameObject.transform.parent.gameObject.layer == 10)
			hitAlly.Add (col.gameObject.transform.parent.gameObject);
	}
	void OnTriggerExit(Collider col){
		if(col.transform.parent.gameObject.layer == 10)
			hitAlly.Remove (col.transform.parent.gameObject);
	}
}