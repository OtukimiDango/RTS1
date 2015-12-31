using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyControl : MonoBehaviour
{
	public	static	sbyte servantCount = 0;

	private	bool	summonActivater = true;

	public	static	int sp = 50;

	public	sbyte	UpSp = 1;

	private	static 	Transform spawnPoint;
	public	static 	IEnumerator coroutine;
	public static 	List<GameObject> AllEnemy = new List<GameObject> ();
	public static	List<GameObject> AllySoldier = new List<GameObject> ();
	public static	List<GameObject> AllyWitch = new List<GameObject> ();
	public static 	List<GameObject> AllyGuard = new List<GameObject> ();
	public static	Queue<int> RecordCount = new Queue<int> ();
	public static	List<GameObject> EnemySoldier = new List<GameObject> ();
	public static	List<GameObject> EnemyWitch = new List<GameObject> ();
	public static 	List<GameObject> EnemyGuard = new List<GameObject> ();
	private	static	string	summonState = "NeedSoldier";
	private static byte level = 1;
	// Use this for initialization
	void Start ()
	{
		coroutine	=	spawnPoints ();
		StartCoroutine (spUp ());
		StartCoroutine (gameTime ());
		StartCoroutine (Brain ());
		StartCoroutine (summon());
	}

	private IEnumerator spUp ()
	{
		while (true) {
			yield return new WaitForSeconds (1);//１秒待つ
			sp += UpSp;//spをプラス
		}
	}

	private IEnumerator summonServant (string s, int cost, float hpPlus)
	{
		sp -= cost;//spからコストを引く
		summonActivater = false;//召喚不可能にする
		servantCount++;//召喚したサーヴァントの数をプラス
		coroutine.MoveNext ();//召喚ポジションを移動
		GameObject servant = (GameObject)Resources.Load ("Servents/" + s);//召喚するオブジェクトを変数に入れる
		Vector3 summonPosition = spawnPoint.position;//召喚時の初期座標を変数に入れる
		summonPosition.y = summonPosition.y + servant.transform.localScale.y/2;//初期座標y軸に召喚するオブジェクトの半径をプラス
		GameObject sa = (GameObject)Instantiate ((GameObject)Resources.Load ("Servents/" + s), summonPosition, spawnPoint.rotation);//召喚
		servant.name = (s + servantCount);//召喚するオブジェクトを召喚数を付随させた名前にす

		GameObject hp = (GameObject)Resources.Load ("HPbar");//召喚したオブジェクトに付随させるHPを変数に入れる
		hp.name = (servant.name + "hp");//付随するオブジェクトとの組み合わせをわかりやすくするために名前にカウントをつける;
		hp.transform.localScale = new Vector3 (hpPlus, 0.15f, 0.1f);//HP量に合わせてバーの長さを変更
		GameObject hpbar = (GameObject)Instantiate (hp, Vector3.zero, Quaternion.identity);//HPバーをHierarchyに
		hpbar.transform.SetParent (GameObject.Find ("Canvas").transform, false);//HPバーの親オブジェクトをCanvasにしてUI表示する
		hpbar.transform.position = Camera.main.WorldToScreenPoint (summonPosition);

		switch (s) {
		case ("RedSoldier"):
			AllySoldier.Add (sa);
			break;
		case ("RedWitch"):
			AllyWitch.Add (sa);
			break;
		case("RedGuard"):
			AllyGuard.Add (sa);
			break;
		}
		yield return new WaitForSeconds (1);//1秒待つ
		summonActivater = true;//召喚可能にする

	}

	private IEnumerator gameTime ()
	{
		yield return new WaitForSeconds (30);//３０秒待つ
		UpSp++;//SPをプラス

		yield return new WaitForSeconds (60);//6０秒待つ
		UpSp++;//SPをプラス

		yield return new WaitForSeconds (120);//12０秒待つ
		UpSp++;//SPをプラス
		//最終ステージまで210秒
	}



	public IEnumerator spawnPoints ()
	{
		spawnPoint = GameObject.Find ("spawnPointAred").transform;//召喚一A
		yield return null;
		spawnPoint = GameObject.Find ("spawnPointBred").transform;//召喚一B
		yield return null;
		spawnPoint = GameObject.Find ("spawnPointCred").transform;//召喚一C
		coroutine = spawnPoints ();//最初からやり直し
		yield return null;
	}

	public IEnumerator Brain ()
	{

		while (true) {
			yield return new WaitForSeconds (level);//状況に応じてサボる

			int soldier = AllySoldier.Count;
			int witch = AllyWitch.Count;
			int guard = AllyGuard.Count;
			int ans = Mathf.Min (soldier, witch);
			int ans2 = Mathf.Min (ans, guard);

			if (ans2 == soldier) {
				summonState = "NeedSoldier";
			} else if (ans2 == witch) {
				summonState = "NeedWitch";
			} else if (ans2 == guard) {
				summonState = "NeedGuard";
			}

			AllEnemy.Clear ();
			EnemySoldier.Clear ();
			EnemyWitch.Clear ();
			EnemyGuard.Clear ();

			AllEnemy.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
			AllEnemy.AddRange (GameObject.FindGameObjectsWithTag ("StopPlayer"));

			foreach (GameObject ec in AllEnemy) {
				switch ((int)ec.transform.localScale.x) {
				case(6):
					EnemySoldier.Add (ec);
					break;
				case(8):
					EnemyGuard.Add (ec);
					break;
				case(10):
					EnemyWitch.Add (ec);
					break;
				}
			}

			RecordCount.Enqueue (AllEnemy.Count);
			if (RecordCount.Count > 3) {
				float i = ((float)AllEnemy.Count / (float)RecordCount.Dequeue ());//15秒前と現在の敵の数の上昇率
				if (i >= 1.2) {
					level = 3;
				}
			}//上昇率が120%以上になるとレベル上昇して召喚間隔と思考回転間隔が短くなる
			try{
				if(AllyGuard.Count+AllyWitch.Count+AllySoldier.Count>AllEnemy.Count	&& AllyGuard.Count+AllyWitch.Count+AllySoldier.Count/AllEnemy.Count >= 1.2f){
				level = 4;
				}//自分の味方の数が敵の数より1.2倍なら召喚しない
				else if(AllyGuard.Count+AllyWitch.Count+AllySoldier.Count<AllEnemy.Count	&& AllEnemy.Count/AllyGuard.Count+AllyWitch.Count+AllySoldier.Count >= 1.5f){
					level = 1;
				}
			}catch{

			}
		}
	}

	private IEnumerator summon ()
	{
		while (true) {
			if (level != 4) {
				Debug.Log ("true");
				if (summonState == "NeedSoldier") {
					if (summonActivater && sp > 9 && AllySoldier.Count < 10) {
						StartCoroutine (summonServant ("RedSoldier", 5, 1f));
					}
				} else if (summonState == "NeedWitch") {
					if (summonActivater && sp > 9 && AllyWitch.Count < 10) {
						StartCoroutine (summonServant ("RedWitch", 5, 1f));
					}
				} else if (summonState == "NeedGuard") {
					if (summonActivater && sp > 9 && AllyGuard.Count < 10) {
						StartCoroutine (summonServant ("RedGuard", 5, 1f));
					}
				}
				yield return new WaitForSeconds (level);
			}
			yield return null;
		}
	}
}