using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Measure : MonoBehaviour {
	enemyMove em;
	private static float frontdis = 7.0f;
	private static List<GameObject> red = new List<GameObject>();
	private static List<GameObject> blue = new List<GameObject>();

	// Use this for initialization
	void Start () {
		StartCoroutine (measureRed ());
		red.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
		StartCoroutine (measureBlue ());
		red.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
	}
	
	// Update is called once per frame
	void Update () {
	}
	private IEnumerator measureRed ()
	{
		while(true){
			red.Clear ();
			red.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
			enemyMove.allys.Clear ();
			enemyMove.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopEnemy"));
			//		allys.Remove (gameObject);
			//		foreach (Transform child in transform) {
			//			allys.Remove (child.gameObject);
			//		}
			foreach (GameObject me in red) {
				enemyMove em = me.GetComponent<enemyMove> ();
				foreach (GameObject posObj in enemyMove.allys) {
					

					Vector3 allyDis = enemyMove.distance (posObj.transform.position, me.transform.position);
					float myScaleX = me.transform.localScale.x;
					float myScaleZ = me.transform.localScale.z;
					float allyScaleZ = posObj.transform.localScale.z;
					if (allyDis.x <= myScaleX / 2
					   && allyDis.x >= -myScaleX / 2
					   && allyDis.z - ((myScaleZ + allyScaleZ) / 2) >= -frontdis
					   && allyDis.z <= 0) {
						frontdis = allyDis.z;
						em.frontAlly = posObj;
						Debug.Log (frontdis);
						em.detourReady ();

					}
					frontdis = 7f;
				}

			}
		yield return new WaitForSeconds (0.1f);
		}
	}
	private IEnumerator measureBlue ()
	{
		while(true){
			red.Clear ();
			red.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
			enemyMove.allys.Clear ();
			enemyMove.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopEnemy"));
			//		allys.Remove (gameObject);
			//		foreach (Transform child in transform) {
			//			allys.Remove (child.gameObject);
			//		}
			foreach (GameObject me in red) {
				enemyMove em = me.GetComponent<enemyMove> ();
				foreach (GameObject posObj in enemyMove.allys) {


					Vector3 allyDis = enemyMove.distance (posObj.transform.position, me.transform.position);
					float myScaleX = me.transform.localScale.x;
					float myScaleZ = me.transform.localScale.z;
					float allyScaleZ = posObj.transform.localScale.z;
					if (allyDis.x <= myScaleX / 2
						&& allyDis.x >= -myScaleX / 2
						&& allyDis.z - ((myScaleZ + allyScaleZ) / 2) >= -frontdis
						&& allyDis.z <= 0) {
						frontdis = allyDis.z;
						em.frontAlly = posObj;
						Debug.Log (frontdis);
						em.detourReady ();

					}
					frontdis = 7f;
				}

			}
			yield return new WaitForSeconds (0.1f);
		}
	}
}
