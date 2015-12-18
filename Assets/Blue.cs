using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blue : MonoBehaviour
{
	private GameObject tgt;
	private Vector3 tgtDis;

	public int HP;
	private Vector3 myPos;
	private Vector3 savePos;
	private float detourDis;
	private static byte speed = 15;
	public string state;

	public GameObject frontAlly = null;
	public static List<GameObject> allys = new List<GameObject> ();
	public List<GameObject> atEnemys = new List<GameObject> ();
	private GameObject saveFrontAlly;

	private bool right = true;

	public Red script;

	private bool attackSpace = true;

	public static byte count = 0;

	public GameObject attackEnemy;

	public LineRenderer renderer;


	void Start ()
	{
		renderer = GetComponent<LineRenderer> ();
		count++;
		gameObject.name = "BlueSoldier" + count;
		right = Random.value > 0.5f ? true : false;
		tgt = GameObject.Find ("summonRed");
		myPos = transform.position;
		tgtDis = distance (tgt.transform.position, myPos);
		HP = 200;
		state = "move";
	}

	// Update is called once per frame
	void Update ()
	{
		renderer.SetPosition(0, transform.position);
		renderer.SetPosition(1, tgt.transform.position);
		switch (state) {
		case "move":
			transform.LookAt (tgt.transform);
			myPos.z = myPos.z + (tgtDis.z * Time.deltaTime) / speed;

			if (myPos.x >= 1 || myPos.x <= -1) {
				myPos.x = myPos.x - (tgtDis.x * Time.deltaTime) / (speed * (myPos.z / tgtDis.z));
			}
			transform.position = myPos;
			break;
		case "detour":
			detour ();
			break;
		case "fight":
			transform.LookAt (attackEnemy.transform);
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

	void OnTriggerEnter (Collider col)
	{
		switch (col.gameObject.tag) {
		case "Player":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<Red> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
					attackEnemy = col.gameObject;
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "StopPlayer":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<Red> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
					attackEnemy = col.gameObject;
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
			detourDis = right ? frontAlly.transform.localScale.x : -frontAlly.transform.localScale.x;
			state = "detour";
			savePos = myPos;
		}
	}

	private void detour ()
	{
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
		attackEnemy.GetComponent<Red>().HP -= 30;
		attackSpace = false;
		yield return new WaitForSeconds (2.5f);
		attackSpace = true;
	}

	private void death ()
	{
		if (attackEnemy != null) {
			attackEnemy.GetComponent<Red> ().atEnemys.Remove (gameObject);
			if(attackEnemy.GetComponent<Red>().atEnemys.Count != 0){
				attackEnemy.GetComponent<Red> ().attackEnemy = attackEnemy.GetComponent<Red> ().atEnemys [0];
		}
		}
		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<Red> ();
			script.state = "move";
			enemy.tag = "Player";
			script.attackEnemy = null;
		}
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}