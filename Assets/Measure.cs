using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Measure : MonoBehaviour
{
	private static float frontdis = 14.0f;
	private static List<GameObject> blues = new List<GameObject> ();
	private static List<GameObject> reds = new List<GameObject> ();

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (measure ());
		blues.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
		reds.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
	}

	// Update is called once per frame
	void Update ()
	{
	}

	private IEnumerator measure ()
	{
		while (true) {
			blues.Clear ();
			blues.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
			Blue.allys.Clear ();//?
			Blue.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopEnemy"));//?
			//		allys.Remove (gameObject);
			//		foreach (Transform child in transform) {
			//			allys.Remove (child.gameObject);
			//		}
			foreach (GameObject blue in blues) {
				Blue em = blue.GetComponent<Blue> ();
				foreach (GameObject posObj in Blue.allys) {
					Vector3 allyDis = Blue.distance (posObj.transform.position, blue.transform.position);
					float myScaleX = blue.transform.lossyScale.x;
					float myScaleZ = blue.transform.lossyScale.z;
					float allyScaleZ = posObj.transform.localScale.z;
					if (allyDis.x <= myScaleX / 2
						&& allyDis.x >= -myScaleX / 2
						&& allyDis.z - ((myScaleZ + allyScaleZ) / 2) >= -frontdis
						&& allyDis.z <= 0) {
						frontdis = allyDis.z;
						blue.GetComponent<Blue>().frontAlly = posObj;
						blue.GetComponent<Blue>().detourReady ();

					}

					frontdis = 7.0f;
				}

			}
			yield return new WaitForSeconds (0.1f);

			reds.Clear ();
			reds.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
			Red.allys.Clear ();
			Red.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopPlayer"));
			//allys.Remove (gameObject);
			//		foreach (Transform child in transform) {
			//			allys.Remove (child.gameObject);
			//		}
			foreach (GameObject red in reds) {
				Red pm = red.GetComponent<Red> ();
				foreach (GameObject posObj in Red.allys) {
					Vector3 allyDis = Red.distance (posObj.transform.position, red.transform.position);
					float myScaleX = red.transform.lossyScale.x;
					float myScaleZ = red.transform.lossyScale.z;
					float allyScaleZ = posObj.transform.localScale.z;
					if (allyDis.x <= myScaleX / 2
						&& allyDis.x >= -myScaleX / 2
						&& allyDis.z - (myScaleZ + allyScaleZ / 2) <= frontdis
						&& allyDis.z >= 0) {
						frontdis = allyDis.z;
						pm.frontAlly = posObj;
						pm.detourReady ();
					}
				}
				frontdis = 7.0f;
			}
			yield return new WaitForSeconds (0.1f);

		}
	}
}