using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Soldier: MonoBehaviour
{
	public static List<GameObject> soldiers = new List<GameObject>();
	public GameObject tgt;
	//攻撃先s
	public int HP;
	//自分のHP
	public string state;
	//自分の状態
	public List<GameObject> atEnemys = new List<GameObject> (), behindAlly = new List<GameObject> ();
	//自分に攻撃してる敵のリスト
	private bool dir;
	//迂回時の方向
	//public bool lightup = false;
	public GameObject attackObj;

	private AudioClip hit;
	private AudioClip ready;
	private AudioSource source;


	void Start ()
	{
		soldiers.Add (gameObject);
		source = gameObject.GetComponent<AudioSource> ();
		tgt = GameObject.Find (Name ("tgtName", false));//移動先!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		gameObject.name = (Name ("myName", false)); //名前に味方召喚数の変数を付随させる
		state = "move";//初期状態を移動にする
		StartCoroutine (move (distance (tgt.transform.position, gameObject.transform.position), true));
		HP = 200;//初期体力は200
		dir = Random.value > 0.5f ? true : false;//迂回時の左右方向を50%で分けてる
		UIHP.targets.Add (gameObject.transform);//召喚したオブジェクトをHP表示するオブジェクトのリストに入れる

	}



	// Update is called once per frame
	void Update ()
	{
		switch (state) {//自分の状態を要素としswitch文

		case "fight"://戦闘中であれば
			try {
				transform.LookAt (tgt.transform);//敵に注目
			} catch {
				tgt = GameObject.Find (Name ("tgtName", false));
				changeAttack (tgt);
			}
			break;
		default :
			break;
		}
		if (HP < 0) {
			Death ();//HPがゼロになっていたら死亡
		}
	}

	public void lightup ()
	{
		line lined = gameObject.GetComponent<line> ();
		lined.setup (color (Name ("myTag", false)), tgt, atEnemys);
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
			teamCount = GameManager.ServantCount;
		}

		switch (need) {
		case"tgtName"://※※※※※※※※※※※進行状況によって対象を変化させる必要あり※※※※※※※※※※※※※※※※
			if (!not) {
				s = team == "Red" ? "BlueFirstCrystal" : "RedFirstCrystal";
			} else {
				s = team == "Red" ? "RedFirstCrystal" : "BlueFirstCrystal";
			}
			break;
		case"myName":
			if (transform.localScale == new Vector3 (6, 6, 6)) {
				s = team + "Warrior" + teamCount;
				hit = (AudioClip)Resources.Load ("Sound/W_hit");
				ready = (AudioClip)Resources.Load ("Sound/W_ready");
				source.clip = ready;
				source.Play ();
			} else if (transform.localScale == new Vector3 (6.05f, 6.05f, 6.05f)) {
				s = team + "Witch" + teamCount;
				hit = (AudioClip)Resources.Load ("Sound/W_hit");
				ready = (AudioClip)Resources.Load ("Sound/Wi_ready");
			} else if (transform.localScale == new Vector3 (6.1f, 6.1f, 6.1f)) {
				s = team + "Guard" + teamCount;
				hit = (AudioClip)Resources.Load ("Sound/W_hit");
				ready = (AudioClip)Resources.Load ("Sound/W_ready");
				source.clip = ready;
				source.Play ();
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
			GameObject test = col.gameObject.transform.parent.gameObject;
		} catch {
			return;
		}
		GameObject colOb = col.gameObject.transform.parent.gameObject;

		
		if (tgt.layer == LayerMask.NameToLayer (Name ("myTag", true)) && tgt != colOb || col.gameObject.layer != 8) {
			//プレイヤーの指示による攻撃対象とは違うオブジェクトであればreturn
			return;
		}

		if (colOb.layer == LayerMask.NameToLayer (Name ("myTag", true)) && state != "fight") {
			//移動中にぶつかったら
			Soldier script = colOb.GetComponent<Soldier> ();
			if (script.atEnemys.Count < 3) {
				//敵を狙う味方の数が３人未満であれば
				script.atEnemys.Add (gameObject);//自分を敵のリストに追加
				tgt = colOb;//攻撃対象に選択
				attackObj = tgt;//攻撃開始対象
				state = "fight";//状態を攻撃中に変更
				behindAlly.ForEach (i => i.GetComponent<Soldier> ().StartCoroutine (i.GetComponent<Soldier> ().keepAway (gameObject, 10)));
				//自分の背後にいる味方をKeepAwaYさせる
				behindAlly.Clear ();//背後のリストを真っ白にする
				tag = Name ("myStopTag", false);//自分のタグを停止中にする
				StartCoroutine (attack (30)); //威力３０で攻撃開始
			}
		} else if (colOb.layer == LayerMask.NameToLayer (Name ("tgtLayer", false)) || colOb.layer == 2 + LayerMask.NameToLayer (Name ("tgtLayer", false))) {
			//タワーにぶつかったら
			if (gameObject.CompareTag (Name ("myTag", false)) && state != "fight") {
				tgt = colOb;
				attackObj = tgt;
				state = "fight";
				behindAlly.ForEach (i => i.GetComponent<Soldier> ().StartCoroutine (i.GetComponent<Soldier> ().keepAway (gameObject, 10)));
				behindAlly.Clear ();
				tag = Name ("myStopTag", false);
				StartCoroutine (attack (30)); //攻撃
			}
		}
		if (colOb == tgt) {
			//衝突対象が攻撃対象であればに攻撃対象色
			colOb.GetComponent<Light> ().color = color (Name ("myTag", false));
	
		}
	}

	//====================================================================================
	//避けるメソッド  BOIDBOIDBOIDBOIDBOIDBOIDBOID
	//====================================================================================
	/// <summary>
	/// Keeps the away.ベジェ曲線
	/// </summary>
	/// <returns>The away.</returns>
	/// <param name="front">Front.</param>
	/// <param name="speed">Speed.</param>
	public IEnumerator keepAway (GameObject front, int speed)
	{

		RaycastHit hit;

		if (Physics.Raycast	(transform.position, dir ? Vector3.right : Vector3.left, out hit, transform.localScale.x / 2 + (dir ? 5 : -5))) {
			if (Physics.Raycast (transform.position, dir ? Vector3.left : Vector3.right, out hit, transform.localScale.x / 2 + (dir ? -5 : 5))) {

				tag = Name ("myStopTag", false);
				yield break;
			} else {
				dir = !dir;
			}
		}

		float AwayDis = 0f;
		float frScale = front.transform.localScale.x / 2;
		float myScale = transform.localScale.x / 2;
		float frPos = front.transform.position.x;
		float myPos = transform.position.x;
		//====================================================================================
		//避ける先
		//====================================================================================
		if (dir) {//右に避ける
			if (frPos <= myPos) {//自分のほうが右にいる場合
				AwayDis = myScale + frScale - (myPos - frPos - 2);
			} else {
				AwayDis = myScale + frScale + (frPos - myPos + 2);
			}
		} else {//左に避ける
			if (frPos <= myPos) {//自分のほうが左にいる場合
				AwayDis = -myScale + -frScale + (frPos - myPos - 3);
			} else {
				AwayDis = -myScale + -frScale + (myPos - frPos - 3);

			}
		}
		//____________________________________________________________________________________
		//z軸とx軸の二点から角度を求める　Y/Z
		//____________________________________________________________________________________
		state = "detour";
		Vector3 p = transform.position;
		//____________________________________________________________________________________
		//避ける
		//____________________________________________________________________________________
		for (float t = 0; t < 1; t += Time.deltaTime * 0.3f) {//速度に対応して変更する必要あり
			if (transform.position.x > 307 ||
			   transform.position.x < 207) {
				transform.LookAt (tgt.transform);

				if (Physics.Raycast (transform.position, Vector3.forward, out hit, 10)) {
					
					tag = Name ("myStopTag", false);
					yield break;
				}
				break;
			}//ベジェ曲線 一個前のバージョンはGithub参照
			try {
				transform.position = new Vector3 ((1 - t) * (1 - t) * p.x + 2 * (1 - t) * t * (frPos + (dir ? +3 : -3)) + t * t * (p.x + AwayDis)
											, transform.position.y
					, (1 - t) * (1 - t) * p.z + 2 * (1 - t) * t * (front.transform.position.z - 3) + t * t * front.transform.position.z);
			} catch {
			}
			yield return null;
		}
		//____________________________________________________________________________________
		//避け終わったら状態を移動中にする
		//____________________________________________________________________________________
		state = "move";
		gameObject.tag = Name ("myTag", false);
		changeAttack (GameObject.Find (Name ("tgtName", false)));
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
		source.clip = ready;
		try {
			GameObject m = gameObject.transform.FindChild ("Magic").gameObject;
			m.GetComponent<SpriteRenderer> ().enabled = true;
			source.Play ();
		} catch {
			goto through;
		}
		yield return new WaitForSeconds (1);
		through:
		;
		source.clip = hit;
		while (state == "fight") {
			yield return new WaitForSeconds (3);
			try {
				tgt.GetComponent<Soldier> ().HP -= power;
			} catch {
				tgt.GetComponent<crystal> ().HP -= power;
			}
			source.Play ();
			GameObject myHPBar = GameObject.Find (tgt.name + ("hp(Clone)"));
			myHPBar.transform.localScale -= new Vector3 (0.15f, 0, 0);
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
		})) {
			attackObj = tgt;//Atenemyだけでなく、TriggerEnter中も探す必要あり
		} else {
			state = "move";
			tag = Name ("myTag", false);
			StopAllCoroutines ();
			try {
				GameObject m = gameObject.transform.FindChild ("Magic").gameObject;
				m.GetComponent<SpriteRenderer> ().enabled = false;
			} catch {

			}
			StartCoroutine (move (distance (tgt.transform.position, transform.position), true));
		}
	}

	//====================================================================================
	//死んだ時におこなうメソッド
	//====================================================================================
	private void Death ()
	{
		char[] n = name.ToCharArray ();
		if (n [0] == 'B') {
			switch (n [6]) {// B 0 L 1 U 2 E 3 W 4 A 5 R 6
			case 'r':
				GameManager.WarriorCount--;
				break;
			case 't':
				GameManager.WitchCount--;
				break;
			case 'a':
				GameManager.GuardCount--;
				break;
			}
		}
//		lightup = false;
		if (gameObject == Instruction.rayobj) {
			Instruction.rayobj = null;
		}
		if (tgt.layer == LayerMask.NameToLayer (Name ("myTag", true))) {
			tgt.GetComponent<Soldier> ().atEnemys.Remove (gameObject);

			Instruction.charaDestroy (gameObject);
			foreach (GameObject enemy in atEnemys) {//自分を狙っている敵
				try {
					Soldier script = enemy.GetComponent<Soldier> ();//敵のスクリプトを入手
					script.atEnemys.Remove (gameObject);
					if (script.atEnemys.Count == 0) {//敵を狙っている味方がいなければ
						script.tgt = GameObject.Find (Name ("tgtName", true));//敵のターゲットを自陣に
						script.changeAttack (script.tgt);
					} else
						script.tgt = script.atEnemys [0];
				} catch {
				}
			}
			if (Instruction.saveChara == gameObject)
				Instruction.saveChara = null;
			UIHP.targets.Remove (gameObject.transform);
			Destroy (GameObject.Find (gameObject.name + "hp(Clone)"));
			GameManager.sp += 10;
			Destroy (gameObject);

		}
	}
}