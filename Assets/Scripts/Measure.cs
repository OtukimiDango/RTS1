//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class Measure : MonoBehaviour
//{
//	private static float frontdis = 14.0f;
//	private static List<GameObject> Reds = new List<GameObject> ();
//	private static List<GameObject> Blues = new List<GameObject> ();
//
//	// Use this for initialization
//	void Start ()
//	{
//		//StartCoroutine (measure ());
//		Reds.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
//		Blues.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
//	}
//
//	// Update is called once per frame
//	void Update ()
//	{
//	}
//
//	private IEnumerator measure ()
//	{
//		while (true) {
//			Reds.Clear ();
//			Reds.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
//			Red.allys.Clear ();//?
//			Red.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopEnemy"));//?
//			//		allys.Remove (gameObject);
//			//		foreach (Transform child in transform) {
//			//			allys.Remove (child.gameObject);
//			//		}
//			foreach (GameObject reds in Reds) {
//				Red em = reds.GetComponent<Red> ();
//				foreach (GameObject posObj in Red.allys) {
//					Vector3 allyDis = Red.distance (posObj.transform.position, reds.transform.position);
//					float myScaleX = reds.transform.lossyScale.x;
//					float myScaleZ = reds.transform.lossyScale.z;
//					float allyScaleZ = posObj.transform.localScale.z;
//					if (allyDis.x <= myScaleX / 2
//						&& allyDis.x >= -myScaleX / 2
//						&& allyDis.z - ((myScaleZ + allyScaleZ) / 2) >= -frontdis
//						&& allyDis.z <= 0) {
//						frontdis = allyDis.z;
//						reds.GetComponent<Red>().frontAlly = posObj;
//						reds.GetComponent<Red>().detourReady ();
//
//					}
//
//					frontdis = 7.0f;
//				}
//
//			}
//			yield return new WaitForSeconds (0.1f);
//
//			Blues.Clear ();
//			Blues.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
//			Blue.allys.Clear ();
//			Blue.allys.AddRange (GameObject.FindGameObjectsWithTag ("StopPlayer"));
//			//allys.Remove (gameObject);
//			//		foreach (Transform child in transform) {
//			//			allys.Remove (child.gameObject);
//			//		}
//			foreach (GameObject blues in Blues) {
//				Blue pm = blues.GetComponent<Blue> ();
//				foreach (GameObject posObj in Blue.allys) {
//					Vector3 allyDis = Blue.distance (posObj.transform.position, blues.transform.position);
//					float myScaleX = blues.transform.lossyScale.x;
//					float myScaleZ = blues.transform.lossyScale.z;
//					float allyScaleZ = posObj.transform.localScale.z;
//					if (allyDis.x <= myScaleX / 2
//						&& allyDis.x >= -myScaleX / 2
//						&& allyDis.z - (myScaleZ + allyScaleZ / 2) <= frontdis
//						&& allyDis.z >= 0) {
//						frontdis = allyDis.z;
//						pm.frontAlly = posObj;
//						pm.detourReady ();
//					}
//				}
//				frontdis = 7.0f;
//			}
//			yield return new WaitForSeconds (0.1f);
//
//		}
//	}
//}