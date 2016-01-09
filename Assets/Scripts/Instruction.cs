using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{
	private static GameObject Playercamera;
	public static string mouseState;
	public static GameObject saveChara;
	public Vector3 squadPos1;


	public static GameObject rayobj;
	public static bool rayMouse = false;

	void Start ()
	{
		rayobj = GameObject.Find ("summonRed");
		saveChara = GameObject.Find ("RedSoldier0");
		mouseState = "normal";
		Playercamera = gameObject.transform.parent.gameObject;

	}
	// Update is called once per frame
	void Update ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//マウスのポジションにrayを飛ばす
		RaycastHit hit;

		if (Input.GetMouseButtonDown (0) && !rayMouse) {
			
			int layermask = (1 << LayerMask.NameToLayer ("Terrain"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layermask)) {
				squadPos1 = hit.point;
				serchBlock.hitAlly.Clear ();
				Instantiate ((GameObject)Resources.Load ("SerchBlock"), hit.point, Quaternion.identity);
			}
		}
		if (Input.GetMouseButtonUp (0) && !rayMouse) {//クリックしたときLine展開中でなければ
			
			int layermask = (1 << LayerMask.NameToLayer ("Terrain"));

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layermask)) {
				if (Mathf.Abs (hit.point.x - squadPos1.x) >= 5f && serchBlock.hitAlly.Count != 0 || Mathf.Abs (hit.point.z - squadPos1.z) >= 5f && serchBlock.hitAlly.Count != 0) {
					gameObject.GetComponent<LineRenderer> ().enabled = true;
					rayMouse = true;
					Empty.allys = serchBlock.hitAlly;
					GameObject ob = new GameObject ("Empty");
					ob.AddComponent<Empty> ();
					StartCoroutine (firstRay (true, ob));
					serchBlock.hitAlly.ForEach (i => i.GetComponent<Light> ().enabled = true);//対象全てを光らせる。LightComponent仕様変更時に要変更
					Destroy (GameObject.Find ("SerchBlock(Clone)"));

				} else {
					Destroy (GameObject.Find ("SerchBlock(Clone)"));
				}
			}
			int mask = (1 << LayerMask.NameToLayer ("touchChara"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask)) {//Rayがキャラクターに当たると
				clickCharacter (hit.collider.gameObject);//Method実
			}
		} else if (Input.GetMouseButtonUp (0) && rayMouse) {

			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {//rayがあたった先がキャラクターでなければ
				try {
					if (hit.collider.gameObject.layer != 9
					    && hit.collider.gameObject.layer != 10
					    && hit.collider.gameObject.tag != "charabody")
						clickCharacter (saveChara.transform.FindChild ("TouchCol").gameObject);
				} catch {
					foreach (GameObject i in serchBlock.hitAlly) {
						i.GetComponent<Light> ().enabled = false;
						gameObject.GetComponent<LineRenderer> ().enabled = false;
						rayMouse = false;
					}
					Destroy (GameObject.Find ("Empty"));
				}
			}
			int mask = (1 << LayerMask.NameToLayer ("touchChara"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask)) {
				if (hit.collider.gameObject.transform.parent.gameObject == saveChara)
					clickCharacter (saveChara.transform.FindChild ("TouchCol").gameObject);
				else if (hit.collider.gameObject.transform.parent.gameObject.layer == 9 && saveChara.layer == 10
				         || hit.collider.gameObject.transform.parent.gameObject.layer == 10 && saveChara.layer == 9) {
					GameObject cSerch;
						cSerch = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Soldier> ().tgt;


					////////////////////////
						foreach (GameObject obj in saveChara.GetComponent<Soldier>().atEnemys)
							obj.GetComponent<Light> ().color = Color.red;
						if (hit.collider.gameObject.transform.parent.gameObject != saveChara.GetComponent<Soldier> ().tgt) {
							saveChara.SendMessage ("changeAttack", hit.collider.gameObject.transform.parent.gameObject);
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;
						} else if (cSerch == saveChara.GetComponent<Soldier> ().tgt)
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.blue;
					////////////////////////////
					gameObject.GetComponent<LineRenderer> ().enabled = false;
					rayMouse = false;
				}
			}
		}
		if (Input.GetMouseButton (0)) {
			if (!rayMouse) {

				GameObject cube = GameObject.Find ("SerchBlock(Clone)");

				int layermask = (1 << LayerMask.NameToLayer ("Terrain"));

				if (Physics.Raycast (ray, out hit, Mathf.Infinity, layermask)) {
					cube.transform.position = new Vector3 (((Mathf.Abs (hit.point.x) - Mathf.Abs (squadPos1.x)) / 2f) + squadPos1.x, 3f, ((Mathf.Abs (hit.point.z) - Mathf.Abs (squadPos1.z)) / 2f) + squadPos1.z);
					cube.transform.localScale = new Vector3 (Mathf.Abs (hit.point.x) - Mathf.Abs (squadPos1.x), 1f, Mathf.Abs (hit.point.z) - Mathf.Abs (squadPos1.z));
				}

			}
		}

		if (Input.GetKey ("d")) {
			Playercamera.transform.position = new Vector3 (Playercamera.transform.position.x, Playercamera.transform.position.y, Playercamera.transform.position.z + Time.deltaTime * 50);
		}
		if (Input.GetKey ("a")) {
			Playercamera.transform.position = new Vector3 (Playercamera.transform.position.x, Playercamera.transform.position.y, Playercamera.transform.position.z - Time.deltaTime * 50);
		}

	}

	public IEnumerator firstRay (bool area, GameObject lineObj)
	{
		while (rayMouse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//毎フレーム画面にRayを飛ばす
			RaycastHit hit;
			int layermask = (1 << LayerMask.NameToLayer ("Terrain"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layermask)) {//Rayがヒットしたら
				if (hit.collider.gameObject != rayobj && rayobj != null) //ヒットしたオブジェクトが前回のオブジェクと出なければ　
				rayobj.GetComponent<Light> ().enabled = false;//前回ヒットしたオブジェクトのライトをオフにする

				var pos = new Vector3 (hit.point.x, 3, hit.point.z);
				gameObject.GetComponent<LineRenderer> ().SetPosition (0, lineObj.transform.position);//Lineを飛ばす地点１
				gameObject.GetComponent<LineRenderer> ().SetPosition (1, pos);//Lineを飛ばす地点２

			}
			int mask = (1 << LayerMask.NameToLayer ("touchChara"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask) && area == false) {//rayがキャラクターに当たる
				try {
					if (saveChara.layer == 10 && hit.collider.gameObject.transform.parent.gameObject.layer == 9
					    || saveChara.layer == 9 && hit.collider.gameObject.transform.parent.gameObject.layer == 10) {
						//rayが当たったキャラと以前当たったキャラが敵対していたら
						GameObject cSerch;//rayが当たっとキャラクターのターゲット
	
							cSerch = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Soldier> ().tgt;

						if (cSerch != saveChara) {
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().enabled = true;//当たったキャラのライトをON
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;//黄色く光らせる
							rayobj = hit.collider.gameObject.transform.parent.gameObject;//rayMouse内での判定用に保存
						}
					}
				} catch {
				}
			}
			yield return null;
		}
	}

	private void clickCharacter (GameObject clickChara)
	{
		GameObject parent = clickChara.transform.parent.gameObject;//クリックしたオブジェクトの親オブジェクト
		if (parent.gameObject.layer == 10 || parent.gameObject.layer == 9) { //上記オブジェクトがキャラクターであるならば
			bool lineFlag = parent.GetComponent<LineRenderer> ().enabled;//クリックしたオブジェクトのラインのbool
			gameObject.GetComponent<LineRenderer> ().enabled = !lineFlag;//上記変数を反転した値をenableに代入
			rayMouse = !lineFlag;//上記同様
			if (rayMouse) {
				StartCoroutine (firstRay (false, clickChara));
			}
			if (saveChara != null) {//前回クリックしたキャラがいれば
				saveChara.GetComponent<LineRenderer> ().enabled = false;//前回のキャラのラインを消す
				saveChara.GetComponent<Light> ().enabled = false;//前回のキャラのライトを消す

					try {
						saveChara.GetComponent<Soldier> ().lightup = false;
						saveChara.GetComponent<Soldier> ().tgt.GetComponent<Light> ().enabled = false;
					} catch {
					}
					saveChara.GetComponent<Soldier> ().atEnemys.ForEach (i => i.GetComponent<Light> ().enabled = false);
			}

			parent.GetComponent<LineRenderer> ().enabled = !lineFlag;
			parent.GetComponent<Light> ().enabled = !lineFlag;

				parent.GetComponent<Soldier> ().lightup = !lineFlag;
				parent.GetComponent<Light> ().color = Color.blue;
				if (parent.GetComponent<Soldier> ().tgt != null && parent.GetComponent<Soldier> ().tgt != GameObject.Find ("summonRed")) {
					try {
						parent.GetComponent<Soldier> ().tgt.GetComponent<Light> ().enabled = !lineFlag;
						if (parent.GetComponent<Soldier> ().tgt == parent.GetComponent<Soldier> ().attackObj)
							parent.GetComponent<Soldier> ().tgt.GetComponent<Light> ().color = Color.blue;
					} catch {
					}
				}
					
			saveChara = parent;
		}
	}

	public static void charaDestroy (GameObject chara)
	{
		if (chara != saveChara)
			return;

		Playercamera.transform.FindChild ("Main Camera").gameObject.GetComponent<LineRenderer> ().enabled = false;
		rayMouse = false;

			chara.GetComponent<Soldier> ().atEnemys.ForEach (i => i.GetComponent<Light> ().enabled = false);
	}
}