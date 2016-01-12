﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Soldier: MonoBehaviour
{
	public GameObject tgt;
	//攻撃先s
	public int HP;
	//自分のHP
	public string state;
	//自分の状態
	public List<GameObject> atEnemys = new List<GameObject> (), behindAlly = new List<GameObject> ();
	//自分に攻撃してる敵のリスト
	private bool right;
	//迂回時の方向
	public bool lightup = false;
	public GameObject attackObj;


	void Start ()
	{
		tgt = GameObject.Find (Name ("tgtName", false));//移動先!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		if (transform.localScale == new Vector3 (6, 6, 6))
			gameObject.name = (Name ("myName", false)); //名前に味方召喚数の変数を付随させる
		 else if (transform.localScale == new Vector3 (10, 10, 10))
			gameObject.name = (Name ("myName", false)); //名前に味方召喚数の変数を付随させる
		else if (transform.localScale == new Vector3 (8, 8, 8))
			gameObject.name = (Name ("myName", false)); //名前に味方召喚数の変数を付随させる
		state = "move";//初期状態を移動にする
		StartCoroutine (move (distance (tgt.transform.position, gameObject.transform.position), true));
		HP = 200;//初期体力は200
		right = Random.value > 0.5f ? true : false;//迂回時の左右方向を50%で分けてる
		UIHP.targets.Add (gameObject.transform);//召喚したオブジェクトをHP表示するオブジェクトのリストに入れる

	}



	// Update is called once per frame
	void Update ()
	{
		switch (state) {//自分の状態を要素としswitch文
		case "move"://移動中であれば
			
			break;
		case "detour"://迂回であれば
			//keepAway (10, radian);//※※※※※※※※※※※※※※※※※※※※※※※※※※※※※
			break;
		case "fight"://戦闘中であれば
			try {
				transform.LookAt (tgt.transform);//敵に注目
			} catch {
				tgt = GameObject.Find (Name ("tgtName", false));
				state = "move";
				StartCoroutine (move (distance (tgt.transform.position, gameObject.transform.position), true));
			}
			break;
		default :
			break;
		}
		if (HP < 0) {
			Death ();//HPがゼロになっていたら死亡
		}
		
		if (lightup) {
			LineRenderer linerende = GetComponent<LineRenderer> ();//LineRendererコンポーネントを変数に
			linerende.SetPosition (0, transform.position);
			linerende.SetPosition (1, tgt.transform.position);
			try {
				if (tgt.GetComponent<Light> ().enabled == false)
					tgt.GetComponent<Light> ().enabled = true;
				if (attackObj != tgt && tgt.GetComponent<Light> ().color != Color.yellow) {
					tgt.GetComponent<Light> ().color = Color.yellow;
				} else if (attackObj == tgt && tgt.GetComponent<Light> ().color == color (Name ("myTag", true)) && atEnemys.Count == 0) {
					tgt.GetComponent<Light> ().color = color (Name ("myTag", false));
				}
			} catch {
			}
			if (gameObject.GetComponent<Light> ().color != color (Name ("myTag", false)))
				gameObject.GetComponent<Light> ().color = color (Name ("myTag", false));
			for (int i = 0; i < atEnemys.Count; i++) {
				if (atEnemys [i].GetComponent<Light> ().enabled == false) {
					atEnemys [i].GetComponent<Light> ().enabled = true;
					atEnemys [i].GetComponent<Light> ().color = color (Name ("myTag", true));
					if (atEnemys [i].name == tgt.name) {
						atEnemys [i].GetComponent<Light> ().color = color (Name ("myTag", false));
					}
				}
				linerende.SetVertexCount (2 + ((i + 1) * 2));
				linerende.SetPosition (2 + ((i + 1) * 2 - 2), transform.position);
				linerende.SetPosition (2 + ((i + 1) * 2 - 1), atEnemys [i].transform.position);
			}
		}
	}

	//====================================================================================
	//移動
	//====================================================================================
	public IEnumerator move (Vector3 dis, bool auto)
	{
		
		float remDis = Mathf.Abs (dis.x) + Mathf.Abs (dis.z);
		dis = dis.normalized;
		while (remDis > 0 && state == "move") {
			if (auto) {
				try {
					transform.LookAt (tgt.transform);//移動先に注目
				} catch {
					tgt = GameObject.Find (Name ("tgtName", false));
				}
			}

			float speedZ = (dis.z * 10 * Time.deltaTime);
			float speedX = (dis.x * 10 * Time.deltaTime);

			gameObject.transform.position = new Vector3 (transform.position.x + speedX, transform.position.y, transform.position.z + speedZ);
			remDis -= Mathf.Abs (speedX) + Mathf.Abs (speedZ);
			if (remDis < 1) {
				tgt = GameObject.Find (Name ("tgtName", false));
				changeAttack (tgt);
			}
			yield return null;
		}
	}

	//====================================================================================
	//求められた色を返すメソッド
	//====================================================================================
	public static Color color (string name)
	{
		Color c;
		if (name == "Player") {
			c = Color.blue;
		} else {
			c = Color.red;
		}
		return c;
	}

	//====================================================================================
	//求められたNameを返すメソッド
	//====================================================================================
	public string Name (string need, bool not)
	{
		
		int mask = LayerMask.NameToLayer ("Enemy");
		string s = "";
		string team = "";
		int teamCount;

		if (gameObject.layer == mask) {
			team = "Red";
			teamCount = EnemyControl.servantCount;
		} else { 
			team = "Blue";
			teamCount = summonsServant.servantCount;
		}

		switch (need) {
		case"tgtName"://※※※※※※※※※※※進行状況によって対象を変化させる必要あり※※※※※※※※※※※※※※※※
			s = team == "Red" ? "BlueFirstCrystal" : "RedFirstCrystal";
			break;
		case"myName":
			if (transform.localScale == new Vector3 (6, 6, 6)) {
				s = team + "Warrior" + teamCount;
			} else if (transform.localScale == new Vector3 (10, 10, 10)) {
				s = team + "Witch" + teamCount;
			} else if (transform.localScale == new Vector3 (8, 8, 8)) {
				s = team + "Guard" + teamCount;
			}
			break;
		case"myTag":
			if (team == "Red") {
				if (!not) {
					s = "Enemy";
				} else {
					s = "Player";
				}
			} else {
				if (!not) {
					s = "Player";
				} else {
					s = "Enemy";
				}
			}
			break;
		case "myStopTag":
			if (team == "Red") {
				s = "StopEnemy";
			} else {
				s = "StopPlayer";
			}
			break;
		case "tgtLayer":
			if (team == "Red")
				s = "BlueCrystal";
			else
				s = "RedCrystal";
			break;
		}
		return s;
	}

	//====================================================================================
	//衝突時
	//====================================================================================
	void OnTriggerEnter (Collider col)
	{
		try {
			if (tgt.layer == LayerMask.NameToLayer (Name ("myTag", true)) && tgt != col.gameObject) {
				return;
			}
		} catch {
			return;
		}
		if (col.gameObject.layer == LayerMask.NameToLayer (Name ("myTag", true)) && state != "fight") {
			Soldier script = col.gameObject.GetComponent<Soldier> ();
			if (script.atEnemys.Count < 3) {
				script.atEnemys.Add (gameObject);
				tgt = col.gameObject;
				attackObj = tgt;
				state = "fight";
				//behindAlly.ForEach (i=> i.GetComponent<Soldier>().StopAllCoroutines());
				behindAlly.ForEach (i => i.GetComponent<Soldier> ().StartCoroutine( i.GetComponent<Soldier>().keepAway (gameObject,10)));
				behindAlly.Clear ();
				tag = Name ("myStopTag", false);
				StartCoroutine (attack (30)); //攻撃
			}
		} else if (col.gameObject.layer == LayerMask.NameToLayer (Name ("tgtLayer", false)) || col.gameObject.layer == 1 << 2 + LayerMask.NameToLayer (Name ("tgtLayer", false))) {

			if (gameObject.CompareTag (Name ("myTag", false)) && state != "fight") {
				tgt = col.gameObject;
				attackObj = tgt;
				state = "fight";
				//behindAlly.ForEach (i=> i.GetComponent<Soldier>().StopAllCoroutines());
				behindAlly.ForEach (i => i.GetComponent<Soldier> ().StartCoroutine( i.GetComponent<Soldier>().keepAway (gameObject,10)));
				behindAlly.Clear ();
				tag = Name ("myStopTag", false);
				StartCoroutine (attack (30)); //攻撃
			}
		}
		if (col.gameObject == tgt) {
			col.gameObject.GetComponent<Light> ().color = color (Name ("myTag", false));
	
		}
	}

	//====================================================================================
	//避けるメソッド
	//====================================================================================
	public IEnumerator keepAway (GameObject front,int speed)
	{
		float detourDis = 0f;
		float frScale = front.transform.localScale.x / 2;
		float myScale = transform.localScale.x / 2;
		float frPos = front.transform.position.x;
		float myPos = transform.position.x;
		var rot = transform.rotation;

		//====================================================================================
		//避ける先
		//====================================================================================
			if (right) {//右に避ける
				if (frPos <= myPos) {//自分のほうが右にいる場合
					detourDis = myScale + frScale - (myPos - frPos - 3);
				} else {
					detourDis = myScale + frScale + (frPos - myPos + 3);
				}
			} else {//左に避ける
				if (frPos <= myPos) {//自分のほうが左にいる場合
					detourDis = -myScale + -frScale + (frPos - myPos - 3);
				} else {
					detourDis = -myScale + -frScale + (myPos - frPos - 3);

				}
			}
		//____________________________________________________________________________________
		//z軸とx軸の二点から角度を求める　Y/Z
		//____________________________________________________________________________________
		state = "detour";
		float dx = front.transform.position.z - gameObject.transform.position.z;
		float radian = Mathf.Atan2 (detourDis, dx) * Mathf.Rad2Deg;
		Vector3 dis = distance (front.transform.position,transform.position);
		dis = dis.normalized;

		//____________________________________________________________________________________
		//避ける
		//____________________________________________________________________________________
		for(float t = 0;t<1;t+=Time.deltaTime){
			if(transform.position.x>307||
				transform.position.x < 207){
				yield break;
			}
			transform.rotation = Quaternion.Slerp (rot, Quaternion.Euler (rot.x, radian, rot.z), t);
			transform.Translate (transform.forward *(dis.z * speed * Time.deltaTime));
			yield return null;
		}
		//____________________________________________________________________________________
		//避け終わったら状態を移動中にする
		//____________________________________________________________________________________
		state = "move";
		gameObject.tag = Name ("myTag", false);
		changeAttack(GameObject.Find( Name ("tgtName", false)));
	}

	public static Vector3 distance (Vector3 target, Vector3 me)
	{
		Vector3 dis = target - me;

		return dis;
	}

	//====================================================================================
	//3秒に一回攻撃(威力:引数)
	//====================================================================================
	private IEnumerator attack (int power)
	{
		while (state == "fight") {
			try {
				tgt.GetComponent<Soldier> ().HP -= power;
			} catch {
				tgt.GetComponent<crystal> ().HP -= power;
			}
			GameObject myHPBar = GameObject.Find (tgt.name + ("hp(Clone)"));
			myHPBar.transform.localScale -= new Vector3 (0.15f, 0, 0);
			yield return new WaitForSeconds (3);
		}
	}

	//====================================================================================
	//攻撃対象変更
	//====================================================================================
	private void changeAttack (GameObject obj)
	{
		try {
			if (tgt.GetComponent<Light> ().color == Color.yellow)
				tgt.GetComponent<Light> ().enabled = false;
		} catch {
		}
		if (tgt.layer == LayerMask.NameToLayer (Name ("myTag", true)))
			tgt.GetComponent<Soldier> ().atEnemys.Remove (gameObject);
		tgt = obj;
		if (atEnemys.Find (delegate(GameObject ob) {
			return ob.name == tgt.name;
		})){
			attackObj = tgt;//Atenemyだけでなく、TriggerEnter中も探す必要あり
		}else {
			state = "move";
			tag = Name ("myTag", false);
			//StopAllCoroutines ();
			StartCoroutine(move (distance(tgt.transform.position,transform.position),true));
		}
	}

	//====================================================================================
	//死んだ時におこなうメソッド
	//====================================================================================
	private void Death ()
	{
		lightup = false;
		if (gameObject == Instruction.rayobj) {
			Instruction.rayobj = null;
		}
		if (tgt.layer == 1 << LayerMask.NameToLayer (Name ("myTag", true))) {
			tgt.GetComponent<Soldier> ().atEnemys.Remove (gameObject);
		}
		try {
			tgt.GetComponent<LineRenderer > ().SetVertexCount (tgt.GetComponent<Soldier> ().atEnemys.Count + 2);
		} catch {

		}
		Instruction.charaDestroy (gameObject);
		foreach (GameObject enemy in atEnemys) {//自分を狙っている敵
			try {
				Soldier script = enemy.GetComponent<Soldier> ();//敵のスクリプトを入手
				script.atEnemys.Remove (gameObject);
				if (script.atEnemys.Count == 0) {//敵を狙っている味方がいなければ
					script.state = "move";//敵の状態をmove
					StartCoroutine (move (distance (tgt.transform.position, gameObject.transform.position), true));
					script.tgt = GameObject.Find ("summonBlue");//敵のターゲットを自陣に
					enemy.tag = Name ("myTag", true);//敵のタグを一般に
				} else
					script.tgt = script.atEnemys [0];
			} catch {
				Debug.Log (gameObject.name + "   " + enemy.name);
			}
		}
		if (Instruction.saveChara == gameObject)
			Instruction.saveChara = null;
		UIHP.targets.Remove (gameObject.transform);
		Destroy (GameObject.Find (gameObject.name + "hp(Clone)"));
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}