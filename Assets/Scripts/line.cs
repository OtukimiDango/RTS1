using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class line : MonoBehaviour {
	public GameObject lineImage;
	private List<GameObject> lineob = new List<GameObject> ();
	private Vector3 p1;
	public bool mouseLine;

	public bool enable{
		set{
			if(value == false)
				Destroy (this);
		}
		get{
			return true;
		}
	}
	public Color color{
		set{
			for(int i = -1; i < lineob.Count;i++){
				lineob[i].GetComponent<SpriteRenderer> ().color = value;
			}
		}
		get{
			return lineob[0].GetComponent<SpriteRenderer> ().color;
		}
	}
	/// <summary>
	/// Lineパラメーター初期設定
	/// </summary>
	/// <param name="i">The index.</param>
	/// <param name="c">C.</param>
	private void lineCons(int i,Color c){
		p1 = transform.position;
		lineob[i] = (GameObject)Instantiate (lineImage,new Vector3(p1.x,0.1f,p1.z),Quaternion.identity);
		lineob[i].GetComponent<SpriteRenderer> ().color = c;
		lineob[i].transform.Rotate (90,0,0);

	}
	/// <summary>
	/// Line初期設定　兵士用
	/// </summary>
	/// <param name="c">C.</param>
	/// <param name="obs">Obs.</param>
	public void setup(Color c,List<GameObject> obs){
		for(int a = -1;a<obs.Count;a++){
			lineCons (a + 1,c);
		}
		if (c==Color.blue)
			RayLine (true);
		ObLine (obs);
	}
	/// <summary>
	/// Line初期設定　範囲指定用
	/// </summary>
	/// <param name="c">C.</param>
	/// <param name="mouse">If set to <c>true</c> mouse.</param>
	public void setup(Color c){
		lineCons (0,c);
		RayLine (true);
	}
	/// <summary>
	/// Line初期設定　範囲指定後用
	/// </summary>
	/// <param name="c">C.</param>
	/// <param name="pos">Position.</param>
	public void setup(Color c,Vector3 pos){
		lineCons (0,c);
		dropLine (pos);

	}
	/// <summary>
	/// マウスの座標にLine表示する
	/// 引数はtrueで範囲移動時にライトアップさせない
	/// </summary>
	/// <param name="area">If set to <c>true</c> area.</param>
	private void RayLine(bool area){//引数は範囲移動時は攻撃対象をライトアップさせないため
		lineob[0].GetComponent<SpriteRenderer>().color = Color.yellow;
		int terrain = 1 << LayerMask.NameToLayer ("Terrain");
		while (mouseLine) {
			p1 = transform.position;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit,Mathf.Infinity,terrain)) {
				LineParameter (hit.point,p1);
			}
			int mask = (1 << LayerMask.NameToLayer ("touchChara"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask) && area == false) {//rayがキャラクターに当たる
				try {
					if (Instruction.saveChara.layer == 10 && hit.collider.gameObject.transform.parent.gameObject.layer == 9
						|| Instruction.saveChara.layer == 9 && hit.collider.gameObject.transform.parent.gameObject.layer == 10) {
						//rayが当たったキャラと以前当たったキャラが敵対していたら
						GameObject Hetgt;//rayが当たっとキャラクターのターゲット
						Hetgt = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Soldier> ().tgt;
						if (Hetgt != Instruction.saveChara) {
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().enabled = true;//当たったキャラのライトをON
							hit.collider.gameObject.transform.parent.gameObject.GetComponent<Light> ().color = Color.yellow;//黄色く光らせる
							Instruction.rayobj = hit.collider.gameObject.transform.parent.gameObject;//rayMouse内での判定用に保存
						}
					}
				} catch {
				}
			}
			return;
		}
	}
	/// <summary>
	/// 兵士の状態確認用
	/// 引数はラインの対象
	/// </summary>
	/// <param name="obs">Obs.</param>
	private void ObLine(List<GameObject> obs){//キャラクターへのLine 引数は対象リスト
		while (true) {
			p1 = transform.position;
			int i = 0;
			foreach(GameObject ob in obs){
				try{
					Vector3 p0 = ob.transform.position;
					LineParameter (p0,p1);
				}catch{
					obs.Remove (ob);
					lineob.RemoveAt(i);
				}
				i++;
			}
			i = 0;
			return;
		}
	}
	/// <summary>
	/// 範囲指定後のライン表示用
	/// 引数は指定座標
	/// </summary>
	/// <returns>The line.</returns>
	/// <param name="p0">P0.</param>
	public void dropLine(Vector3 p0){
		Vector3 pos = transform.position;
		try{
			foreach(GameObject ob in Empty.emptys){
				ob.GetComponent<Empty> ().listSerch (gameObject.GetComponent<Empty>().allys);
			}
		}catch{
		}
		name = "moveEmpty";
		Empty.emptys.Add (gameObject);
		while(Mathf.Abs(pos.x-p0.x) > 0.5f && Mathf.Abs(pos.z-p0.z) > 0.5f){
			pos = transform.position;
			LineParameter (p0,p1);
			return;
		}//目的地に着くとwhileを抜ける
		Empty.emptys.Remove (gameObject);//staticリストから自分を外す
		Destroy (gameObject);//自殺
		return;
	}

	/// <summary>
	/// Lineの動的変化メソッド
	/// 引数は原点座標と対象座標
	/// </summary>
	/// <param name="p0">P0.</param>
	/// <param name="p1">P1.</param>
	public void LineParameter(Vector3 p0,Vector3 p1){
		float d = (Mathf.Abs (p0.x - p1.x) + Mathf.Abs (p0.z - p1.z));
		float atan =  Mathf.Atan2 ((p0.x-p1.x),(p0.z - p1.z))*Mathf.Rad2Deg;
		lineob[0].transform.rotation = Quaternion.Euler (90, atan, 0);
		lineob[0].transform.localScale = new Vector3 (500f,d*70,0);
		lineob[0].transform.position = new Vector3 ((p0.x+p1.x)/2,0.1f,(p0.z+p1.z)/2);
	}
}
