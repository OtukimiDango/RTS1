using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UIHP : MonoBehaviour {
	public Camera camera;
	public static List<Transform> targets = new List<Transform>();
	void start(){
	}

	void Update () 
	{
		if(targets == null )
			return;
		for (int i = 0; i < targets.Count; i++) {
			targets [i] = GameObject.Find (targets [i].name).transform;
		}
		foreach (Transform tgt in targets) {
			GameObject hp = GameObject.Find (tgt.name +"hp(Clone)");
			Debug.Log (hp);
			Vector3 setPos = new Vector3 (tgt.position.x, tgt.position.y + tgt.localScale.y, tgt.position.z);
			hp.transform.position = camera.WorldToScreenPoint (setPos);
		}
	}
}