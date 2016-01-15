using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class summonsServant : MonoBehaviour {

	public static bool summonSpace = true;

	private static Transform spawnPoint;
	public static IEnumerator coroutine;
	// Use this for initialization
	void Start () {
		coroutine = spawnPoints();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1) && summonSpace && GameManager.sp > 9 && GameManager.WarriorCount< 10){
			StartCoroutine(summonServant("BlueWarrior",10,1f));
			GameManager.WarriorCount++;
		}
		if(Input.GetKeyDown(KeyCode.Alpha2) && summonSpace && GameManager.sp > 19 && GameManager.WitchCount < 10){
			StartCoroutine(summonServant("BlueWitch",20,1f));
			GameManager.WitchCount++;
		}
		if(Input.GetKeyDown(KeyCode.Alpha3) && summonSpace && GameManager.sp > 19 && GameManager.GuardCount < 10){
			StartCoroutine(summonServant("BlueGuard",15,1f));
			GameManager.GuardCount++;
		}
	}

	private IEnumerator summonServant(string s, int cost,float hpPlus){
		summonSpace = false;
		AudioSource[] source;
		source =  GameObject.Find("Main Camera").GetComponents<AudioSource> ();
		source[1].clip = (AudioClip)Resources.Load ("Sound/summon");

		GameManager.sp -= cost;//spからコストを引く
		GameManager.servantCount++;//召喚したサーヴァントの数をプラス
		coroutine.MoveNext ();
		GameObject servant = (GameObject)Resources.Load ("Servents/" + s);//召喚するオブジェクトを変数に入れる
		Vector3 summonPosition = spawnPoint.position;//召喚時の初期座標を変数に入れる
		summonPosition.y = summonPosition.y + servant.transform.localScale.y/2;//初期座標y軸に召喚するオブジェクトの半径をプラス
		Instantiate((GameObject)Resources.Load ("Servents/" + s),summonPosition,spawnPoint.rotation);//召喚
		source[1].Play();
		servant.name = (s+GameManager.servantCount);//召喚するオブジェクトを召喚数を付随させた名前にする

		GameObject hp = (GameObject)Resources.Load ("HPbar");//召喚したオブジェクトに付随させるHPを変数に入れる
		hp.name = (servant.name+"hp");//付随するオブジェクトとの組み合わせをわかりやすくするために名前にカウントをつける;
		hp.transform.localScale = new Vector3(hpPlus,0.15f,0.1f);//HP量に合わせてバーの長さを変更
		GameObject hpbar = (GameObject)Instantiate (hp,Vector3.zero,Quaternion.identity);//HPバーをHierarchyに
		hpbar.transform.SetParent(GameObject.Find ("Canvas").transform,false);//HPバーの親オブジェクトをCanvasにしてUI表示する
		hpbar.transform.position = Camera.main.WorldToScreenPoint(summonPosition);

		yield return new WaitForSeconds(1);//2秒待つ
		summonSpace = true;

	}

	public IEnumerator spawnPoints(){
		spawnPoint = GameObject.Find("spawnPointA").transform;
		yield return null;
		spawnPoint = GameObject.Find("spawnPointB").transform;
		yield return null;
		spawnPoint = GameObject.Find("spawnPointC").transform;
		coroutine = spawnPoints ();
		yield return null;
	}
}