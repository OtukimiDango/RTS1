using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeCol : MonoBehaviour {

	void OnTriggerEnter (Collider col)
	{
		if (gameObject.transform.parent.CompareTag("Player") && col.gameObject.layer == 8) {
			if(col.gameObject.transform.parent.CompareTag("StopPlayer")){
				Debug.Log(gameObject+" &&"+col.gameObject);
				gameObject.transform.parent.GetComponent<Blue> ().frontAlly = col.gameObject;
				gameObject.transform.parent.GetComponent<Blue> ().detourReady ();
			} else if (col.gameObject.transform.parent.CompareTag("Player")) {
				Debug.Log (gameObject.transform.parent+" & "+col.gameObject);
				gameObject.transform.parent.GetComponent<Blue> ().frontAlly = col.gameObject;
				col.gameObject.transform.parent.GetComponent<Blue> ().behindAlly.Add(gameObject.transform.parent.gameObject);
			}
		} else if (gameObject.transform.parent.CompareTag("Enemy") && col.gameObject.layer == 8) {
			if (col.gameObject.transform.parent.CompareTag("StopEnemy")) {
				
					gameObject.transform.parent.GetComponent<Red> ().frontAlly = col.gameObject;
					gameObject.transform.parent.GetComponent<Red> ().detourReady ();
				
			} else if (col.gameObject.transform.parent.CompareTag("Enemy")) {
				gameObject.transform.parent.GetComponent<Red> ().frontAlly = col.gameObject;
				col.gameObject.transform.parent.GetComponent<Red> ().behindAlly.Add(gameObject.transform.parent.gameObject);
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
