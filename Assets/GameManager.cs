using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	public Text Timetext;
	private static DateTime startTime = DateTime.Now;
	public static bool _isGameOver = false;

	void Start(){
		StartCoroutine (CountTime());
	}

	private IEnumerator CountTime(){
		TimeSpan time;
		while(!_isGameOver){
			time = (TimeSpan)(DateTime.Now - startTime);
			Timetext.text = time.Minutes.ToString ("D2")+":"+time.Seconds.ToString("D2");
			yield return null;
		}
	}
}
