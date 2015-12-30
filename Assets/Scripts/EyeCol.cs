using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeCol : MonoBehaviour {

	void OnTriggerEnter (Collider col)
	{
		if (gameObject.transform.parent.tag == "Player") {
			if(col.gameObject.tag == ("StopPlayer")){
				try{
				gameObject.transform.parent.GetComponent<Blue> ().frontAlly = col.gameObject;
				gameObject.transform.parent.GetComponent<Blue> ().detourReady ();
				}catch{
					return;
				}
			} else if (col.gameObject.tag == ("Player")) {
				Debug.Log (gameObject.transform.parent+" & "+col.gameObject);
				gameObject.transform.parent.GetComponent<Blue> ().frontAlly = col.gameObject;
				col.gameObject.GetComponent<Blue> ().behindAlly.Add(gameObject.transform.parent.gameObject);
			}
		} else if (gameObject.transform.parent.tag == "Enemy") {
			if (col.gameObject.tag == ("StopEnemy")) {
				try {
					gameObject.transform.parent.GetComponent<Red> ().frontAlly = col.gameObject;
					gameObject.transform.parent.GetComponent<Red> ().detourReady ();
				} catch {
					return;
				}
			} else if (col.gameObject.tag == ("Enemy")) {
				gameObject.transform.parent.GetComponent<Red> ().frontAlly = col.gameObject;
				col.gameObject.GetComponent<Red> ().behindAlly.Add(gameObject.transform.parent.gameObject);
			}
		}
	}
	void OnTriggerExit(Collider col){
		if (col.gameObject.layer == gameObject.transform.parent.gameObject.layer) {
			try{
				col.gameObject.GetComponent<Red>().behindAlly.Remove(gameObject.transform.parent.gameObject);
			}catch{
				col.gameObject.GetComponent<Blue> ().behindAlly.Remove (gameObject.transform.parent.gameObject);
			}
		}
	}
}
