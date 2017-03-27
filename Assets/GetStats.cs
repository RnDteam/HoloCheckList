using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetStats : MonoBehaviour {

    public Text taskToSignNumber;
    public Text signedTasksNumber;

    void Start () {
        signedTasksNumber.text = (TaskManager.TasksToSign - TaskManager.GetNumberOfSignedTasks()).ToString();
        taskToSignNumber.text = (TaskManager.TasksToSign).ToString();
    }
}
