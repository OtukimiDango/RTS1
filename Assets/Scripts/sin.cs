using UnityEngine;
using System.Collections;

public class sin : MonoBehaviour {
	private Vector3 mypos;
	// Use this for initialization
	void Start () {
		mypos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		switch(transform.name){
		case ("1"):
			mypos.x += Mathf.Sin (Time.time / 6)/2;
			mypos.z += Mathf.Cos (Time.time / 6)/2;
			transform.Rotate (0, 0.3f, 0);
			transform.position = mypos;
			break;
		case("2"):
			mypos.x += Mathf.Sin ((Time.time / 4)*-1);
			mypos.z += Mathf.Cos ((Time.time / 4)*-1);
			transform.Rotate (0, 0.5f, 0);
			transform.position = mypos;
			break;
		case("3"):
			transform.Rotate (0, 0.1f, 0);
			break;
	}
		switch (transform.tag) {
		case("SubCrystal"):
			mypos.z -= Mathf.Sin (Time.time/1.6f)/3;
			mypos.x -= Mathf.Cos (Time.time/1.6f)/3;
			transform.Rotate (0, 0.3f, 0);
			transform.position = mypos;
			break;
		case("SubCrystal2"):
			mypos.z += Mathf.Sin (Time.time / 1.6f) / 3;
			mypos.x += Mathf.Cos (Time.time / 1.6f)/3;
			transform.Rotate (0, 0.3f, 0);
			transform.position = mypos;
			break;
		}
}
}
