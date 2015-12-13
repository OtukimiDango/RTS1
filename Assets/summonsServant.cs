using UnityEngine;
using System.Collections;

public class summonsServant : MonoBehaviour {
	public GameObject Soldier;

	private int servantCount  = 0;

	private bool summonSpace = true;

	public static int sp = 50;

	private GameObject spawnPoint;
	// Use this for initialization
	void Start () {
		StartCoroutine (spUp ());
		spawnPoint = GameObject.Find("spawnPoint");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Alpha1) && summonSpace && sp > 9){
			StartCoroutine(summonServent(Soldier));
		}
	}
	private IEnumerator spUp(){
		while (true) {
			yield return new WaitForSeconds(1);
			sp++;
		}
	}
	private IEnumerator summonServent(GameObject s){
		sp -= 10;
		summonSpace = false;
		Object.Instantiate(s,spawnPoint.transform.position,spawnPoint.transform.rotation);
		servantCount++;
		yield return new WaitForSeconds(2.5f);
		summonSpace = true;

	}
	void OnGUI() {
		GUI.Label (new Rect (0, 0, 100, 30), "sp : "+sp);
		GUI.Label (new Rect (100, 0, 100, 30), "Servants : " + servantCount);
	}
}
