using UnityEngine;
using System.Collections;

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
			yield return new WaitForSeconds(1);
			sp += UpSp;
		}
	}
	private IEnumerator summonServent(string s, int stuff){
		sp -= stuff;
		summonSpace = false;
		GameObject servent = (GameObject)Resources.Load ("Servents/" + s);
		Vector3 summonPosition = spawnPoint.transform.position;
		summonPosition.y = summonPosition.y + servent.transform.position.y;
		Object.Instantiate(servent,summonPosition,spawnPoint.transform.rotation);
		servantCount++;
		yield return new WaitForSeconds(2f);
		summonSpace = true;

	}
	void OnGUI() {
		GUI.Label (new Rect (0, 0, 100, 30), "sp : "+sp);
		GUI.Label (new Rect (100, 0, 100, 30), "Servants : " + servantCount);
		GUI.Label(new Rect(200,0,100,30),"Time : "+(int)Time.time);
	}
	private static IEnumerator gameTime(){
		yield return new WaitForSeconds (30);
		summonsServant.UpSp++;

		yield return new WaitForSeconds (60);
		summonsServant.UpSp++;

		yield return new WaitForSeconds (120);
		summonsServant.UpSp++;
	}
}