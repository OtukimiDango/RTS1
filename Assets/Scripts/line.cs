using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class line : MonoBehaviour {
	public GameObject lineImage;
	private GameObject lineob;
	private Vector3 p1;
	public Vector3[] pos;
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
			lineImage.GetComponent<SpriteRenderer> ().color = value;
		}
		get{
			return lineImage.GetComponent<SpriteRenderer> ().color;
		}
	}
	//	void Start(){
	//		p1 = new Vector3 (0,0,0);
	//		lineob = (GameObject)Instantiate (lineImage,new Vector3(p1.x,0.1f,p1.z),Quaternion.identity);
	//		lineImage = lineob;
	//		lineImage.GetComponent<SpriteRenderer> ().color = Color.red;
	//		lineob.transform.Rotate (90,0,0);
	//	}
	line(GameObject p1ob,Color c,bool mouse,List<GameObject> obs){
		p1 = p1ob.transform.position;
		lineImage = (GameObject)Instantiate (lineImage,new Vector3(p1.x,0.1f,p1.z),Quaternion.identity);
		lineob = p1ob;
		lineImage.GetComponent<SpriteRenderer> ().color = c;
		lineob.transform.Rotate (90,0,0);
		if (mouse)
			RayLine ();
		ObLine (obs);

	}
	private void RayLine(){
		int terrain = 1 << LayerMask.NameToLayer ("Terrain");
		while (mouseLine) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit,Mathf.Infinity,terrain)) {
				float d = (Mathf.Abs (hit.point.x - p1.x) + Mathf.Abs (hit.point.z - p1.z));
				Debug.Log (hit.point);
				float atan =  Mathf.Atan2 ((hit.point.x-p1.x),(hit.point.z - p1.z))*Mathf.Rad2Deg;
				lineob.transform.rotation = Quaternion.Euler (90, atan, 0);
				lineob.transform.localScale = new Vector3 (500f,d*70,0);
				lineob.transform.position = new Vector3 ((hit.point.x+p1.x)/2,0.1f,(hit.point.z+p1.z)/2);
			}
			return;
		}
	}
	private void ObLine(List<GameObject> obs){
		while (true) {
			foreach(GameObject ob in obs){
				try{
					Vector3 p0 = ob.transform.position;
					float d = (Mathf.Abs (p0.x - p1.x) + Mathf.Abs (p0.z - p1.z));
					Debug.Log (p0);
					float atan =  Mathf.Atan2 ((p0.x-p1.x),(p0.z - p1.z))*Mathf.Rad2Deg;
					lineob.transform.rotation = Quaternion.Euler (90, atan, 0);
					lineob.transform.localScale = new Vector3 (500f,d*70,0);
					lineob.transform.position = new Vector3 ((p0.x+p1.x)/2,0.1f,(p0.z+p1.z)/2);
				}catch{
					obs.Remove (ob);
				}
			}
			return;
		}
	}
}
