using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyMove : MonoBehaviour
{
	public int HP = 200;
	private GameObject tgt;
	private Vector3 myPos;
	private Vector3 savePos;
	private float detourDis;
	private Vector3 tgtDis;
	private int speed = 10;
	public string state = "move";
	private bool measureSpace = true;
	private GameObject frontAlly = null;
	private List<GameObject> allys = new List<GameObject> ();
	public List<GameObject> atEnemys = new List<GameObject> ();
	private float frontdis = 7.0f;
	private GameObject saveFrontAlly;
	private bool right = true;
	public PlayerMove script;
	private bool attackSpace = true;

	void Start ()
	{
		right = Random.value > 0.5f ? true : false;
		tgt = GameObject.Find ("summonRed");
		myPos = transform.position;
		tgtDis = distance (tgt.transform.position, myPos);
	}

	// Update is called once per frame
	void Update ()
	{
		switch (state) {
		case "move":
			myPos.z = myPos.z + (tgtDis.z * Time.deltaTime) / speed;

			if (myPos.x >= 1 || myPos.x <= -1) {
				myPos.x = myPos.x - (tgtDis.x * Time.deltaTime) / (speed * (myPos.z / tgtDis.z));
			}
			transform.position = myPos;
			if (measureSpace) {
				StartCoroutine (measure ());
			}
			break;
		case "detour":
			if (measureSpace) {
				StartCoroutine (measure ());
			}
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
					//this.tag = "PlayerStop";
				}
			}
			break;
		case "PlayerStop":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<PlayerMove> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
					//this.tag = "PlayerStop";
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

	private IEnumerator measure ()
	{
		measureSpace = false;
		allys.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
		allys.Remove (gameObject);
		foreach (Transform child in transform) {
			allys.Remove (child.gameObject);
		}
		foreach (GameObject posObj in allys) {
			Vector3 allyDis = distance (posObj.transform.position, myPos);
			float myScaleX = gameObject.transform.localScale.x;
			float myScaleZ = gameObject.transform.localScale.z;
			float allyScaleZ = posObj.transform.localScale.z;
			Debug.Log( allyDis.z - (myScaleZ + allyScaleZ / 2));
			if (allyDis.x <= myScaleX / 2
			    && allyDis.x >= -myScaleX / 2
			    && allyDis.z - (myScaleZ + allyScaleZ / 2) >= -frontdis
			    && allyDis.z >= 0) {
				frontdis = allyDis.z;
				frontAlly = posObj;
				detourReady ();
			}
		}
		yield return new WaitForSeconds (0.2f);
		measureSpace = true;
	}

	private void detourReady ()
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

	private Vector3 distance (Vector3 target, Vector3 me)
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
		}
		//tgt.SendMessage ("victorySpUp", 2);
		summonsServant.sp += 10;
		Destroy (this.gameObject);

	}
}
