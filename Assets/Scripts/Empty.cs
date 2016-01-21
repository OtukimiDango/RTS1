﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Empty : MonoBehaviour {
	public static List<GameObject> emptys = new List<GameObject>();
	public List<GameObject> allys = new List<GameObject>();

	// Update is called once per frame
	void Update () {
		if(allys.Count == 0){
			emptys.Remove (gameObject);
			Destroy (gameObject);
			return;
		}
		Vector3 ave = new Vector3 (0, 0, 0);
		allys.ForEach (i => ave = i.transform.position + ave);
		ave = ave / allys.Count;
		gameObject.transform.position = new Vector3 (ave.x, 3, ave.z);
	}
	public IEnumerator line(Vector3 point){
		try{
		foreach(GameObject ob in emptys){
			ob.GetComponent<Empty> ().listSerch (allys);
		}
		}catch{
		}
		name = "moveEmpty";
		emptys.Add (gameObject);
		Vector3 pos = transform.position;
		LineRenderer linerend = transform.GetComponent<LineRenderer> ();
		while(Mathf.Abs(pos.x-point.x) > 0.5f && Mathf.Abs(pos.z-point.z) > 0.5f){
			pos = transform.position;
			linerend.SetPosition(0,pos);//自分の座標
			linerend.SetPosition(1,point);//point座標は固定
			yield return null;
		}//目的地に着くとwhileを抜ける
		emptys.Remove (gameObject);//staticリストから自分を外す
		Destroy (gameObject);//自殺
		yield return null;
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
