using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class summonsServant : MonoBehaviour {
	public Camera maincamera;
	public static int servantCount  = 0;

	private bool summonSpace = true;

	public static int sp = 500;

	public int UpSp = 1;

	private int soldierCount = 0;
	private int witchCount = 0;
	private Transform spawnPoint;
	public IEnumerator coroutine;
	// Use this for initialization
	void Start () {
		coroutine = spawnPoints();
		StartCoroutine (spUp ());
		StartCoroutine (gameTime());

		//spawnPoint = GameObject.Find("spawnPoint");
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1) && summonSpace && sp > 9 && soldierCount < 10){
			StartCoroutine(summonServant("BlueSoldier",10,1f));
			soldierCount++;
		}
		if(Input.GetKeyDown(KeyCode.Alpha2) && summonSpace && sp > 19 && witchCount < 10){
			StartCoroutine(summonServant("BlueWitch",20,1f));
			witchCount++;
		}
	}
	private IEnumerator spUp(){
		while (true) {
			yield return new WaitForSeconds(1);//１秒待つ
			sp += UpSp;//spをプラス
		}
	}
	private IEnumerator summonServant(string s, int cost,float hpPlus){
		sp -= cost;//spからコストを引く
		summonSpace = false;//召喚不可能にする
		servantCount++;//召喚したサーヴァントの数をプラス
		coroutine.MoveNext ();
		GameObject servant = (GameObject)Resources.Load ("Servents/" + s);//召喚するオブジェクトを変数に入れる
		Vector3 summonPosition = spawnPoint.position;//召喚時の初期座標を変数に入れる
		summonPosition.y = summonPosition.y + servant.transform.position.y;//初期座標y軸に召喚するオブジェクトの半径をプラス
		Object.Instantiate((GameObject)Resources.Load ("Servents/" + s),summonPosition,spawnPoint.rotation);//召喚
		servant.name = (s+servantCount);//召喚するオブジェクトを召喚数を付随させた名前にする

		GameObject hp = (GameObject)Resources.Load ("HPbar");//召喚したオブジェクトに付随させるHPを変数に入れる
		hp.name = (servant.name+"hp");//付随するオブジェクトとの組み合わせをわかりやすくするために名前にカウントをつける;
		hp.transform.localScale = new Vector3(hpPlus,0.15f,0.1f);//HP量に合わせてバーの長さを変更
		GameObject hpbar = (GameObject)Instantiate (hp,Vector3.zero,Quaternion.identity);//HPバーをHierarchyに
		hpbar.transform.SetParent(GameObject.Find ("Canvas").transform,false);//HPバーの親オブジェクトをCanvasにしてUI表示する
		hpbar.transform.position = maincamera.WorldToScreenPoint(summonPosition);

		yield return new WaitForSeconds(1);//2秒待つ
		summonSpace = true;//召喚可能にする

	}
	void OnGUI() {
		GUI.Label (new Rect (0, 0, 100, 30), "sp : "+sp);
		GUI.Label (new Rect (100, 0, 100, 30), "Servants : " + servantCount);
		GUI.Label(new Rect(200,0,100,30),"Time : "+(int)Time.time);
	}
	private IEnumerator gameTime(){
		yield return new WaitForSeconds (30);//３０秒待つ
		UpSp++;//SPをプラス

		yield return new WaitForSeconds (60);//6０秒待つ
		UpSp++;//SPをプラス

		yield return new WaitForSeconds (120);//12０秒待つ
		UpSp++;//SPをプラス
		//最終ステージまで210秒
	}
	public IEnumerator spawnPoints(){
		spawnPoint = GameObject.Find("spawnPointA").transform;
		Debug.Log (spawnPoint.name);
		yield return null;
		spawnPoint = GameObject.Find("spawnPointB").transform;
		yield return null;
		spawnPoint = GameObject.Find("spawnPointC").transform;
		coroutine = spawnPoints ();
		yield return null;
	}
}