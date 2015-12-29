using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Red : MonoBehaviour
{
	public GameObject tgt;//攻撃先
	public Vector3 tgtDis;//攻撃先までの距離

	public int HP;//自分のHP
	private Vector3 myPos;//自分の座標
	private float detourDis;//迂回する距離
	private readonly int speed = 10;//移動速度
	public string state;//自分の状態
	public GameObject frontAlly = null , saveFrontAlly = null;
	public static List<GameObject> allys = new List<GameObject> ();//味方のリスト
	public List<GameObject> atEnemys = new List<GameObject> ();//自分に攻撃してる敵のリスト
	private bool right;//迂回時の方向
	private bool attackActivator = true;//攻撃のクールタイムが終了しているか
	public LineRenderer linerend;//ラインレンダラー
	public bool lightup = false;
	public  GameObject attackObj;
	public bool detourbool = false;
	float dx ,dy, radian=1f,radi = 0f, i = 0;

	void Start ()
	{
		tgt = GameObject.Find ("summonBlue");//移動先
		linerend = GetComponent<LineRenderer> ();//LineRendererコンポーネントを変数に
		myPos = transform.position;//自分のポジションを入れる
		if (transform.localScale == new Vector3 (6, 6, 6)) 
			gameObject.name = ("RedSoldier" + EnemyControl.servantCount); //名前に味方召喚数の変数を付随させる
		 else if (transform.localScale == new Vector3 (10, 10, 10))
			gameObject.name = ("RedWitch" + EnemyControl.servantCount); //名前に味方召喚数の変数を付随させる
		else if (transform.localScale == new Vector3 (8, 8, 8))
			gameObject.name = ("RedGuard" + EnemyControl.servantCount); //名前に味方召喚数の変数を付随させる
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
			myPos.z = myPos.z + (tgtDis.z * speed * Time.deltaTime);//移動先へ移動
			if (myPos.x >= tgt.transform.position.x+2 || myPos.x <= tgt.transform.position.x-2){
				myPos.x = myPos.x - (tgtDis.x * (Time.deltaTime / (speed * (myPos.z / tgtDis.z))));//縦軸一定範囲まで移動
		}
			transform.position = myPos;//変更された変数を自分のポジションへ代入
			break;
		case "detour"://迂回であれば
			detour ();//迂回を開始する
			break;
		case "fight"://戦闘中であれば
			try{
				transform.LookAt (tgt.transform);//敵に注目
			}catch{
				tgt = GameObject.Find ("summmonRed");
				state = "move";
			}
			if (attackActivator) //攻撃のクールタイムが終わっていれば
				StartCoroutine (attack ()); //攻撃
			break;
		default :
			break;
		}
		if (HP < 0)
			Death ();//HPがゼロになっていたら死亡
		if (lightup) {
			linerend.SetPosition(0, transform.position);
			linerend.SetPosition (1, tgt.transform.position);
			if (tgt.GetComponent<Light> ().enabled == false) {
				tgt.GetComponent<Light> ().enabled = true;
			}
			if (attackObj!=tgt && tgt.GetComponent<Light> ().color != Color.yellow) {
				tgt.GetComponent<Light> ().color = Color.yellow;
			}else if (attackObj == tgt && tgt.GetComponent<Light> ().color == Color.red && atEnemys.Count == 0){
				tgt.GetComponent<Light> ().color = Color.blue;
			}
			if (gameObject.GetComponent<Light> ().color != Color.red) {
				gameObject.GetComponent<Light> ().color = Color.red;
			}
			for (int i = 0; i < atEnemys.Count; i++) {
				if (atEnemys [i].GetComponent<Light> ().enabled == false) {
					atEnemys [i].GetComponent<Light> ().enabled=true;
					atEnemys [i].GetComponent<Light> ().color = Color.blue;
					if (atEnemys[i].name == tgt.name) {
						atEnemys [i].GetComponent<Light> ().color = Color.red;
					}
				}
				linerend.SetVertexCount (2+((i+1)*2));
				linerend.SetPosition (2+((i+1)*2-2), transform.position);
				linerend.SetPosition (2+((i+1)*2-1), atEnemys[i].transform.position);
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		try{
		if (tgt.layer == 10 && tgt != col.gameObject) {
			return;
		}
		}catch{
			return;
		}

		switch (col.gameObject.tag) {
		case "Player":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				Blue script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					tgt = col.gameObject;
					attackObj = tgt;
					state = "fight";
					gameObject.tag = "StopEnemy";
				}
			}
			break;
		case "StopPlayer":
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				Blue script = col.gameObject.GetComponent<Blue> ();
				if (script.atEnemys.Count < 3) {
					script.atEnemys.Add (gameObject);
					tgt = col.gameObject;
					attackObj = tgt;
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
		if (col.gameObject == tgt)
			col.gameObject.GetComponent<Light> ().color = Color.red;
	}

	public void detourReady ()
	{
		if (saveFrontAlly != frontAlly) { 
			saveFrontAlly = frontAlly;
			detourDis = right ? saveFrontAlly.transform.localScale.x+3 : -(saveFrontAlly.transform.localScale.x+3);
			state = "detour";
			detourbool = true;
			detour();
		}
	}

	private void detour ()
	{
		if (detourbool) {
			dy = frontAlly.transform.position.x + detourDis - gameObject.transform.position.x;
			dx = frontAlly.transform.position.z - gameObject.transform.position.z;
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
		attackActivator = false;
		yield return new WaitForSeconds (3);
		attackActivator = true;
	}
	private void changeAttack(GameObject obj){
		if (tgt.GetComponent<Light> ().color == Color.yellow)
			tgt.GetComponent<Light> ().enabled = false;
		if (tgt.layer == 10){
			tgt.GetComponent<Blue> ().atEnemys.Remove (gameObject);
			tgt = obj;
			tgtDis = distance (tgt.transform.position, myPos);
		}

	}
	private void Death(){
		lightup = false;
		if (gameObject == Player.rayobj)
			Player.rayobj = null;
		if(tgt.layer == 10)
		tgt.GetComponent<Blue> ().atEnemys.Remove (gameObject);
		tgt.GetComponent<LineRenderer > ().SetVertexCount (tgt.GetComponent<Blue> ().atEnemys.Count+2);
		Player.charaDestroy (gameObject);
		foreach (GameObject enemy in atEnemys) {
			Blue script = enemy.GetComponent<Blue> ();
			if (script.atEnemys.Count == 0) {
				script.state = "move";
				script.tgt = GameObject.Find("summonRed");
				enemy.tag = "Player";
			} else
				script.tgt = script.atEnemys [0];
		}
		if (Player.saveChara == gameObject)
			Player.saveChara = null;
		UIHP.targets.Remove (gameObject.transform);
		Destroy(GameObject.Find (gameObject.name + "hp(Clone)"));
		summonsServant.sp += 10;
		Destroy (gameObject);

	}
}