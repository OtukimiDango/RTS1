using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Red : MonoBehaviour
{
	private GameObject tgt;
	public Vector3 tgtDis;

	public int HP;
	private Vector3 myPos;
	private Vector3 savePos;
	private float detourDis;
	private static int speed = 10;
	public string state;
	public GameObject frontAlly = null;
	public static List<GameObject> allys = new List<GameObject> ();
	public List<GameObject> atEnemys = new List<GameObject> ();
	private GameObject saveFrontAlly;
	private bool right;
	public Blue script;
	private bool attackSpace = true;
	private static int count = 0;
	public GameObject attackEnemy;
	public LineRenderer renderer;

	void Start ()
	{
		renderer = GetComponent<LineRenderer> ();
		tgt = GameObject.Find ("summonBlue");
		myPos = transform.position;
		count++;
		gameObject.name = "RedSoldier" + count;
		tgtDis = distance (tgt.transform.position, myPos);
		state = "move";
		HP = 200;
		right = Random.value > 0.5f ? true : false;
	}

	// Update is called once per frame
	void Update ()
	{
		renderer.SetPosition(0, transform.position);
		renderer.SetPosition(1, tgt.transform.position);
		switch (state) {
		case "move":
			myPos.z = myPos.z + (tgtDis.z * Time.deltaTime) / speed;
			if (myPos.x >= 1 || myPos.x <= -1) {
				myPos.x = myPos.x - (tgtDis.x * (Time.deltaTime / (speed * (myPos.z / tgtDis.z))));
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
		if (HP < 0) {
			death ();
		}

	}

	void OnCollisionEnter (Collision col)
	{
		switch (col.gameObject.tag) {
		case "Enemy":
			if (gameObject.CompareTag ("Player") && state != "fight") {
				script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					attackEnemy = col.gameObject;
					state = "fight";
					gameObject.tag = "StopPlayer";
				}
			}
			break;
		case "StopEnemy":
			if (gameObject.CompareTag ("Player") && state != "fight") {
				script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					attackEnemy = col.gameObject;
					state = "fight";
					gameObject.tag = "StopPlayer";
				}
			}
			break;
		case "summonBlue":
			state = "fight";
			gameObject.tag = "StopPlayer";
			Debug.Log ("yes");
			break;
		default :
			break;
		}
	}

	public void detourReady ()
	{
		if (saveFrontAlly != frontAlly) { 
			saveFrontAlly = frontAlly;
			detourDis = right ? saveFrontAlly.transform.localScale.x+3 : -(saveFrontAlly.transform.localScale.x+3);
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
			if (myPos.x >= savePos.x + detourDis +1) {
				state = "move";
				gameObject.tag="Player";
				tgtDis.x = myPos.x;

			} else {
				myPos.x += (detourDis * (Time.deltaTime * 3));
				transform.position = myPos;
			}
		} else {
			if (myPos.x <= savePos.x + detourDis+1) {
				state = "move";
				gameObject.tag="Player";
				tgtDis.x = myPos.x;
			} else {
				myPos.x += (detourDis * (Time.deltaTime * 3));
				transform.position = myPos;
			}
		}
	}

	public static Vector3 distance (Vector3 target, Vector3 me)
	{
		Vector3 dis = target - me;
		return dis;
	}

	private IEnumerator attack ()
	{
		attackEnemy.GetComponent<Blue>().HP -= 30;
		attackSpace = false;
		yield return new WaitForSeconds (3);
		attackSpace = true;
	}

	private void death ()
	{
		if (attackEnemy != null) {
			attackEnemy.GetComponent<Blue> ().atEnemys.Remove (gameObject);
		}

		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<Blue> ();
			script.atEnemys.Remove (gameObject);
			script.state = "move";
			script.attackEnemy = null;
			enemy.tag = "Enemy";
		}
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}