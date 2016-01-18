using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class summonsServant : MonoBehaviour {

	public static bool summonSpace = true;

	private static Transform spawnPoint;
	public static IEnumerator coroutine;
	// Use this for initialization
	void Awake () {
		coroutine = spawnPoints();
		StartCoroutine (updateGame());
	}

	private IEnumerator updateGame(){
		while (!GameManager._isGameOver) {
			if(Input.GetKeyDown(KeyCode.Alpha1) && summonSpace && GameManager.sp > 9 && GameManager.WarriorCount< 10){
				GameManager.WarriorCount++;
				StartCoroutine(summonServant("BlueWarrior",10,1f));
			}
			if(Input.GetKeyDown(KeyCode.Alpha2) && summonSpace && GameManager.sp > 19 && GameManager.WitchCount < 10){
				GameManager.WitchCount++;
				StartCoroutine(summonServant("BlueWitch",20,1f));
			}
			if(Input.GetKeyDown(KeyCode.Alpha3) && summonSpace && GameManager.sp > 19 && GameManager.GuardCount < 10){
				GameManager.GuardCount++;
				StartCoroutine(summonServant("BlueGuard",15,1f));
			}
			yield return null;
		}
	}

	private IEnumerator summonServant(string s, int cost,float hpPlus){
		summonSpace = false;
		AudioSource[] source;
		source =  GameObject.Find("Main Camera").GetComponents<AudioSource> ();
		source[1].clip = (AudioClip)Resources.Load ("Sound/summon");

		GameManager.sp -= cost;//spからコストを引く
		coroutine.MoveNext ();
		GameObject servant = (GameObject)Resources.Load ("Servents/" + s);//召喚するオブジェクトを変数に入れる
		Vector3 summonPosition = spawnPoint.position;//召喚時の初期座標を変数に入れる
		summonPosition.y = summonPosition.y + servant.transform.localScale.y/2;//初期座標y軸に召喚するオブジェクトの半径をプラス
		Instantiate((GameObject)Resources.Load ("Servents/" + s),summonPosition,spawnPoint.rotation);//召喚
		source[1].Play();
		servant.name = (s+GameManager.ServantCount);//召喚するオブジェクトを召喚数を付随させた名前にする

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
		spawnPoint.transform.FindChild ("Particle System").GetComponent<ParticleSystem> ().Play ();
		yield return null;
		spawnPoint = GameObject.Find("spawnPointB").transform;
		spawnPoint.transform.FindChild ("Particle System").GetComponent<ParticleSystem> ().Play ();
		yield return null;
		spawnPoint = GameObject.Find("spawnPointC").transform;
		spawnPoint.transform.FindChild ("Particle System").GetComponent<ParticleSystem> ().Play ();
		coroutine = spawnPoints ();
		yield return null;
	}
}