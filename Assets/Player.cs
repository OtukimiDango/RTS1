using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private static GameObject Playercamera;
	private static Vector3 cameraPos;
	public static string mouseState;
	// Use this for initialization
	void Start () {
		mouseState = "normal";
		Playercamera = GameObject.Find ("Camera");
		cameraPos = Playercamera.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && mouseState == "normal") {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit,Mathf.Infinity)){
				Debug.Log(hit.collider.gameObject.name);
				OnGUI (hit.point);
				Debug.Log (hit.point);
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
	void OnMouseDrag()
	{
		Vector3 objectPointInScreen
		= Camera.main.WorldToScreenPoint(this.transform.position);

		Vector3 mousePointInScreen
		= new Vector3(Input.mousePosition.x,
			Input.mousePosition.y,
			objectPointInScreen.z);

		Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(mousePointInScreen);
		mousePointInWorld.z = this.transform.position.z;
		this.transform.position = mousePointInWorld;
	}

}
