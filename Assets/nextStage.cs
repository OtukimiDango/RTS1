using UnityEngine;
using System.Collections;

public class nextStage : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		Application.LoadLevel("stage1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void next(){
		Application.LoadLevel("stage1");
		Debug.Log ("yes");
	}
}
