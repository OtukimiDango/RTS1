﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{
	private static GameObject Playercamera;
	public static string mouseState;
	public static GameObject saveChara;
	public LayerMask mask;
	public LayerMask maskTerrain;
	public GameObject lineObject;
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
		if (Input.GetMouseButtonDown (0) && !rayMouse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//マウスのポジションにrayを飛ばす
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, maskTerrain.value)) {
				squadPos1 = hit.point;
				Instantiate ((GameObject)Resources.Load("SerchBlock"),hit.point,Quaternion.identity);
			}
		}
		if (Input.GetMouseButtonUp (0) && !rayMouse) {//クリックしたときLine展開中でなければ
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//マウスのポジションにrayを飛ばす
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, maskTerrain.value)) {
				if (Mathf.Abs( hit.point.x - squadPos1.x) >= 5f || Mathf.Abs(hit.point.z - squadPos1.z) >= 5f) {
					try {
						Debug.Log(serchBlock.hitAlly.Count);
						Destroy (GameObject.Find ("SerchBlock(Clone)"));
					} catch {
					}
				} else {
					Debug.Log(serchBlock.hitAlly.Count);
					Destroy (GameObject.Find ("SerchBlock(Clone)"));

				}
					
			}

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask.value)) {//Rayがキャラクターに当たると
				lineObject = hit.collider.gameObject;//オブジェクト代入

				clickCharacter (hit.collider.gameObject);//Method実
			}
		} else if (Input.GetMouseButtonUp (0) && rayMouse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {//rayがあたった先がキャラクターでなければ
				try{
					if (hit.collider.gameObject.layer != 9
						&& hit.collider.gameObject.layer != 10
						&& hit.collider.gameObject.tag != "charabody")
						clickCharacter (saveChara.transform.FindChild ("TouchCol").gameObject);
				}catch{
					clickCharacter (saveChara.transform.FindChild ("TouchCol").gameObject);
				}
			}
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask.value)) {
				if (hit.collider.gameObject.transform.parent.gameObject == saveChara)
					clickCharacter (saveChara.transform.FindChild ("TouchCol").gameObject);
				else if (hit.collider.gameObject.transform.parent.gameObject.layer == 9 && saveChara.layer == 10
					|| hit.collider.gameObject.transform.parent.gameObject.layer == 10 && saveChara.layer == 9) {
					GameObject cSerch;
					if (hit.collider.gameObject.transform.parent.gameObject.layer == 10)
						cSerch = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Blue> ().tgt;
					else
						cSerch = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Red> ().tgt;


					////////////////////////
					if (saveChara.layer == 10) {
						foreach (GameObject obj in saveChara.GetComponent<Blue>().atEnemys)
							obj.GetComponent<Light> ().color = Color.red;
						if (hit.collider.gameObject.transform.parent.gameObject != saveChara.GetComponent<Blue> ().tgt) {
							saveChara.SendMessage ("changeAttack", hit.collider.gameObject.transform.parent.gameObject);
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;
						} else if (cSerch == saveChara.GetComponent<Blue> ().tgt)
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.blue;
					} else if (saveChara.layer == 9) {
						foreach (GameObject obj in saveChara.GetComponent<Red>().atEnemys)
							obj.GetComponent<Light> ().color = Color.blue;

						if (hit.collider.gameObject.transform.parent.gameObject != saveChara.GetComponent<Red> ().tgt) {
							saveChara.SendMessage ("changeAttack", hit.collider.gameObject.transform.parent.gameObject);
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;
						} else if (cSerch == saveChara.GetComponent<Blue> ().tgt)
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.red;
					}
					////////////////////////////
					gameObject.GetComponent<LineRenderer> ().enabled = false;
					rayMouse = false;
				}
			}
		}
		if(Input.GetMouseButton(0)){
			if (!rayMouse) {

				GameObject cube = GameObject.Find ("SerchBlock(Clone)");
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//マウスのポジションにrayを飛ばす
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, Mathf.Infinity, maskTerrain.value)) {
					cube.transform.position = new Vector3 (((Mathf.Abs(hit.point.x)-Mathf.Abs(squadPos1.x))/2f)+squadPos1.x,3f,((Mathf.Abs(hit.point.z)-Mathf.Abs(squadPos1.z))/2f)+squadPos1.z);
					cube.transform.localScale = new Vector3 (Mathf.Abs (hit.point.x) - Mathf.Abs (squadPos1.x), 1f, Mathf.Abs (hit.point.z) - Mathf.Abs (squadPos1.z));
				}

			}
		}

		if (Input.GetKey ("d")) {
			Playercamera.transform.position = new Vector3 (Playercamera.transform.position.x,Playercamera.transform.position.y,Playercamera.transform.position.z+Time.deltaTime*50);
		}
		if (Input.GetKey ("a")) {
			Playercamera.transform.position = new Vector3 (Playercamera.transform.position.x,Playercamera.transform.position.y,Playercamera.transform.position.z-Time.deltaTime*50);
		}
		if (rayMouse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//毎フレーム画面にRayを飛ばす
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {//Rayがヒットしたら
				if (hit.collider.gameObject != rayobj && rayobj != null) //ヒットしたオブジェクトが前回のオブジェクと出なければ　
					rayobj.GetComponent<Light> ().enabled = false;//前回ヒットしたオブジェクトのライトをオフにする

				gameObject.GetComponent<LineRenderer> ().SetPosition (0, lineObject.transform.position);//Lineを飛ばす地点１
				gameObject.GetComponent<LineRenderer> ().SetPosition (1, hit.point);//Lineを飛ばす地点２
			}
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask.value)) {//rayがキャラクターに当たる
				try{
					if (saveChara.layer == 10 && hit.collider.gameObject.transform.parent.gameObject.layer == 9
						|| saveChara.layer == 9 && hit.collider.gameObject.transform.parent.gameObject.layer == 10) {
						//rayが当たったキャラと以前当たったキャラが敵対していたら
						GameObject cSerch;//rayが当たっとキャラクターのターゲット
						if (hit.collider.gameObject.transform.parent.gameObject.layer == 9)
							cSerch = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Red> ().tgt;
						else
							cSerch = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Blue> ().tgt;

						if (cSerch != saveChara) {
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().enabled = true;//当たったキャラのライトをON
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;//黄色く光らせる
							rayobj = hit.collider.gameObject.transform.parent.gameObject;//rayMouse内での判定用に保存
						}
					} else {
						if (saveChara == hit.collider.gameObject.transform.parent.gameObject)
							return;
					}
				}catch{
					return;
				}
			}
		}
	}

	private void clickCharacter (GameObject clickChara)
	{
		GameObject parent = clickChara.transform.parent.gameObject;//クリックしたオブジェクトの親オブジェクト
		if (parent.gameObject.layer == 10 || parent.gameObject.layer == 9) { //上記オブジェクトがキャラクターであるならば
			bool lineFlag = parent.GetComponent<LineRenderer> ().enabled;//クリックしたオブジェクトのラインのbool
			gameObject.GetComponent<LineRenderer> ().enabled = !lineFlag;//上記変数を反転した値をenableに代入
			rayMouse = !lineFlag;//上記同様
			if (saveChara != null) {//前回クリックしたキャラがいれば
				saveChara.GetComponent<LineRenderer> ().enabled = false;//前回のキャラのラインを消す
				saveChara.GetComponent<Light> ().enabled = false;//前回のキャラのライトを消す

				if (saveChara.layer == 9) {//前回のキャラがエネミーであれば
					try{
						saveChara.GetComponent<Red> ().lightup = false;//ライトを消して
						saveChara.GetComponent<Red> ().tgt.GetComponent<Light> ().enabled = false;
					}catch{
					}

					//					foreach (GameObject saveatEnemys in saveChara.GetComponent<Red>().atEnemys) //エネミーに注目する敵(自分)
					//						saveatEnemys.GetComponent<Light> ().enabled = false;//ライトを消す
					saveChara.GetComponent<Red>().atEnemys.ForEach(i => i.GetComponent<Light>().enabled = false);

				} else if (saveChara.layer == 10) {
					try{
						saveChara.GetComponent<Blue> ().lightup = false;
						saveChara.GetComponent<Blue> ().tgt.GetComponent<Light> ().enabled = false;
					}catch{
					}

					//					foreach (GameObject saveatEnemys in saveChara.GetComponent<Blue>().atEnemys)
					//						saveatEnemys.GetComponent<Light> ().enabled = false;
					saveChara.GetComponent<Blue> ().atEnemys.ForEach (i=> i.GetComponent<Light> ().enabled = false);
				}
			}

			parent.GetComponent<LineRenderer> ().enabled = !lineFlag;
			parent.GetComponent<Light> ().enabled = !lineFlag;

			if (parent.layer == 10) {//クリックしたのがプレイヤーであれば
				parent.GetComponent<Blue> ().lightup = !lineFlag;
				parent.GetComponent<Light> ().color = Color.blue;
				if (parent.GetComponent<Blue> ().tgt != null && parent.GetComponent<Blue> ().tgt != GameObject.Find ("summonRed")) {
					try{
						parent.GetComponent<Blue> ().tgt.GetComponent<Light> ().enabled = !lineFlag;
						if (parent.GetComponent<Blue> ().tgt == parent.GetComponent<Blue> ().attackObj)
							parent.GetComponent<Blue> ().tgt.GetComponent<Light> ().color = Color.blue;
					}catch{
					}
				}


			} else if (parent.layer == 9) {//クリックしたのエネミーであれば
				parent.GetComponent<Red> ().lightup = !lineFlag;
				parent.GetComponent<Light> ().color = Color.red;
				rayMouse = false;
				gameObject.GetComponent<LineRenderer> ().enabled = false;
				if (parent.GetComponent<Red> ().tgt != null && parent.GetComponent<Red> ().tgt != GameObject.Find ("summonBlue")) {
					try{
						parent.GetComponent<Red> ().tgt.GetComponent<Light> ().enabled = !lineFlag;
						if (parent.GetComponent<Red> ().tgt == parent.GetComponent<Red> ().attackObj)
							parent.GetComponent<Red> ().tgt.GetComponent<Light> ().color = Color.red;
					}catch{
					}
				}

			}
			saveChara = parent;
		}
	}

	public static void charaDestroy (GameObject chara)
	{
		if (chara != saveChara)
			return;

		Playercamera.transform.FindChild("Main Camera").gameObject.GetComponent<LineRenderer> ().enabled = false;
		rayMouse = false;
		if (chara.layer == 9) {
			//			foreach (GameObject lightDown in chara.GetComponent<Red>().atEnemys)
			//				lightDown.GetComponent<Light> ().enabled = false;
			chara.GetComponent<Red>().atEnemys.ForEach(i=>i.GetComponent<Light>().enabled = false);
		} else {
			//			foreach (GameObject lightDown in chara.GetComponent<Blue>().atEnemys)
			//				lightDown.GetComponent<Light> ().enabled = false;
			chara.GetComponent<Blue>().atEnemys.ForEach(i=>i.GetComponent<Light>().enabled = false);
		}
	}
}