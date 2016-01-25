using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Empty : MonoBehaviour {
	public static List<GameObject> emptys = new List<GameObject>();
	public List<GameObject> allys = new List<GameObject>();

	// Update is called once per frame
	void Update () {
		if(allys.Count == 0){
			emptys.Remove (gameObject);
			gameObject.GetComponent<line> ().enable = false;
			Destroy (gameObject);
			return;
		}
		Vector3 ave = new Vector3 (0, 0, 0);
		allys.ForEach (i => ave = i.transform.position + ave);
		ave = ave / allys.Count;
		gameObject.transform.position = new Vector3 (ave.x, 3, ave.z);
	}

	/// <summary>
	/// 渡されたオブジェクトが自分とかぶっている場合に自分のリストから削除
	/// </summary>
	/// <param name="list">List.</param>
	public void listSerch(List<GameObject> list){
		HashSet<GameObject> hashAllys = new HashSet<GameObject> ();
		allys.ForEach (i => hashAllys.Add(i));
		foreach(GameObject ob in list){
			if (hashAllys.Contains (ob)) {
				allys.Remove (ob);
			}
		}
	}
}
