
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeCol : MonoBehaviour {

	void OnTriggerEnter (Collider col)
	{
		string mask = col.transform.parent.GetComponent<Soldier>().Name("myTag",false);
//		string notMask = col.GetComponent<Soldier>().otherName("myTag",true);
		string stopMask = col.transform.parent.GetComponent<Soldier>().Name("myStopTag",false);
		if (gameObject.transform.parent.CompareTag (mask) && col.gameObject.layer == 8 && col.gameObject.transform.parent.gameObject.layer == LayerMask.NameToLayer(mask)) {
			Debug.Log (col.gameObject);
			if (col.gameObject.transform.parent.CompareTag (stopMask)) {
				gameObject.transform.parent.GetComponent<Soldier> ().detourReady (col.gameObject.transform.parent.gameObject);
				Debug.Log (gameObject.transform.parent.name+" send from"+col.gameObject.transform.name);
			} else if (col.gameObject.transform.parent.CompareTag (mask)) {
				col.gameObject.transform.parent.GetComponent<Soldier> ().behindAlly.Add (gameObject.transform.parent.gameObject);
				Debug.Log (gameObject.transform.parent.name+" send from"+col.gameObject.transform.name);
			}
		}
	}
	void OnTriggerExit(Collider col){
		if (col.gameObject.layer == gameObject.transform.parent.gameObject.layer) {

				col.gameObject.GetComponent<Soldier> ().behindAlly.Remove (gameObject.transform.parent.gameObject);
		}
	}
}