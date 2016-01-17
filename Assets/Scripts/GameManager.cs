using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	public static Text Timetext, SpText, ServantText,WarriorText,WitchText,GuardText;
	private static DateTime startTime = DateTime.Now;

	public static int servantCount = 0;
	private	static int warriorCount = 0,witchCount = 0,guardCount = 0;
	public static int sp = 500;
	public static sbyte UpSp = 1;
	public static bool _isGameOver = false;

	public static int WarriorCount{
		set{
			warriorCount = value;
			WarriorText.text = warriorCount.ToString ();
		}
		get{
			return warriorCount;
		}
	}
	public static int WitchCount{
		set{
			witchCount = value;
			WitchText.text = witchCount.ToString ();
		}
		get{
			return witchCount;
		}
	}
	public static int GuardCount{
		set{
			guardCount = value;
			GuardText.text = guardCount.ToString ();
		}
		get{
			return guardCount;
		}
	}

	void Awake(){
		Timetext = GameObject.Find ("TimeText").GetComponent<Text>();
		SpText = GameObject.Find ("SpText").GetComponent<Text>();
		ServantText = GameObject.Find ("ServantText").GetComponent<Text>();
		WarriorText = GameObject.Find ("warriorText").GetComponent<Text>();
		WitchText = GameObject.Find ("witchText").GetComponent<Text>();
		GuardText = GameObject.Find ("guardText").GetComponent<Text>();
		StartCoroutine (CountTime());
		StartCoroutine (spUp());
		StartCoroutine (gameTime());
	}

	private IEnumerator CountTime(){
		TimeSpan time;
		while(!_isGameOver){
			time = (TimeSpan)(DateTime.Now - startTime);
			Timetext.text = time.Minutes.ToString ("D2")+":"+time.Seconds.ToString("D2");
			SpText.text = sp.ToString();
			ServantText.text = servantCount.ToString ();
			yield return null;
		}
	}
	private IEnumerator spUp(){
		while (!_isGameOver) {
			yield return new WaitForSeconds(1);//１秒待つ
			sp += UpSp;//spをプラス
		}
	}
	private IEnumerator gameTime(){
		yield return new WaitForSeconds (30);//３０秒待つ
		UpSp++;//SPをプラス

		yield return new WaitForSeconds (60);//6０秒待つ
		UpSp++;//SPをプラス

		yield return new WaitForSeconds (120);//12０秒待つ
		UpSp++;//SPをプラス
		//最終ステージまで210秒
	}
}
