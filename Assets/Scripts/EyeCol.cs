
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeCol : MonoBehaviour {

	void OnTriggerEnter (Collider col)
	{
		string mask = col.GetComponent<Warrior>().otherName("myTag",false);
		string notMask = col.GetComponent<Warrior>().otherName("myTag",true);
		string stopMask = col.GetComponent<Warrior>().otherName("myStopTag",false);

		if (gameObject.transform.parent.CompareTag (mask) && col.gameObject.layer == LayerMask.NameToLayer (notMask)) {
			Debug.Log (col.gameObject);
			if (col.gameObject.transform.parent.CompareTag (stopMask)) {
				gameObject.transform.parent.GetComponent<Warrior> ().frontAlly = col.gameObject;
				gameObject.transform.parent.GetComponent<Warrior> ().detourReady ();
			} else if (col.gameObject.transform.parent.CompareTag (mask)) {
				gameObject.transform.parent.GetComponent<Warrior> ().frontAlly = col.gameObject;
				col.gameObject.transform.parent.GetComponent<Warrior> ().behindAlly.Add (gameObject.transform.parent.gameObject);
			}
		}
	}
	void OnTriggerExit(Collider col){
		if (col.gameObject.layer == gameObject.transform.parent.gameObject.layer) {

				col.gameObject.GetComponent<Warrior> ().behindAlly.Remove (gameObject.transform.parent.gameObject);
		}
	}
}