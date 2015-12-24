using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Red : MonoBehaviour
{
	public GameObject tgt;//攻撃先
	public Vector3 tgtDis;//攻撃先までの距離

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
	public Blue script;//敵スクリプト
	private bool attackSpace = true;//攻撃のクールタイムが終了しているか
	public LineRenderer renderer;//ラインレンダラー
	public bool lightup = false;

	void Start ()
	{
		renderer = GetComponent<LineRenderer> ();//LineRendererコンポーネントを変数に
		tgt = GameObject.Find ("summonBlue");//移動先!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		myPos = transform.position;//自分のポジションを入れる
		if (transform.localScale == new Vector3 (6, 6, 6)) {
			gameObject.name = ("RedSoldier" + summonsServant.servantCount); //名前に味方召喚数の変数を付随させる
		} else if (transform.localScale == new Vector3 (10, 10, 10)) {
			gameObject.name = ("redWitch" + summonsServant.servantCount); //名前に味方召喚数の変数を付随させる
		}
		tgtDis = distance (tgt.transform.position, myPos);//移動先の座標と自分の座標の差分を図り、変数にいれる
		tgtDis = tgtDis.normalized;
		state = "move";//初期状態を移動にする
		HP = 200;//初期体力は200
		right = Random.value > 0.5f ? true : false;//迂回時の左右方向を50%で分けてる
		//UIHP.targets.Add (gameObject.transform);//召喚したオブジェクトをHP表示するオブジェクトのリストに入れる

	}

	// Update is called once per frame
	void Update ()
	{
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
			if (tgt == null) {
				state = "move";
				break;
			}
			transform.LookAt (tgt.transform);//敵に注目
			if (attackSpace) {//攻撃のクールタイムが終わっていれば
				StartCoroutine (attack ()); //攻撃
			}
			break;
		default :
			break;
		}
		if (HP < 0) {
			Death ();//HPがゼロになっていたら死亡
		}
		if (lightup) {
			renderer.SetPosition(0, transform.position);
			renderer.SetPosition (1, tgt.transform.position);

			for (int i = 0; i < atEnemys.Count; i++) {
				if (atEnemys [i].GetComponent<Light> ().enabled == false) {
					atEnemys [i].GetComponent<Light> ().enabled=true;
					atEnemys [i].GetComponent<Light> ().color = Color.red;
					if (atEnemys[i].name == tgt.name) {
						Debug.Log (atEnemys [i].GetComponent<Light> ().color);
						atEnemys [i].GetComponent<Light> ().color = Color.blue;
					}
				}
				renderer.SetVertexCount (2+((i+1)*2));
				renderer.SetPosition (2+((i+1)*2-2), transform.position);
				renderer.SetPosition (2+((i+1)*2-1), atEnemys[i].transform.position);
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		switch (col.gameObject.tag) {
		case "Player":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					tgt = col.gameObject;
					state = "fight";
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "StopPlayer":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					tgt = col.gameObject;
					state = "fight";
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "summonBlue":
			state = "fight";
			gameObject.tag = "StopEnemy";
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
				gameObject.tag="Enemy";
				tgtDis.x = myPos.x;
			} else {
				myPos.x += (detourDis * (Time.deltaTime * 3));
				transform.position = myPos;
			}
		} else {
			if (myPos.x <= savePos.x + detourDis+1) {
				state = "move";
				gameObject.tag="Enemy";
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
		tgt.GetComponent<Blue>().HP -= 30;
		GameObject myHPBar = GameObject.Find (tgt.name + ("hp(Clone)"));
		myHPBar.transform.localScale -= new Vector3 (0.15f, 0, 0);
		attackSpace = false;
		yield return new WaitForSeconds (3);
		attackSpace = true;
	}
	private void changeAttack(GameObject obj){
		if(tgt.layer==10)
			tgt.GetComponent<Blue> ().atEnemys.Remove (gameObject);
		tgt = obj;

	}
	private void Death(){
		lightup = false;
		if (gameObject == Player.rayobj)
			Player.rayobj = null;
		tgt.GetComponent<Blue> ().atEnemys.Remove (gameObject);
		tgt.GetComponent<LineRenderer > ().SetVertexCount (tgt.GetComponent<Blue> ().atEnemys.Count+2);
		Player.charaDestroy (gameObject);
		foreach (GameObject enemy in atEnemys) {
			script = enemy.GetComponent<Blue> ();
			if (script.atEnemys.Count == 0) {
				script.state = "move";
				script.tgt = GameObject.Find("summonRed");
				enemy.tag = "Player";
			} else {
				script.tgt = script.atEnemys [0];
			}
		}
		if (Player.saveChara == gameObject)
			Player.saveChara = null;
		UIHP.targets.Remove (gameObject.transform);
		Destroy(GameObject.Find (gameObject.name + "hp(Clone)"));
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}