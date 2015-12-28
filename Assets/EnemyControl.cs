using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyControl : MonoBehaviour
{
	public	static	sbyte servantCount = 0;

	private	bool	summonActivater = true;

	public	static	int sp = 500;

	public	sbyte	UpSp = 1;

	private	static 	Transform spawnPoint;
	public	static 	IEnumerator coroutine;
	public static 	List<GameObject> AllEnemy = new List<GameObject> ();
	public static	List<GameObject> AllySoldier = new List<GameObject> ();
	public static	List<GameObject> AllyWitch = new List<GameObject> ();
	public static 	List<GameObject> AllyGuard = new List<GameObject> ();
	public static	List<GameObject> EnemySoldier = new List<GameObject> ();
	public static	List<GameObject> EnemyWitch = new List<GameObject> ();
	public static 	List<GameObject> EnemyGuard = new List<GameObject> ();
	private	static	string	summonState = "normal";
	// Use this for initialization
	void Start ()
	{
		coroutine	=	spawnPoints ();
		StartCoroutine(spUp ());
		StartCoroutine(gameTime ());
		StartCoroutine(Brain ());
	}
	
	// Update is called once per frame

	void Update ()
	{
		if (summonState == "NeedSoldier") {
			if (summonActivater && sp > 9 && AllySoldier.Count < 10) {
				StartCoroutine (summonServant ("RedSoldier", 10, 1f));
			}
		} else if (summonState == "NeedWitch") {
			if (summonActivater && sp > 19 && AllyWitch.Count < 10) {
				StartCoroutine (summonServant ("RedWitch", 20, 1f));
			}
		} else if (summonState == "NeedGuard") {
			if (summonActivater && sp > 19 && AllyGuard.Count < 10) {
				StartCoroutine (summonServant ("RedGuard", 15, 1f));
			}
		}

		if (summonState == "normal") {
			if (summonActivater && sp > 9 && AllySoldier.Count < 10) {
				StartCoroutine (summonServant ("RedSoldier", 10, 1f));
			}
			if (summonActivater && sp > 19 && AllyWitch.Count < 10) {
				StartCoroutine (summonServant ("RedWitch", 20, 1f));
			}
			if (summonActivater && sp > 19 && AllyGuard.Count < 10) {
				StartCoroutine (summonServant ("RedGuard", 15, 1f));
			}
		}
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
		coroutine.MoveNext ();
		GameObject servant = (GameObject)Resources.Load ("Servents/" + s);//召喚するオブジェクトを変数に入れる
		Vector3 summonPosition = spawnPoint.position;//召喚時の初期座標を変数に入れる
		summonPosition.y = summonPosition.y + servant.transform.position.y;//初期座標y軸に召喚するオブジェクトの半径をプラス
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
		yield return new WaitForSeconds (1);//2秒待つ
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
		spawnPoint = GameObject.Find ("spawnPointAred").transform;
		yield return null;
		spawnPoint = GameObject.Find ("spawnPointBred").transform;
		yield return null;
		spawnPoint = GameObject.Find ("spawnPointCred").transform;
		coroutine = spawnPoints ();
		yield return null;
	}

	public IEnumerator Brain ()
	{
		while (true) {
			int soldier = AllySoldier.Count;
			int witch = AllyWitch.Count;
			int guard = AllyGuard.Count;
			int ans = Mathf.Min (soldier, witch);
			int ans2 = Mathf.Min (ans, guard);
			if (ans2 == soldier)
				summonState = "NeedSoldier";
			else if (ans2 == witch)
				summonState = "NeedWitch";
			else if (ans2 == guard)
				summonState = "NeedGuard";
			AllEnemy.Clear ();
			EnemySoldier.Clear ();
			EnemyWitch.Clear ();
			EnemyGuard.Clear ();
			AllEnemy.AddRange (GameObject.FindGameObjectsWithTag("Player"));
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
			Debug.Log ("All : "+AllEnemy.Count+"\nSoldier : "+EnemySoldier.Count+"\nWitch : "+EnemyWitch.Count+"\nGuard"+EnemyGuard.Count);
			yield return new WaitForSeconds (3);
		}
	}
}
