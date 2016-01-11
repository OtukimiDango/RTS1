
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeCol : MonoBehaviour {

	void OnTriggerEnter (Collider col)
	{
		string mask = col.transform.parent.GetComponent<Soldier>().Name("myTag",false);
		//string notMask = col.GetComponent<Soldier>().otherName("myTag",true);
		string stopMask = col.transform.parent.GetComponent<Soldier>().Name("myStopTag",false);
		if (transform.parent.CompareTag (mask) && col.gameObject.layer == 8 
			&& col.gameObject.transform.parent.gameObject.layer == LayerMask.NameToLayer(mask)
			&& transform.parent.GetComponent<Soldier>().state != "fight") {
			if (col.gameObject.transform.parent.CompareTag (stopMask)) {
				if (transform.parent.name == "RedSoldier1")
					Debug.Log (transform.parent.GetComponent<Soldier> ().state);
				transform.parent.GetComponent<Soldier> ().StopAllCoroutines();
				transform.parent.GetComponent<Soldier> ().StartCoroutine(transform.parent.GetComponent<Soldier>().keepAway (col.gameObject.transform.parent.gameObject,10));
			} else if (col.gameObject.transform.parent.CompareTag (mask)) {
				col.gameObject.transform.parent.GetComponent<Soldier> ().behindAlly.Add (transform.parent.gameObject);
			}
		}
	}
	void OnTriggerExit(Collider col){
		if (col.gameObject.layer == gameObject.transform.parent.gameObject.layer) {

				col.gameObject.GetComponent<Soldier> ().behindAlly.Remove (gameObject.transform.parent.gameObject);
		}
	}
}