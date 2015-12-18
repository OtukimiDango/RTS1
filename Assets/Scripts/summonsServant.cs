﻿using UnityEngine;
using System.Collections;

public class summonsServant : MonoBehaviour {
	public GameObject Soldier;
	public GameObject Witch;

	private int servantCount  = 0;

	private bool summonSpace = true;

	public static int sp = 500;

	public static int UpSp = 1;

	private static float GameTime = 0;

	private static int soldierCount = 0;
	private static int witchCount = 0;

	private GameObject spawnPoint;
	// Use this for initialization
	void Start () {
		StartCoroutine (spUp ());
		StartCoroutine (gameTime());

		spawnPoint = GameObject.Find("spawnPoint");
	}

	// Update is called once per frame
	void Update () {
		GameTime += Time.deltaTime;
		if(Input.GetKey(KeyCode.Alpha1) && summonSpace && sp > 9 && soldierCount < 10){
			StartCoroutine(summonServent(Soldier,10));
			soldierCount++;
		}
		if(Input.GetKey(KeyCode.Alpha2) && summonSpace && sp > 19 && witchCount < 10){
			StartCoroutine(summonServent(Witch,20));
			witchCount++;
		}
	}
	private IEnumerator spUp(){
		while (true) {
			yield return new WaitForSeconds(1);
			sp += UpSp;
		}
	}
	private IEnumerator summonServent(GameObject s, int stuff){
		sp -= stuff;
		summonSpace = false;
		Vector3 summonPosition = spawnPoint.transform.position;
		summonPosition.y = summonPosition.y + s.transform.position.y;
		Object.Instantiate(s,summonPosition,spawnPoint.transform.rotation);
		servantCount++;
		yield return new WaitForSeconds(1f);
		summonSpace = true;

	}
	void OnGUI() {
		GUI.Label (new Rect (0, 0, 100, 30), "sp : "+sp);
		GUI.Label (new Rect (100, 0, 100, 30), "Servants : " + servantCount);
		GUI.Label(new Rect(200,0,100,30),"Time : "+(int)GameTime);
	}
	private static IEnumerator gameTime(){
		yield return new WaitForSeconds (30);
		summonsServant.UpSp++;

		yield return new WaitForSeconds (60);
		summonsServant.UpSp++;

		yield return new WaitForSeconds (120);
		summonsServant.UpSp++;
	}
}