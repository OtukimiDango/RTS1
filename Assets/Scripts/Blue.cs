using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blue : MonoBehaviour
{
	private GameObject tgt;
	public Vector3 tgtDis;

	public int HP;//自分のHP
	private Vector3 myPos;//自分の座標
	private Vector3 savePos;//自分の座標を一時的に保存する変数
	private float detourDis;//迂回する距離
	private static int speed = 10;//移動速度
	public string state;//自分の状態
	public GameObject frontAlly = null;//前方の味方
	public static List<GameObject> allys = new List<GameObject> ();//味方のリスト
	public List<GameObject> atEnemys = new List<GameObject> ();//自分に攻撃してる敵のリスト
	private GameObject saveFrontAlly;//前方の味方を判定するときにブッキングを回避するための保存用変数
	private bool right;//迂回時の方向
	public Red script;//敵スクリプト
	private bool attackSpace = true;//攻撃のクールタイムが終了しているか
	public GameObject attackEnemy;//攻撃している敵
	public LineRenderer renderer;

	void Start ()
	{
		renderer = GetComponent<LineRenderer> ();//LineRendererコンポーネントを変数に
		tgt = GameObject.Find ("summonRed");//移動先!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		myPos = transform.position;//自分のポジションを入れる
		if (transform.localScale == new Vector3 (6, 6, 6)) {
			gameObject.name = ("BlueSoldier" + summonsServant.servantCount); //名前に味方召喚数の変数を付随させる
		} else if (transform.localScale == new Vector3 (10, 10, 10)) {
			gameObject.name = ("BlueWitch" + summonsServant.servantCount); //名前に味方召喚数の変数を付随させる
		}
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
		renderer.SetPosition(0, transform.position);
		renderer.SetPosition(1, tgt.transform.position);
		switch (state) {//自分の状態を要素としswitch文
		case "move"://移動中であれば
			transform.LookAt (tgt.transform);//移動先に注目
			myPos.z = myPos.z + (tgtDis.z * speed * Time.deltaTime);//移動先へ移動
			if (myPos.x >= 10 || myPos.x <= -10) {
				myPos.x = myPos.x - (tgtDis.x * (Time.deltaTime / (speed * (myPos.z / tgtDis.z))));//縦軸一定範囲まで移動
			}
			transform.position = myPos;//変更された変数を自分のポジションへ代入
			break;
		case "detour"://迂回であれば
			detour ();//迂回を開始する
			break;
		case "fight"://戦闘中であれば
			if (attackEnemy == null) {
				state = "move";
				break;
			}
			transform.LookAt (attackEnemy.transform);//敵に注目
			if (attackSpace) {//攻撃のクールタイムが終わっていれば
				StartCoroutine (attack ()); //攻撃
			}
			break;
		default :
			break;
		}
		if (HP < 0) {
			death ();//HPがゼロになっていたら死亡メソッドをまわす
		}

	}

	void OnTriggerEnter (Collider col)
	{
		switch (col.gameObject.tag) {
		case "Enemy":
			if (gameObject.CompareTag ("Player") && state != "fight") {
				script = col.gameObject.GetComponent<Red> ();
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
				script = col.gameObject.GetComponent<Red> ();
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
		attackEnemy.GetComponent<Red>().HP -= 30;
		//		GameObject myHPBar = GameObject.Find (gameObject.name + ("hp(Clone)"));
		//		myHPBar.transform.localScale.x -= 30 / myHPBar.transform.localScale.x;
		attackSpace = false;
		yield return new WaitForSeconds (3);
		attackSpace = true;
	}

	private void death ()
	{
		if (attackEnemy != null) {
			attackEnemy.GetComponent<Red> ().atEnemys.Remove (gameObject);
		}

		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<Red> ();
			if (script.atEnemys.Count != 0) {
				Debug.Log (script.atEnemys.Count);
				script.attackEnemy = script.atEnemys [0];
			} else {
				script.state = "move";
				script.attackEnemy = null;
				enemy.tag = "Enemy";
			}
		}
		UIHP.targets.Remove (gameObject.transform);
		Destroy(GameObject.Find (gameObject.name + "hp(Clone)"));
		//UIHP.HPs.Remove (GameObject.Find (gameObject.name + "hp(Clone)").transform);
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
	private void changeAttak(){
		if (attackEnemy == null || atEnemys != null) {
			attackEnemy = atEnemys [0];
		}
	}
}