using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	private static GameObject Playercamera;
	private static Vector3 cameraPos;
	public static string mouseState;
	public static GameObject saveChara;
	public LayerMask mask;
	// Use this for initialization
	void Start ()
	{
		saveChara = GameObject.Find ("RedSoldier1");
		//Debug.Log (saveChara.name);
		mouseState = "normal";
		Playercamera = GameObject.Find ("Camera");
		cameraPos = Playercamera.transform.position;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0) && mouseState == "normal") {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask.value)) {
				clickCharacter (hit.collider.gameObject);
			}
		}
		if (Input.GetKey ("d")) {
			cameraPos.z += Time.deltaTime * 50;
			Playercamera.transform.position = cameraPos;
		}
		if (Input.GetKey ("a")) {
			cameraPos.z -= Time.deltaTime * 50;
			Playercamera.transform.position = cameraPos;
		}
	}

	private static void OnGUI (Vector3 position)
	{

	}
	private static void clickCharacter (GameObject clickChara)
	{
		
		GameObject parent = clickChara.transform.parent.gameObject;
		if (parent.tag == "Player" || parent.tag == "StopPlayer" || parent.tag == "Enemy" || parent.tag == "StopEnemy") { 
			bool lineFlag = parent.GetComponent<LineRenderer> ().enabled;
			if (saveChara != null) {
				saveChara.GetComponent<LineRenderer> ().enabled = false;
				saveChara.GetComponent<Light> ().enabled = false;
				if (saveChara.tag == "Enemy" || saveChara.tag == "StopEnemy") {
					saveChara.GetComponent<Red> ().lightup = false;
					foreach (GameObject saveatEnemys in saveChara.GetComponent<Red>().atEnemys) {
						saveatEnemys.GetComponent<Light> ().enabled = false;
					}
				} else if(saveChara.tag == "Player"||saveChara.tag=="StopPlayer"){
					saveChara.GetComponent<Blue> ().lightup = false;
					foreach (GameObject saveatEnemys in saveChara.GetComponent<Blue>().atEnemys) {
						saveatEnemys.GetComponent<Light> ().enabled = false;
					}
				}
			}
				
			parent.GetComponent<LineRenderer> ().enabled = !lineFlag;
			parent.GetComponent<Light> ().enabled = !lineFlag;

			if (parent.tag == "Player" || parent.tag == "StopPlayer") {
				parent.GetComponent<Blue> ().lightup = !lineFlag;
				parent.GetComponent<Light> ().color = Color.blue;
//				foreach (GameObject lightup in parent.GetComponent<Blue>().atEnemys) {
//					lightup.GetComponent<Light> ().enabled = false;
//				}
			}
			if (parent.tag == "Enemy" || parent.tag == "StopEnemy") {
				parent.GetComponent<Red> ().lightup = !lineFlag;
				parent.GetComponent<Light> ().color = Color.red;
				Debug.Log (parent.GetComponent<Light> ().color);

//				foreach (GameObject lightup in parent.GetComponent<Red>().atEnemys) {
//					lightup.GetComponent<Light> ().enabled = !lineFlag;
//				}
			}
			saveChara = parent;
		}
	}
	public static void charaDestroy(GameObject chara){
		if (chara != saveChara)
			return;
		if (chara.tag == "Enemy" || chara.tag == "StopEnemy") {
			foreach (GameObject lightDown in chara.GetComponent<Red>().atEnemys) {
				lightDown.GetComponent<Light> ().enabled = false;
			}
		} else {
			foreach (GameObject lightDown in chara.GetComponent<Blue>().atEnemys) {
				lightDown.GetComponent<Light> ().enabled = false;
			}
		}
	}
}