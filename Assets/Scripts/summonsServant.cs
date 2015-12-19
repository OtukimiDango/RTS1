using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class summonsServant : MonoBehaviour {
	private int servantCount  = 0;

	private bool summonSpace = true;

	public static int sp = 500;

	public static int UpSp = 1;

	private static float GameTime = 0;

	private static int soldierCount = 0;
	private static int witchCount = 0;

	private GameObject spawnPoint;
	// Use this for initialization
	void Start () {
		StartCoroutine (spUp ());
		StartCoroutine (gameTime());

		spawnPoint = GameObject.Find("spawnPoint");
	}

	// Update is called once per frame
	void Update () {
		GameTime += Time.deltaTime;
		if(Input.GetKey(KeyCode.Alpha1) && summonSpace && sp > 9 && soldierCount < 10){
			StartCoroutine(summonServent("BlueSoldier",10));
			soldierCount++;
		}
		if(Input.GetKey(KeyCode.Alpha2) && summonSpace && sp > 19 && witchCount < 10){
			StartCoroutine(summonServent("BlueWitch",20));
			witchCount++;
		}
	}
	private IEnumerator spUp(){
		while (true) {
			yield return new WaitForSeconds(1);//１秒待つ
			sp += UpSp;//spをプラス
		}
	}
	private IEnumerator summonServent(string s, int stuff){
		sp -= stuff;//spからコストを引く
		summonSpace = false;//召喚不可能にする
		servantCount++;//召喚したサーヴァントの数をプラス
		GameObject servent = (GameObject)Resources.Load ("Servents/" + s);//召喚するオブジェクトを変数に入れる
		Vector3 summonPosition = spawnPoint.transform.position;//召喚時の初期座標を変数に入れる
		summonPosition.y = summonPosition.y + servent.transform.position.y;//初期座標y軸に召喚するオブジェクトの半径をプラス
		servent.name = (s + servantCount);//召喚するオブジェクトを召喚数を付随させた名前にする
		Object.Instantiate((GameObject)Resources.Load ("Servents/" + s),summonPosition,spawnPoint.transform.rotation);//召喚
		UIHP.targets.Add (servent.transform);//召喚したオブジェクトをHP表示するオブジェクトのリストに入れる

		GameObject hp = (GameObject)Resources.Load ("HPbar");//召喚したオブジェクトに付随させるHPを変数に入れる
		hp.name = (servent.name+"hp");//付随するオブジェクトとの組み合わせをわかりやすくするために名前にカウントをつける;
		hp.transform.localScale = new Vector3(0.4f,0.1f,0.1f);//HP量に合わせてバーの長さを変更
		GameObject hpbar = (GameObject)Instantiate (hp,Vector3.zero,Quaternion.identity);//HPバーをHierarchyに
		hpbar.transform.SetParent(GameObject.Find ("Canvas").transform,false);//HPバーの親オブジェクトをCanvasにしてUI表示する
		UIHP.HPs.Add (hpbar.transform);//オブジェクトに合わせて動くHPのリストに入れる

		yield return new WaitForSeconds(2);//2秒待つ
		summonSpace = true;//召喚可能にする

	}
	void OnGUI() {
		GUI.Label (new Rect (0, 0, 100, 30), "sp : "+sp);
		GUI.Label (new Rect (100, 0, 100, 30), "Servants : " + servantCount);
		GUI.Label(new Rect(200,0,100,30),"Time : "+(int)Time.time);
	}
	private static IEnumerator gameTime(){
		yield return new WaitForSeconds (30);//３０秒待つ
		summonsServant.UpSp++;//SPをプラス

		yield return new WaitForSeconds (60);//6０秒待つ
		summonsServant.UpSp++;//SPをプラス

		yield return new WaitForSeconds (120);//12０秒待つ
		summonsServant.UpSp++;//SPをプラス
		//最終ステージまで210秒
	}
}