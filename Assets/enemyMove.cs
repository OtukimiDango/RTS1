using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyMove : MonoBehaviour
{
	private GameObject tgt;
	private Vector3 tgtDis;

	public short HP = 200;
	private Vector3 myPos;
	private Vector3 savePos;
	private float detourDis;
	private static byte speed = 15;
	public string state = "move";

	//private bool measureSpace = true;//gamemanagerを作りstaticにする
	public GameObject frontAlly = null;
	//public static List<GameObject> red = new List<GameObject> ();
	public static List<GameObject> allys = new List<GameObject> ();
	public List<GameObject> atEnemys = new List<GameObject> ();
	//private static float frontdis = 7;
	private GameObject saveFrontAlly;

	private bool right = true;

	public PlayerMove script;

	private bool attackSpace = true;

	public static byte count = 0;

	void Start ()
	{
		//red.Add (gameObject);
		count++;
		gameObject.name = "BlueSoldier" + count;
		right = Random.value > 0.5f ? true : false;
		tgt = GameObject.Find ("summonRed");
		myPos = transform.position;
		tgtDis = distance (tgt.transform.position, myPos);
	}

	// Update is called once per frame
	void Update ()
	{
		//Debug.Log (gameObject+" == "+state);
		switch (state) {
		case "move":
			myPos.z = myPos.z + (tgtDis.z * Time.deltaTime) / speed;

			if (myPos.x >= 1 || myPos.x <= -1) {
				myPos.x = myPos.x - (tgtDis.x * Time.deltaTime) / (speed * (myPos.z / tgtDis.z));
			}
			transform.position = myPos;
//			if (measureSpace) {
//				StartCoroutine (measure ());
//			}
			break;
		case "detour":
//			if (measureSpace) {
//				StartCoroutine (measure ());
//			}
			detour ();
			break;
		case "fight":
			if (attackSpace) {
				StartCoroutine (attack ()); 
			}
			break;
		default :
			break;
		}
		if (HP <= 0) {
			death ();
		}

	}

	void OnCollisionEnter (Collision col)
	{
		switch (col.gameObject.tag) {
		case "Player":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<PlayerMove> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "StopPlayer":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<PlayerMove> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "summonRed":
			state = "fight";
			//this.tag = "PlayerStop";
			break;
		default :
			break;
		}
	}

//	private IEnumerator measure ()
//	{
//		measureSpace = false;
//		//allys.Clear();
//		//allys.AddRange (GameObject.FindGameObjectsWithTag ("StopEnemy"));
////		allys.Remove (gameObject);
////		foreach (Transform child in transform) {
////			allys.Remove (child.gameObject);
////		}
//		foreach (GameObject posObj in allys) {
//			Vector3 allyDis = distance (posObj.transform.position, myPos);
//			float myScaleX = gameObject.transform.localScale.x;
//			float myScaleZ = gameObject.transform.localScale.z;
//			float allyScaleZ = posObj.transform.localScale.z;
//			if (allyDis.x <= myScaleX / 2
//			    && allyDis.x >= -myScaleX / 2
//				&& allyDis.z - ((myScaleZ + allyScaleZ) / 2) >= -frontdis
//			    && allyDis.z <= 0) {
//				frontdis = allyDis.z;
//				frontAlly = posObj;
//				detourReady ();
//			}
//		}
//		yield return new WaitForSeconds (0.2f);
//		measureSpace = true;
//	}

	public void detourReady ()
	{
		if (saveFrontAlly != frontAlly) { 
			saveFrontAlly = frontAlly;
			detourDis = right ? frontAlly.transform.localScale.x : -frontAlly.transform.localScale.x;
			state = "detour";
			savePos = myPos;
		}
	}

	private void detour ()
	{
		//		myPos.x -= Mathf.Cos (detourTime * 3) *(detourTarget.transform.localScale.x * 3 / 10);
		//		myPos.z += Mathf.Sin(detourTime*1.1f)*(1 + ((detourTarget.transform.localScale.z/10)*1.5f));
		//		myPos.x -= Mathf.Cos (detourTime*2) *(detourTarget.transform.localScale.x/10);
		//		myPos.z += Mathf.Sin(0.21f)*(1 + ((detourTarget.transform.localScale.z/10)*1.5f));
		//		transform.position = myPos;
		if (detourDis > 0) {
			if (myPos.x >= savePos.x + detourDis) {
				state = "move";
				tgtDis.x = myPos.x;
			} else {
				myPos.x += (detourDis * (Time.deltaTime * 3));
				transform.position = myPos;
			}
		} else {
			if (myPos.x <= savePos.x + detourDis) {
				state = "move";
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

	public  static Vector3 distance (Vector3 target, Vector3 me)
	{
		Vector3 dis = target - me;
		return dis;
	}

	private IEnumerator attack ()
	{
		script.HP -= 30;
		attackSpace = false;
		yield return new WaitForSeconds (3);
		attackSpace = true;
	}

	private void death ()
	{
		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<PlayerMove> ();
			script.state = "move";
			enemy.tag = "Player";
		}
		summonsServant.sp += 10;
		Destroy (this.gameObject);

	}
}
