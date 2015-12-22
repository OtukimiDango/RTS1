﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Red : MonoBehaviour
{
	public GameObject tgt;
	private Vector3 tgtDis;

	public int HP;
	private Vector3 myPos;
	private Vector3 savePos;
	private float detourDis;
	private static byte speed = 10;
	public string state;

	public GameObject frontAlly = null;
	public static List<GameObject> allys = new List<GameObject> ();
	public List<GameObject> atEnemys = new List<GameObject> ();
	private GameObject saveFrontAlly;

	private bool right = true;

	public Blue script;

	private bool attackSpace = true;

	public static byte count = 0;

	public bool lightup = false;

	public LineRenderer renderer;


	void Start ()
	{
		renderer = GetComponent<LineRenderer> ();
		count++;
		gameObject.name = "RedSoldier" + count;
		right = Random.value > 0.5f ? true : false;
		tgt = GameObject.Find ("summonBlue");
		myPos = transform.position;
		tgtDis = distance (tgt.transform.position, myPos);
		tgtDis = tgtDis.normalized;
		HP = 200;
		state = "move";
	}

	// Update is called once per frame
	void Update ()
	{
		switch (state) {
		case "move":
			transform.LookAt (tgt.transform);
			myPos.z = myPos.z + (tgtDis.z * speed * Time.deltaTime);
			if (tgtDis.x - myPos.x >= 10 || tgtDis.x - myPos.x <= -10) {
				myPos.x = myPos.x + (tgtDis.x * speed * Time.deltaTime);
			}
			transform.position = myPos;
			break;
		case "detour":
			detour ();
			break;
		case "fight":
			if (tgt == null) {
				state = "move";
				break;
			}
			transform.LookAt (tgt.transform);
			if (attackSpace) {
				StartCoroutine (attack ()); 
			}
			break;
		default :
			break;
		}
		if (lightup) {
			renderer.SetPosition (0, transform.position);
			renderer.SetPosition (1, tgt.transform.position);
			if (0 == atEnemys.Count) {
				renderer.SetVertexCount (2);
				return;
			}
			for (int i = 0; i < atEnemys.Count; i++) {

				if (atEnemys [i].GetComponent<Light> ().enabled == false) {
					atEnemys [i].GetComponent<Light> ().enabled=true;
					atEnemys [i].GetComponent<Light> ().color = Color.blue;
				if (atEnemys[i].name == tgt.name) {
						atEnemys [i].GetComponent<Light> ().color = Color.red;
				}
			}
				renderer.SetVertexCount (2 + ((i + 1) * 2));
				renderer.SetPosition (2 + ((i + 1) * 2 - 2), transform.position);
				renderer.SetPosition (2 + ((i + 1) * 2 - 1), atEnemys [i].transform.position);
			}
		}
		if (HP <= 0) {
			Death ();
		}

	}

	void OnTriggerEnter (Collider col)
	{
		switch (col.gameObject.tag) {
		case "Player":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
					tgt = col.gameObject;
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "StopPlayer":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
					tgt = col.gameObject;
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "summonRed":
			state = "fight";
			gameObject.tag = "PlayerStop";
			break;
		default :
			break;
		}
	}

	public void detourReady ()
	{
		if (saveFrontAlly != frontAlly) { 
			saveFrontAlly = frontAlly;
			detourDis = right ? saveFrontAlly.transform.localScale.x + 3 : -(saveFrontAlly.transform.localScale.x + 3);
			state = "detour";
			savePos = myPos;
		}
	}

	private void detour ()
	{
		if (detourDis > 0) {
			if (myPos.x >= savePos.x + detourDis) {
				state = "move";
				gameObject.tag = ("Enemy");
				tgtDis.x = myPos.x;
			} else {
				myPos.x += (detourDis * (Time.deltaTime * 3));
				transform.position = myPos;
			}
		} else {
			if (myPos.x <= savePos.x + detourDis) {
				state = "move";
				gameObject.tag = ("Enemy");
				tgtDis.x = myPos.x;
			} else {
				myPos.x += (detourDis * (Time.deltaTime * 3));
				transform.position = myPos;
			}
		}
	}
	//		switch((int)detourDis){
	//		case (int)frontAlly.transform.localScale.x:
	//			if (myPos.x >= savePos.x + detourDis) {
	//				state = "move";
	//				tgtDis.x = myPos.x;
	//			} else {
	//				myPos.x += (detourDis * (Time.deltaTime * 3));
	//				transform.position = myPos;
	//			}
	//			break;
	//		case (int)-frontAlly.transform.localScale.x:
	//			if (myPos.x <= savePos.x + detourDis) {
	//				state = "move";
	//				tgtDis.x = myPos.x;
	//			} else {
	//				myPos.x += (detourDis * (Time.deltaTime * 3));
	//				transform.position = myPos;
	//			}
	//			break;
	//		default:
	//			break;
	//	}
	//	}
	private void changeAttak ()
	{
		if (tgt == null || atEnemys != null) {
			tgt = atEnemys [0];
		}
	}

	public  static Vector3 distance (Vector3 target, Vector3 me)
	{
		Vector3 dis = target - me;
		return dis;
	}

	private IEnumerator attack ()
	{
		tgt.GetComponent<Blue> ().HP -= 30;
		GameObject myHPBar = GameObject.Find (tgt.name + ("hp(Clone)"));
		myHPBar.transform.localScale -= new Vector3 (0.15f, 0, 0);
		attackSpace = false;
		yield return new WaitForSeconds (2.5f);
		attackSpace = true;
	}

	private void Death ()
	{
		if (tgt != null) {
			tgt.GetComponent<Blue> ().atEnemys.Remove (gameObject);
		}
		Player.charaDestroy (gameObject);

		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<Blue> ();
			if (script.atEnemys.Count != 0) {
				script.tgt = script.atEnemys [0];
			} else {
				script.state = "move";
				enemy.tag = "Player";
				script.tgt = GameObject.Find ("summonBlue");
			}
		}
		if (Player.saveChara == gameObject)
			Player.saveChara = null;
		UIHP.targets.Remove (gameObject.transform);
		Destroy (GameObject.Find (gameObject.name + "hp(Clone)"));
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}