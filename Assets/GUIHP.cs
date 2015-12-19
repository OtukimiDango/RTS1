using UnityEngine;
using System.Collections;

public class GUIHP : MonoBehaviour {
	public Camera camera;
	// Use this for initialization
	void Start () {
	
	}
	
	public Transform target; // 対象のオブジェクト
	public Transform HP; // GUITextureもしくはGUIText

	void Update () 
	{
		if( HP == null || target == null )
			return;
		HP.position = camera.WorldToScreenPoint(target.position);
		Debug.Log (HP.position.y);

}
}
