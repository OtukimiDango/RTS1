using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class crystal : MonoBehaviour {
	public int HP;
	private readonly byte power = 90;
	public List<GameObject> nearEnemy = new List<GameObject>();
	public Warrior script;
	// Use this for initialization
	void Start () {
		HP = 2000;
		GameObject hp = (GameObject)Resources.Load ("HPbar");//召喚したオブジェクトに付随させるHPを変数に入れる
		hp.name = (gameObject.name+"hp");//付随するオブジェクトとの組み合わせをわかりやすくするために名前にカウントをつける;
		hp.transform.localScale = new Vector3(10f,0.15f,0.1f);//HP量に合わせてバーの長さを変更
		GameObject hpbar = (GameObject)Instantiate (hp,Vector3.zero,Quaternion.identity);//HPバーをHierarchyに
		hpbar.transform.SetParent(GameObject.Find ("Canvas").transform,false);//HPバーの親オブジェクトをCanvasにしてUI表示する
		hpbar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		UIHP.targets.Add (gameObject.transform);//召喚したオブジェクトをHP表示するオブジェクトのリストに入れる
	}

	// Update is called once per frame
	void Update () {
		if (HP <= 0) {
			Death ();
		}			
	}
	void OnTriggerEnter(Collider col){
		if(col.gameObject.layer == 9){
			nearEnemy.Add (col.gameObject);
			if(nearEnemy.Count == 1){
				StartCoroutine (attack());
			}
		}
	}
	public virtual IEnumerator attack(){
		GameObject attackTgt = gameObject;
		GameObject myHPBar;
		while (nearEnemy.Count != 0) {
			try{
				attackTgt = nearEnemy[0];
				enemyScript(attackTgt);
				script.HP -= power;
				myHPBar = GameObject.Find (attackTgt.name + ("hp(Clone)"));
				myHPBar.transform.localScale -= new Vector3 (0.45f, 0, 0);
			}catch{
				nearEnemy.RemoveAt (0);
			}
			yield return new WaitForSeconds (1);
		}
	}
	public void enemyScript(GameObject e){
		script = e.GetComponent<Warrior> ();
	}
	public virtual void Death(){
		Destroy(GameObject.Find (gameObject.name + "hp(Clone)"));
		UIHP.targets.Remove (gameObject.transform);
		Destroy (gameObject);
	}

}