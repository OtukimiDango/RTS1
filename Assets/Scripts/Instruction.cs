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
		//saveChara = GameObject.Find ("RedWarrior0");
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
				Instantiate ((GameObject)Resources.Load ("SerchBlock"), hit.point, Quaternion.identity);
			}
		}
		if (Input.GetMouseButtonUp (0) && !rayMouse) {//クリックしたときLine展開中でなければ

			int terrain = (1 << LayerMask.NameToLayer ("Terrain"));

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, terrain)) {
				List<GameObject> list =  GameObject.Find("SerchBlock(Clone)").GetComponent<serchBlock>().hitAlly;
				if (Mathf.Abs (hit.point.x - squadPos1.x) >= 5f &&list.Count != 0 || Mathf.Abs (hit.point.z - squadPos1.z) >= 5f && list.Count != 0) {
					gameObject.GetComponent<LineRenderer> ().enabled = true;
					rayMouse = true;
					GameObject ob = new GameObject ("Empty");
					ob.AddComponent<Empty> ();
					ob.GetComponent<Empty>().allys = GameObject.Find("SerchBlock(Clone)").GetComponent<serchBlock>().hitAlly;
					clickCharacter (ob);
					StartCoroutine (firstLine (true, ob));
					list.ForEach (i => i.GetComponent<Light> ().enabled = true);//対象全てを光らせる。LightComponent仕様変更時に要変更
					Destroy (GameObject.Find ("SerchBlock(Clone)"));
				} else {
					Destroy (GameObject.Find ("SerchBlock(Clone)"));
				}
			}
			int chara = (1 << LayerMask.NameToLayer ("touchChara"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, chara)) {//Rayがキャラクターに当たると
				clickCharacter (hit.collider.gameObject.transform.parent.gameObject);//Method実
			}
		} else if (Input.GetMouseButtonUp (0) && rayMouse) {//クリックした時にLINE展開中であれば
			int terrain = (1 << LayerMask.NameToLayer ("Terrain"));
			int chara = (1 << LayerMask.NameToLayer ("touchChara"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, chara)) {
				if (hit.collider.gameObject.transform.parent.gameObject == saveChara) {
					clickCharacter (saveChara);
				} else if (hit.collider.gameObject.transform.parent.gameObject.layer == 9 && saveChara.layer == 10
					|| hit.collider.gameObject.transform.parent.gameObject.layer == 10 && saveChara.layer == 9) {
					GameObject Hetgt;
					Hetgt = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Soldier> ().tgt;

					////////////////////////
					foreach (GameObject obj in saveChara.GetComponent<Soldier>().atEnemys)
						obj.GetComponent<Light> ().color = Soldier.color (obj.GetComponent<Soldier> ().Name ("myTag", true));//敵対色
					if (hit.collider.gameObject.transform.parent.gameObject != saveChara.GetComponent<Soldier> ().tgt) {
						saveChara.SendMessage ("changeAttack", hit.collider.gameObject.transform.parent.gameObject);
						hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;
					} else if (Hetgt == saveChara.GetComponent<Soldier> ().tgt)
						hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Soldier.color (hit.collider.transform.parent.GetComponent<Soldier> ().Name ("myTag", true));//戦闘色
					////////////////////////////
					gameObject.GetComponent<LineRenderer> ().enabled = false;
					rayMouse = false;
				}
				return;
			}
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, terrain)) {//rayがあたった先がキャラクターでなけれ
				try {
					Vector3 pos = GameObject.Find ("Empty").transform.position;//emptyがない場合エラー
					Debug.Log(pos);
					float ang = Mathf.Atan2 (hit.point.x - pos.x, hit.point.z - pos.z) * Mathf.Rad2Deg;
					Vector3 dis = Soldier.distance (hit.point, pos);
					foreach (GameObject ob in GameObject.Find("Empty").GetComponent<Empty>().allys) {
						Soldier script = ob.GetComponent<Soldier> ();
						ob.transform.rotation = GameObject.Find ("Empty").transform.rotation;
						ob.transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, ang, 0), 1);
						script.StopAllCoroutines ();
						script.StartCoroutine (script.move (dis, false));

					}
					GameObject em = GameObject.Find("Empty");
					em.AddComponent<LineRenderer>();
					em.GetComponent<Empty>().StartCoroutine(em.GetComponent<Empty>().line(hit.point));
					foreach (GameObject i in GameObject.Find("moveEmpty").GetComponent<Empty>().allys) {
						i.GetComponent<Light> ().enabled = false;
					}
					gameObject.GetComponent<LineRenderer> ().enabled = false;
					rayMouse = false;

				} catch {
					clickCharacter (saveChara);
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

	public IEnumerator firstLine (bool area, GameObject lineObj)
	{
		while (rayMouse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//毎フレーム画面にRayを飛ばす
			RaycastHit hit;
			int layermask = (1 << LayerMask.NameToLayer ("Terrain"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layermask)) {//Rayがヒットしたら
				if (hit.collider.gameObject != rayobj && rayobj != null){ //ヒットしたオブジェクトが前回のオブジェクと出なければ　
					rayobj.GetComponent<Light> ().enabled = false;//前回ヒットしたオブジェクトのライトをオフにする
				}

				var pos = new Vector3 (hit.point.x, 0, hit.point.z);
				gameObject.GetComponent<LineRenderer> ().SetPosition (0, lineObj.transform.position);//Lineを飛ばす地点１
				gameObject.GetComponent<LineRenderer> ().SetPosition (1, pos);//Lineを飛ばす地点２

			}
			int mask = (1 << LayerMask.NameToLayer ("touchChara"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask) && area == false) {//rayがキャラクターに当たる
				try {
					if (saveChara.layer == 10 && hit.collider.gameObject.transform.parent.gameObject.layer == 9
					    || saveChara.layer == 9 && hit.collider.gameObject.transform.parent.gameObject.layer == 10) {
						//rayが当たったキャラと以前当たったキャラが敵対していたら
						GameObject Hetgt;//rayが当たっとキャラクターのターゲット

						Hetgt = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Soldier> ().tgt;

						if (Hetgt != saveChara) {
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

	private void clickCharacter (GameObject chara)
	{
		if (chara.name == "Empty"||chara.name == "moveEmpty") {
			saveChara = chara;
			return;
		}
		Soldier script = chara.GetComponent<Soldier> ();
		//例外処理
		bool lineFlag = chara.GetComponent<LineRenderer> ().enabled;//クリックしたオブジェクトのラインのbool
		gameObject.GetComponent<LineRenderer> ().enabled = !lineFlag;//上記変数を反転した値をenableに代入
		rayMouse = !lineFlag;//反転した値をenableに代入
		if (rayMouse && script.Name ("myTag", false) == "Player") {//クリックしたのがプレイヤー側なら
			StartCoroutine (firstLine (false, chara));//tgt指定Line表示
		}

		if (saveChara != null /*&& saveChara.name != "Empty"*/) {//前回クリックしたキャラがいれば
			Soldier S_script = saveChara.GetComponent<Soldier> ();
			saveChara.GetComponent<LineRenderer> ().enabled = false;//前回のキャラのラインを消す
			saveChara.GetComponent<Light> ().enabled = false;//前回のキャラのライトを消す

			try {
				S_script.lightup = false;
				S_script.tgt.GetComponent<Light> ().enabled = false;
			} catch {
			}
			S_script.atEnemys.ForEach (i => i.GetComponent<Light> ().enabled = false);
		}
		//addcomponent
		chara.GetComponent<LineRenderer> ().enabled = !lineFlag;
		chara.GetComponent<Light> ().enabled = !lineFlag;
		script.lightup = !lineFlag;
		chara.GetComponent<Light> ().color = Soldier.color (script.Name ("myTag", false));

		if (script.tgt != null && script.tgt != GameObject.Find (script.Name ("tgtName", false))) {
			try {
				rayobj = null;
				script.tgt.GetComponent<Light> ().enabled = !lineFlag;
				if (script.tgt == script.attackObj)
					script.tgt.GetComponent<Light> ().color = Soldier.color (script.Name ("myTag", false));
			} catch {
			}
		}
		saveChara = chara;
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