using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Measure : MonoBehaviour
{
	private static float frontdis = 14.0f;
	private static List<GameObject> reds = new List<GameObject> ();
	private static List<GameObject> blues = new List<GameObject> ();

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (measure ());
		reds.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
		blues.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	private IEnumerator measure ()
	{
		while (true) {
			reds.Clear ();
			reds.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
			enemyMove.allys.Clear ();//?
			enemyMove.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopEnemy"));//?
			//		allys.Remove (gameObject);
			//		foreach (Transform child in transform) {
			//			allys.Remove (child.gameObject);
			//		}
			foreach (GameObject red in reds) {
				enemyMove em = red.GetComponent<enemyMove> ();
				foreach (GameObject posObj in enemyMove.allys) {
					Vector3 allyDis = enemyMove.distance (posObj.transform.position, red.transform.position);
					float myScaleX = red.transform.lossyScale.x;
					float myScaleZ = red.transform.lossyScale.z;
					float allyScaleZ = posObj.transform.localScale.z;
					if (allyDis.x <= myScaleX / 2
					    && allyDis.x >= -myScaleX / 2
					    && allyDis.z - ((myScaleZ + allyScaleZ) / 2) >= -frontdis
					    && allyDis.z <= 0) {
						frontdis = allyDis.z;
						red.GetComponent<enemyMove>().frontAlly = posObj;
						red.GetComponent<enemyMove>().detourReady ();

					}

					frontdis = 7.0f;
				}

			}
			yield return new WaitForSeconds (0.1f);

			blues.Clear ();
			blues.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
			PlayerMove.allys.Clear ();
			PlayerMove.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopPlayer"));
			//allys.Remove (gameObject);
			//		foreach (Transform child in transform) {
			//			allys.Remove (child.gameObject);
			//		}
			foreach (GameObject blue in blues) {
				PlayerMove pm = blue.GetComponent<PlayerMove> ();
				foreach (GameObject posObj in PlayerMove.allys) {
					Vector3 allyDis = PlayerMove.distance (posObj.transform.position, blue.transform.position);
					float myScaleX = blue.transform.lossyScale.x;
					float myScaleZ = blue.transform.lossyScale.z;
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
