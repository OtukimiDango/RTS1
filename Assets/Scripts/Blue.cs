using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blue : MonoBehaviour
{
	public GameObject tgt;
//攻撃先
	public Vector3 tgtDis;
//攻撃先までの距離

	public byte HP;
//自分のHP
	private Vector3 myPos;
//自分の座標
	private Vector3 savePos;
//自分の座標を一時的に保存する変数
	private float detourDis;
//迂回する距離
	private const int speed = 10;
//移動速度
	public string state;
//自分の状態
	public GameObject frontAlly = null;
//前方の味方
	public static List<GameObject> allys = new List<GameObject> ();
//味方のリスト
	public List<GameObject> atEnemys = new List<GameObject> ();
//自分に攻撃してる敵のリスト
	private GameObject saveFrontAlly;
//前方の味方を判定するときにブッキングを回避するための保存用変数
	private bool right;
//迂回時の方向
	private bool attackSpace = true;
//攻撃のクールタイムが終了しているか
	public LineRenderer linerende;
//ラインレンダラー
	public bool detourbool = false;
	public bool lightup = false;
	public GameObject attackObj;
	float dx ,dy, radian=1f,radi = 0f, i = 0;


	void Start ()
	{
		linerende = GetComponent<LineRenderer> ();//LineRendererコンポーネントを変数に
		tgt = GameObject.Find ("summonRed");//移動先!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		myPos = transform.position;//自分のポジションを入れる
		if (transform.localScale == new Vector3 (6, 6, 6)) 
			gameObject.name = ("BlueSoldier" + summonsServant.servantCount); //名前に味方召喚数の変数を付随させる
		 else if (transform.localScale == new Vector3 (10, 10, 10))
			gameObject.name = ("BlueWitch" + summonsServant.servantCount); //名前に味方召喚数の変数を付随させる
		else if (transform.localScale == new Vector3 (8, 8, 8))
			gameObject.name = ("BlueGuard" + summonsServant.servantCount); //名前に味方召喚数の変数を付随させる
		tgtDis = distance (tgt.transform.position, myPos);//移動先の座標と自分の座標の差分を図り、変数にいれる
		tgtDis = tgtDis.normalized;
		state = "move";//初期状態を移動にする
		HP = 200;//初期体力は200
		right = Random.value > 0.5f ? true : false;//迂回時の左右方向を50%で分けてる
		UIHP.targets.Add (gameObject.transform);//召喚したオブジェクトをHP表示するオブジェクトのリストに入れる

	}

	// Update is called once per frame
	void Update ()
	{
		switch (state) {//自分の状態を要素としswitch文
		case "move"://移動中であれば
			transform.LookAt (tgt.transform);//移動先に注目
			myPos = myPos + (tgtDis * speed * Time.deltaTime);//移動先へ移動
			transform.position = myPos;//変更された変数を自分のポジションへ代入
			break;
		case "detour"://迂回であれば
			detour(gameObject.transform);
			break;
		case "fight"://戦闘中であれば
			transform.LookAt (tgt.transform);//敵に注目
			if (attackSpace)//攻撃のクールタイムが終わっていれば
				StartCoroutine (attack ()); //攻撃
			break;
		default :
			break;
		}
		if (HP < 0)
			Death ();//HPがゼロになっていたら死亡
		if (lightup) {
			linerende.SetPosition (0, transform.position);
			linerende.SetPosition (1, tgt.transform.position);
			if (tgt.GetComponent<Light> ().enabled == false)
				tgt.GetComponent<Light> ().enabled = true;
			if (attackObj != tgt && tgt.GetComponent<Light> ().color != Color.yellow) {
				tgt.GetComponent<Light> ().color = Color.yellow;
			}else if (attackObj == tgt && tgt.GetComponent<Light> ().color == Color.red && atEnemys.Count == 0){
				tgt.GetComponent<Light> ().color = Color.blue;
			}
			if (gameObject.GetComponent<Light> ().color != Color.blue)
				gameObject.GetComponent<Light> ().color = Color.blue;
			for (int i = 0; i < atEnemys.Count; i++) {
				if (atEnemys [i].GetComponent<Light> ().enabled == false) {
					atEnemys [i].GetComponent<Light> ().enabled = true;
					atEnemys [i].GetComponent<Light> ().color = Color.red;
					if (atEnemys [i].name == tgt.name)
						atEnemys [i].GetComponent<Light> ().color = Color.blue;
				}
				linerende.SetVertexCount (2 + ((i + 1) * 2));
				linerende.SetPosition (2 + ((i + 1) * 2 - 2), transform.position);
				linerende.SetPosition (2 + ((i + 1) * 2 - 1), atEnemys [i].transform.position);
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (tgt.layer == 9 && tgt != col.gameObject) {
			return;
		}
		switch (col.gameObject.tag) {
		case "Enemy":
			if (gameObject.tag == ("Player") && state != "fight") {
				Red script = col.gameObject.GetComponent<Red> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					tgt = col.gameObject;
					attackObj = tgt;
					state = "fight";
					gameObject.tag = "StopPlayer";
				}
			}
			break;
		case "StopEnemy":
			if (gameObject.tag == ("Player") && state != "fight") {
				Red script = col.gameObject.GetComponent<Red> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					tgt = col.gameObject;
					attackObj = tgt;
					state = "fight";
					gameObject.tag = "StopPlayer";
				}
			}
			break;
		case "summonBlue":
			state = "fight";
			gameObject.tag = "StopPlayer";
			break;
		default :
			break;
		}
		if (col.gameObject == tgt){
			col.gameObject.GetComponent<Light> ().color = Color.blue;
	
		}
	}

	public void detourReady ()
	{
		if (saveFrontAlly != frontAlly) { 
			saveFrontAlly = frontAlly;
			detourDis = right ? saveFrontAlly.transform.localScale.x : -(saveFrontAlly.transform.localScale.x);
			state = "detour";
			detourbool = true;
			detour (gameObject.transform);//迂回を開始する
		}
	}

	private void detour (Transform me)
	{
		if (detourbool) {
			dy = frontAlly.transform.position.x + detourDis - me.position.x;
			dx = frontAlly.transform.position.z - me.position.z;
			radian = Mathf.Atan2 (dy, dx);
			detourbool = false;
		}
		i += Time.deltaTime;
		radi = (radian * Mathf.Rad2Deg) * i;
		gameObject.transform.eulerAngles = new Vector3(0, radi, 0);
		transform.Translate (transform.forward * (tgtDis.z * speed * Time.deltaTime));
		myPos = transform.position;
		if (i >= 2) {
			i = 0;
			state = "move";
			gameObject.tag = "Player";
		}
//		if (detourDis > 0) {
//			if (myPos.x >= savePos.x + detourDis + 1) {
//				state = "move";
//				gameObject.tag = "Player";
//				tgtDis.x = myPos.x;
//			} else {
//				myPos = myPos + (tgtDis * speed * Time.deltaTime);//移動先へ移動
//				transform.position = myPos;//変更された変数を自分のポジションへ代入
//			}
//		} else {
//			if (myPos.x <= savePos.x + detourDis + 1) {
//				state = "move";
//				gameObject.tag = "Player";
//				tgtDis.x = myPos.x;
//			} else {
//				myPos = myPos + (tgtDis * speed * Time.deltaTime);//移動先へ移動
//				transform.position = myPos;//変更された変数を自分のポジションへ代入
//
//			}
//		}
	}

	public static Vector3 distance (Vector3 target, Vector3 me)
	{
		Vector3 dis = target - me;

		return dis;
	}

	private IEnumerator attack ()
	{
		tgt.GetComponent<Red> ().HP -= 30;
		//GameObject myHPBar = GameObject.Find (gameObject.name + ("hp(Clone)"));
//		myHPBar.transform.localScale -= new Vector3 (0.15f, 0, 0);
		attackSpace = false;
		yield return new WaitForSeconds (3);
		attackSpace = true;
	}

	private void changeAttack (GameObject obj)
	{
		if (tgt.GetComponent<Light> ().color == Color.yellow)
			tgt.GetComponent<Light> ().enabled = false;
		if (tgt.layer == 9)
			tgt.GetComponent<Red> ().atEnemys.Remove (gameObject);
		tgt = obj;
		if (atEnemys.Find (delegate(GameObject ob) {
			return ob.name == tgt.name;
		}))
			attackObj = tgt;
		else {
			state = "move";
			tag = "Player";
		}
		tgtDis = distance (tgt.transform.position, myPos);
		tgtDis = tgtDis.normalized;
	}

	private void Death ()
	{
		lightup = false;
		if (gameObject == Player.rayobj)
			Player.rayobj = null;
		if (tgt.layer == 9)
			tgt.GetComponent<Red> ().atEnemys.Remove (gameObject);
		tgt.GetComponent<LineRenderer > ().SetVertexCount (tgt.GetComponent<Red> ().atEnemys.Count + 2);
		Player.charaDestroy (gameObject);
		foreach (GameObject enemy in atEnemys) {//自分を狙っている敵
			Red script = enemy.GetComponent<Red> ();//敵のスクリプトを入手
			if (script.atEnemys.Count == 0) {//敵を狙っている味方がいなければ
				script.state = "move";//敵の状態をmove
				script.tgt = GameObject.Find ("summonBlue");//敵のターゲットを自陣に
				enemy.tag = "Enemy";//敵のタグを一般に
			} else
				script.tgt = script.atEnemys [0];
			
		}
		if (Player.saveChara == gameObject)
			Player.saveChara = null;
		UIHP.targets.Remove (gameObject.transform);
		Destroy (GameObject.Find (gameObject.name + "hp(Clone)"));
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}