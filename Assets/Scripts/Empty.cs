using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Empty : MonoBehaviour {
	public static List<GameObject> allys = new List<GameObject> ();

	// Update is called once per frame
	void Update () {
		Vector3 ave = new Vector3 (0, 0, 0);
		allys.ForEach (i => ave = i.transform.position+ave);
		ave = ave/allys.Count;
		gameObject.transform.position = new Vector3( ave.x,3,ave.z);
	}
}
