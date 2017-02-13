using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableTask : MonoBehaviour {

	private bool check = false;
    public bool Interactible = false;

    void StartInteractions()
    {
        Interactible = true;
    }

	public void OnSelect() {
        check = !check;
        Debug.Log("check = " + check.ToString());
        //transform.Find("Checkbox").Find("CheckSign").GetComponent<MeshRenderer>().enabled = check;
        //transform.parent.GetComponent<TasksLoader>().TaskStatus[gameObject.name] = check;
        transform.FindChild("Toggle").GetComponent<Toggle>().isOn = check;
	}

    /*
    public void Designate()
    {
        Debug.Log(gameObject.name);
    }
    */
}
