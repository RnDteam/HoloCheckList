using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

    public GameObject openCanvas;
    public GameObject cardsCanvas;
    public GameObject endCanvas;
    public TrackedObjectsContainer trackedObjectManager;

    void OnEnable()
    {
        TaskManager.OnEndTasks += GoToEndCanvas;
    }

    void OnDisable()
    {
        TaskManager.OnEndTasks -= GoToEndCanvas;
    }

    void Start () {
        GoToOpenCanvas();
    }

    private void GoToOpenCanvas()
    {
        openCanvas.SetActive(true);
        cardsCanvas.SetActive(false);
        endCanvas.SetActive(false);
    }

    public void GoToCardCanvas()
    {
        if (openCanvas.activeSelf)
        {
            TaskManager.InitTaskManager();
            cardsCanvas.GetComponentInChildren<bigChecklistManager>().StartCards();
            trackedObjectManager.CheckCurrentTask();

            openCanvas.SetActive(false);
            cardsCanvas.SetActive(true);
            endCanvas.SetActive(false);
        }
    }

    public void GoToEndCanvas()
    {
        if (cardsCanvas.activeSelf)
        {
            openCanvas.SetActive(false);
            cardsCanvas.SetActive(false);
            endCanvas.SetActive(true);
        }
    }
}
