using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTasks : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Card[] cards = TextsBridge.GetCards();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
