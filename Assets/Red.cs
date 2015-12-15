using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Red : MonoBehaviour
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

	public Blue script;

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
		HP = 200;
		state = "move";
//		Thread threadA = new Thread(new ThreadStart(measure));
//		threadA.IsBackground = true;
//		threadA.Start ();
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
			break;
		case "detour":
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
				script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					state = "fight";
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
		//		myPos.x -= Mathf.Cos (detourTime * 3) *(detourTarget.transform.localScale.x * 3 / 10);
		//		myPos.z += Mathf.Sin(detourTime*1.1f)*(1 + ((detourTarget.transform.localScale.z/10)*1.5f));
		//		myPos.x -= Mathf.Cos (detourTime*2) *(detourTarget.transform.localScale.x/10);
		//		myPos.z += Mathf.Sin(0.21f)*(1 + ((detourTarget.transform.localScale.z/10)*1.5f));
		//		transform.position = myPos;
		if (detourDis > 0) {
			if (myPos.x >= savePos.x + detourDis) {
				state = "move";
				saveFrontAlly = null;
				tgtDis.x = myPos.x;
			} else {
				myPos.x += (detourDis * (Time.deltaTime * 3));
				transform.position = myPos;
			}
		} else {
			if (myPos.x <= savePos.x + detourDis) {
				saveFrontAlly = null;
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
		yield return new WaitForSeconds (2.5f);
		attackSpace = true;
	}

	private void death ()
	{
		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<Blue> ();
			script.state = "move";
			script.atEnemys.Remove (gameObject);
			enemy.tag = "Player";
		}
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}
