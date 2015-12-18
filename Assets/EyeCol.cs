using UnityEngine;
using System.Collections;

public class EyeCol : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == ("StopPlayer") && gameObject.transform.parent.tag == "Player") {
			gameObject.transform.parent.GetComponent<Red> ().frontAlly = col.gameObject;
			gameObject.transform.parent.GetComponent<Red> ().detourReady ();
		} else if (col.gameObject.tag == ("StopEnemy") && gameObject.transform.parent.tag == "Enemy") {
			gameObject.transform.parent.GetComponent<Blue> ().frontAlly = col.gameObject;
			gameObject.transform.parent.GetComponent<Blue> ().detourReady ();
		}
	}
}
