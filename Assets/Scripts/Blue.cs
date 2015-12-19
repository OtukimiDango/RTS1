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
	private static int count = 0;//生成されるオブジェクトの名前に付随する変数
	public GameObject attackEnemy;//攻撃している敵
	public LineRenderer renderer;

	void Start ()
	{
		renderer = GetComponent<LineRenderer> ();//LineRendererコンポーネントを変数に
		tgt = GameObject.Find ("summonRed");//移動先!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		myPos = transform.position;//自分のポジションを入れる
		count++;//各キャラクターを区別するための変数を生成時にプラス
		gameObject.name = "BlueSoldier" + count;//名前に上記の変数を付随させる
		tgtDis = distance (tgt.transform.position, myPos);//移動先の座標と自分の座標の差分を図り、変数にいれる
		tgtDis = tgtDis.normalized;
		state = "move";//初期状態を移動にする
		HP = 200;//初期体力は200
		right = Random.value > 0.5f ? true : false;//迂回時の左右方向を50%で分けてる
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
			if (myPos.x >= 1 || myPos.x <= -1) {
				myPos.x = myPos.x - (tgtDis.x * (Time.deltaTime / (speed * (myPos.z / tgtDis.z))));//縦軸一定範囲まで移動
			}
			transform.position = myPos;//変更された変数を自分のポジションへ代入
			break;
		case "detour"://迂回であれば
			detour ();//迂回を開始する
			break;
		case "fight"://戦闘中であれば
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
		attackSpace = false;
		yield return new WaitForSeconds (3);
		attackSpace = true;
	}

	private void death ()
	{
		if (attackEnemy != null) {
			attackEnemy.GetComponent<Red> ().atEnemys.Remove (gameObject);
			if(attackEnemy.GetComponent<Red>().atEnemys.Count != 0){
				Debug.Log ("aaaa");
			attackEnemy.GetComponent<Red> ().attackEnemy = attackEnemy.GetComponent<Red> ().atEnemys[0];
			}
		}

		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<Red> ();
			script.atEnemys.Remove (gameObject);
			script.state = "move";
			script.attackEnemy = null;
			enemy.tag = "Enemy";
		}
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}