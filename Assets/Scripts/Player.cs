using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	private static GameObject Playercamera;
	private static GameObject playerc;
	private static Vector3 cameraPos;
	public static string mouseState;
	public static GameObject saveChara;
	public LayerMask mask;
	public GameObject writeobject;
	public static GameObject rayobj;
	public static bool rayMouse = false;
	void Start ()
	{
		rayobj = GameObject.Find ("RedSoldier1");
		saveChara = GameObject.Find ("RedSoldier1");
		mouseState = "normal";
		Playercamera = GameObject.Find ("Camera");
		playerc = GameObject.Find ("Main Camera");

		cameraPos = Playercamera.transform.position;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0) && !rayMouse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask.value)) {
				clickCharacter (hit.collider.gameObject);
				writeobject = hit.collider.gameObject;
			}
		} else if(Input.GetMouseButtonDown(0)&&rayMouse){
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask.value)) {
				if (hit.collider.gameObject.transform.parent.gameObject == saveChara) {
					clickCharacter (saveChara.transform.FindChild ("TouchCol").gameObject);
				} else if(hit.collider.gameObject.transform.parent.gameObject.layer==9&&saveChara.layer==10
					||hit.collider.gameObject.transform.parent.gameObject.layer==10&&saveChara.layer==9) {
					if (saveChara.layer==10) {
						foreach (GameObject obj in saveChara.GetComponent<Blue>().atEnemys) {
							obj.GetComponent<Light> ().color = Color.red;
						}
					}else if (saveChara.layer==9) {
						foreach (GameObject obj in saveChara.GetComponent<Red>().atEnemys) {
							obj.GetComponent<Light> ().color = Color.blue;
						}
					}
					saveChara.SendMessage ("changeAttack", hit.collider.gameObject.transform.parent.gameObject);
					hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;
					playerc.GetComponent<LineRenderer> ().enabled = false;
					rayMouse = false;
				}
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
		if (rayMouse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

				if (hit.collider.gameObject != rayobj && rayobj != null) {
					rayobj.GetComponent<Light> ().enabled = false;
				}
				playerc.GetComponent<LineRenderer> ().SetPosition (0, writeobject.transform.position);
				playerc.GetComponent<LineRenderer> ().SetPosition (1,hit.point);
		}
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask.value)) {
				if(saveChara.layer==10&& hit.collider.gameObject.transform.parent.gameObject.layer==9
					||saveChara.layer==9&& hit.collider.gameObject.transform.parent.gameObject.layer==10){

					hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().enabled = true;
					hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;
					rayobj = hit.collider.gameObject.transform.parent.gameObject;
				}
			}
	}
	}

	private static void OnGUI (Vector3 position)
	{

	}
	private static void clickCharacter (GameObject clickChara)
	{
		
		GameObject parent = clickChara.transform.parent.gameObject;
		if (parent.gameObject.layer==10 ||parent.gameObject.layer==9) { 
			bool lineFlag = parent.GetComponent<LineRenderer> ().enabled;//クリックしたオブジェクトのラインのbool
			playerc.GetComponent<LineRenderer>().enabled = !lineFlag;
			rayMouse = !lineFlag;
			if (saveChara != null) {//前回クリックしたキャラがいれば
				saveChara.GetComponent<LineRenderer> ().enabled = false;//前回のキャラのラインを消す
				saveChara.GetComponent<Light> ().enabled = false;//前回のキャラのライトを消す
				if (saveChara.layer==9) {//前回のキャラがエネミーであれば
					saveChara.GetComponent<Red> ().lightup = false;//ライトを消して
					//saveChara.GetComponent<Red>().tgt.GetComponent<Light>().enabled =
					foreach (GameObject saveatEnemys in saveChara.GetComponent<Red>().atEnemys) {//エネミーに注目する敵(自分)
						saveatEnemys.GetComponent<Light> ().enabled = false;//ライトを消す
					}
				} else if(saveChara.layer==10){
					saveChara.GetComponent<Blue> ().lightup = false;
					foreach (GameObject saveatEnemys in saveChara.GetComponent<Blue>().atEnemys) {
						saveatEnemys.GetComponent<Light> ().enabled = false;
					}
				}
			}
				
			parent.GetComponent<LineRenderer> ().enabled = !lineFlag;
			parent.GetComponent<Light> ().enabled = !lineFlag;

			if (parent.layer==10) {
				parent.GetComponent<Blue> ().lightup = !lineFlag;
				parent.GetComponent<Light> ().color = Color.blue;

			}
			if (parent.layer==9) {
				parent.GetComponent<Red> ().lightup = !lineFlag;
				parent.GetComponent<Light> ().color = Color.red;
			}
			saveChara = parent;
		}
	}
	public static void charaDestroy(GameObject chara){
		if (chara != saveChara)
			return;
		playerc.GetComponent<LineRenderer> ().enabled = false;
		rayMouse = false;
		if (chara.layer==9) {
			foreach (GameObject lightDown in chara.GetComponent<Red>().atEnemys) {
				lightDown.GetComponent<Light> ().enabled = false;
			}
		} else {
			foreach (GameObject lightDown in chara.GetComponent<Blue>().atEnemys) {
				lightDown.GetComponent<Light> ().enabled = false;;
			}
		}
	}
}