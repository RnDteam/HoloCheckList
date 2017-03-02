using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class TextLoader : MonoBehaviour {

	public string textName;

	// Use this for initialization
	void Start () {
		GetComponent<Text>().text = TextsBridge.GetText(textName);
	}
}
