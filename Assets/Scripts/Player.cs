using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private static GameObject Playercamera;
	private static Vector3 cameraPos;
	public static string mouseState;
	public static GameObject saveChara;
	public LayerMask mask;
	// Use this for initialization
	void Start () {
		saveChara = GameObject.Find("RedSoldier1");
		//Debug.Log (saveChara.name);
		mouseState = "normal";
		Playercamera = GameObject.Find ("Camera");
		cameraPos = Playercamera.transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && mouseState == "normal") {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit,Mathf.Infinity,mask.value)){
				clickCharacter (hit.collider.gameObject);
			}
		}
		if (Input.GetKey ("d")) {
			cameraPos.z += Time.deltaTime*50;
			Playercamera.transform.position = cameraPos;
		}
		if (Input.GetKey ("a")) {
			cameraPos.z -= Time.deltaTime*50;
			Playercamera.transform.position = cameraPos;
		}
	}
	private static void OnGUI(Vector3 position){

	}
	//	void OnMouseDrag()
	//	{
	//		Vector3 objectPointInScreen
	//		= Camera.main.WorldToScreenPoint(this.transform.position);
	//
	//		Vector3 mousePointInScreen
	//		= new Vector3(Input.mousePosition.x,
	//			Input.mousePosition.y,
	//			objectPointInScreen.z);
	//
	//		Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(mousePointInScreen);
	//		mousePointInWorld.z = this.transform.position.z;
	//		this.transform.position = mousePointInWorld;
	//	}
	private static void clickCharacter(GameObject clickChara){
		GameObject parent = clickChara.transform.parent.gameObject;
		if (parent.tag == "Enemy" || parent.tag == "StopEnemy" || parent.tag == "Player" || parent.tag == "StopPlayer") {
			bool lineFlag = parent.GetComponent<LineRenderer> ().enabled;
			saveChara.GetComponent<LineRenderer> ().enabled = false;
			saveChara.GetComponent<Light> ().enabled = false;
			parent.GetComponent<LineRenderer> ().enabled = !lineFlag;
			parent.GetComponent<Light> ().enabled = !lineFlag;
			saveChara = parent;
		}
	}

}