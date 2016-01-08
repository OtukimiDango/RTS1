using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Red : MonoBehaviour
{
	public GameObject tgt;
//攻撃先
	public Vector3 tgtDis;
//攻撃先までの距離
	public int HP;
//自分のHP
	public string state;
//自分の状態
	public GameObject frontAlly = null;
	public static List<GameObject> allys = new List<GameObject> ();
//味方のリスト
	public List<GameObject> atEnemys = new List<GameObject> (), behindAlly = new List<GameObject> ();
//自分に攻撃してる敵のリスト
	private bool right;
//迂回時の方向
	public bool lightup = false;
	public  GameObject attackObj;
	float radian = 1f,i = 0;

	void Start ()
	{
		tgt = GameObject.Find ("blueFirstCrystal");//移動先
		if (transform.localScale == new Vector3 (6, 6, 6))
			gameObject.name = ("RedSoldier" + EnemyControl.servantCount); //名前に味方召喚数の変数を付随させる
		 else if (transform.localScale == new Vector3 (10, 10, 10))
			gameObject.name = ("RedWitch" + EnemyControl.servantCount); //名前に味方召喚数の変数を付随させる
		else if (transform.localScale == new Vector3 (8, 8, 8))
			gameObject.name = ("RedGuard" + EnemyControl.servantCount); //名前に味方召喚数の変数を付随させる
		tgtDis = distance (tgt.transform.position, transform.position);//移動先の座標と自分の座標の差分を図り、変数にいれる
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
			transform.position = new Vector3
				(transform.position.x+(tgtDis.x * 10 * Time.deltaTime),
					transform.position.y,
					transform.position.z+(tgtDis.z * 10 * Time.deltaTime));
			break;
		case "detour"://迂回であれば
			detour (10,radian);//迂回を開始する
			break;
		case "fight"://戦闘中であれば
			try {
				transform.LookAt (tgt.transform);//敵に注目
			} catch {
				tgt = GameObject.Find ("summonBlue");
				Debug.Log ("aaa");
				state = "move";
			}

			break;
		default :
			break;
		}
		if (HP < 0)
			Death ();//HPがゼロになっていたら死亡
		if (lightup) {
			var linerend = GetComponent<LineRenderer> ();//LineRendererコンポーネントを変数に
			linerend.SetPosition (0, transform.position);
			linerend.SetPosition (1, tgt.transform.position);
			try {
				if (tgt.GetComponent<Light> ().enabled == false) {
					tgt.GetComponent<Light> ().enabled = true;
				}
				if (attackObj != tgt && tgt.GetComponent<Light> ().color != Color.yellow) {
					tgt.GetComponent<Light> ().color = Color.yellow;
				} else if (attackObj == tgt && tgt.GetComponent<Light> ().color == Color.red && atEnemys.Count == 0) {
					tgt.GetComponent<Light> ().color = Color.blue;
				}
			} catch {
			}
			if (gameObject.GetComponent<Light> ().color != Color.red) {
				gameObject.GetComponent<Light> ().color = Color.red;
			}
			for (int i = 0; i < atEnemys.Count; i++) {
				if (atEnemys [i].GetComponent<Light> ().enabled == false) {
					atEnemys [i].GetComponent<Light> ().enabled = true;
					atEnemys [i].GetComponent<Light> ().color = Color.blue;
					if (atEnemys [i].name == tgt.name) {
						atEnemys [i].GetComponent<Light> ().color = Color.red;
					}
				}
				linerend.SetVertexCount (2 + ((i + 1) * 2));
				linerend.SetPosition (2 + ((i + 1) * 2 - 2), transform.position);
				linerend.SetPosition (2 + ((i + 1) * 2 - 1), atEnemys [i].transform.position);
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		try {
			if (tgt.layer == 10 && tgt != col.gameObject) {
				return;
			}
		} catch {
			return;
		}

		switch (col.gameObject.layer) {
		case 10:
			if (gameObject.CompareTag ("Enemy") && state != "fight") {//戦闘中でなければ
				Blue script = col.gameObject.GetComponent<Blue> ();//敵チームスクリプト取得
				if (script.atEnemys.Count < 3) {//敵が3人に狙われていなければ
					script.atEnemys.Add (gameObject);//敵の敵リストに自分を追加
					tgt = col.gameObject;//攻撃対象にする
					attackObj = tgt;//攻撃開始
					state = "fight";//自分の状況を戦闘中に切り替え
					try{
					behindAlly.ForEach (i => i.GetComponent<Red> ().detourReady ());
					}catch{
					}
					gameObject.tag = "StopEnemy";//自分の行動を止める
					StartCoroutine (attack (30)); //攻撃
				}
			}
			break;

		case 16:
			state = "fight";
			gameObject.tag = "StopEnemy";
			break;
		case 13:
			if (gameObject.CompareTag ("Enemy") && state != "fight") {
				tgt = col.gameObject;
				attackObj = tgt;
				state = "fight";
				try{
					behindAlly.ForEach (i => i.GetComponent<Red> ().detourReady ());
				}catch{
				}				gameObject.tag = "StopEnemy";
				StartCoroutine (attack (30)); //攻撃
			}
			break;
		default :
			break;
		}
		if (col.gameObject == tgt) {
			try {
				col.gameObject.GetComponent<Light> ().color = Color.red;
			} catch {
			}
		}
	}

	public void detourReady ()
	{
		float detourDis = 0f;
		try{
		if (right) {
			if (frontAlly.transform.position.x <= transform.position.x) {
				detourDis = transform.localScale.x / 2 + (frontAlly.transform.localScale.x / 2 - (transform.position.x - frontAlly.transform.position.x));
			}
		} else {
			detourDis =  transform.localScale.x/2-(-frontAlly.transform.localScale.x/2 - (transform.position.x - frontAlly.transform.position.x));
		}
		}catch{
			return;
		}
		state = "detour";
		float dy = frontAlly.transform.position.x + detourDis - gameObject.transform.position.x;
		float dx = frontAlly.transform.position.z - gameObject.transform.position.z;
		radian = Mathf.Atan2 (dy, dx);
		detour (10,radian);
	}

	private void detour (byte speed,float radian)
	{
		i += Time.deltaTime;
		var radi = (radian * Mathf.Rad2Deg) * i;
		gameObject.transform.eulerAngles = new Vector3 (0, radi, 0);
		transform.Translate (transform.forward * (tgtDis.z * speed * Time.deltaTime));
		if (i >= 1f) {
			i = 0;
			state = "move";
			gameObject.tag = "Enemy";
		}
	}

	public static Vector3 distance (Vector3 target, Vector3 me)
	{
		Vector3 dis = target - me;
		return dis;
	}

	private IEnumerator attack (byte power)
	{
		while (state == "fight") {
			try {
				tgt.GetComponent<Blue> ().HP -= power;
			} catch {
				tgt.GetComponent<crystal> ().HP -= power;
			}
			GameObject myHPBar = GameObject.Find (tgt.name + ("hp(Clone)"));
			myHPBar.transform.localScale -= new Vector3 (0.15f, 0, 0);
			yield return new WaitForSeconds (3);
		}
	}

	private void changeAttack (GameObject obj)
	{
		try {
			if (tgt.GetComponent<Light> ().color == Color.yellow)
				tgt.GetComponent<Light> ().enabled = false;
		} catch {
		}
		if (tgt.layer == 10) {
			tgt.GetComponent<Blue> ().atEnemys.Remove (gameObject);
			tgt = obj;
			tgtDis = distance (tgt.transform.position, transform.position);
			tgtDis = tgtDis.normalized;
		}

	}

	private void Death ()
	{
		lightup = false;
		if (gameObject == Instruction.rayobj)
			Instruction.rayobj = null;
		if (tgt.layer == 10)
			tgt.GetComponent<Blue> ().atEnemys.Remove (gameObject);
		try {
			tgt.GetComponent<LineRenderer > ().SetVertexCount (tgt.GetComponent<Blue
				> ().atEnemys.Count + 2);
		} catch {

		}
		foreach (GameObject enemy in atEnemys) {
			Blue script = enemy.GetComponent<Blue> ();
			if (script.atEnemys.Count == 0) {
				script.state = "move";
				script.tgt = GameObject.Find ("summonRed");
				enemy.tag = "Player";
			} else
				script.tgt = script.atEnemys [0];
		}
		if (Instruction.saveChara == gameObject)
			Instruction.saveChara = null;
		UIHP.targets.Remove (gameObject.transform);
		summonsServant.sp += 10;
		Instruction.charaDestroy (gameObject);
		Destroy (GameObject.Find (gameObject.name + "hp(Clone)"));
		Destroy (gameObject);

	}
}