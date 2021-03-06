﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void BigChecklistStateChanged();

public class bigChecklistManager : MonoBehaviour {

    public static event BigChecklistStateChanged OnPlaced;
    public static event BigChecklistStateChanged OnMoved;

    private bool lastTaskComplete;
    public VoiceManager voice;
	public PlaceableObject placeableObject;

	public TasksCard cardPrefab;
	public Transform cardsParent;
	private TasksCard[] displayedCards;
	private int CardsNumber;

    void Start () {
        //distanceBetweenTasks = TaskPrefab.GetComponent<RectTransform>().rect.height;
        //InitChecklist();

        //TaskManager.OnEndTasks += OnEndTasks;

		
    }

    public void StartCards()
    {
        CreateCards();
    }

	void OnEnable()
	{
		//TaskManager.OnEndTasks += OnEndTasks;
		TaskManager.OnTaskChanged += OnTaskChanged;
		TaskManager.OnCardChanged += OnCardChanged;
	}

	void OnDisable()
	{
		//TaskManager.OnEndTasks -= OnEndTasks;
		TaskManager.OnTaskChanged -= OnTaskChanged;
		TaskManager.OnCardChanged -= OnCardChanged;
	}

	void OnTaskChanged()
	{
		foreach (var card in displayedCards)
		{
			card.AnimateTask();
		}
	}

	void OnCardChanged()
	{
		foreach (var card in displayedCards)
		{
			card.AnimateCard();
		}
	}

	private void CreateCards()
	{
		CardsNumber = TaskManager.CardsNumber;
		displayedCards = new TasksCard[CardsNumber];

		for(int nCardIndex = 0; nCardIndex < CardsNumber; nCardIndex++)
		{
			displayedCards[nCardIndex] = Instantiate(cardPrefab, cardsParent, true);
            displayedCards[nCardIndex].name = "card" + nCardIndex.ToString();
            if (nCardIndex != 0)
                displayedCards[nCardIndex].SetActiveBackground(false);
            //displayedCards[nCardIndex].transform.localPosition = cardPrefab.transform.position * (nCardIndex + 1);
            //displayedCards[nCardIndex].transform.localScale = cardPrefab.transform.localScale;
            displayedCards[nCardIndex].transform.SetAsFirstSibling();
			displayedCards[nCardIndex].SetCard(nCardIndex);
		}
	}

    #region place/move
    public void Placed()
    {
        if (OnPlaced != null) OnPlaced();
		foreach (var card in displayedCards)
		{
			card.AnimateTask();
		}
		RecordingsManager.Instance.PlayCurrentTaskRecording();
    }

    public void Moved()
    {
        if (OnMoved != null) OnMoved();
		foreach (var card in displayedCards)
		{
			card.DisableAllTasks();
		}
    }
    #endregion

    #region Check/Next/Prev
    public void Check()
    {
		if (OneTaskController.Instance.IsChangingCard() || !gameObject.activeInHierarchy)
		{
			return;
		}

        int CurTaskIndex = TaskManager.TaskIndex;

		if (!PlaceableObject.isPlaced || TaskManager.CurrentCard == null) return;

		if (CurTaskIndex < TaskManager.CurrentCard.tasks.Length)
        {
            // Checking if task need to be signed
			if(TaskManager.CurrentCard.tasks[CurTaskIndex].signedTask)
            {
                TaskManager.check();
				displayedCards[TaskManager.CardIndex].MarkTask(CurTaskIndex, true);
                //taskNumberText.text = string.Format("{0}/{1}", CurTaskIndex + 1, CurrentCard.tasks.Length);
                //StartCoroutine(CheckAnimation(CurTaskIndex));
				Next();
            }
            else
            {
                Debug.Log("Not signed task");
            }
        }
    }

    public void Next()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
#if !TASKS_DEBUG
        if (OneTaskController.Instance.IsChangingCard())
        {
            return;
        }
#endif
            if (!PlaceableObject.isPlaced || TaskManager.CurrentCard == null) return;

        if (TaskManager.TaskIndex < TaskManager.CurrentCard.tasks.Length)
        {
            /*StartCoroutine(NextAnimation(false, TaskManager.TaskIndex));*/
			TaskManager.nextTask();
            playCheckSound();
        }
    }

    public void Undo()
    {
        if (0 < TaskManager.TaskIndex)
        {
			if (OneTaskController.Instance.IsChangingCard())
			{
				OneTaskController.Instance.CancelChangingCard();
			}
            int nTaskIdnex = TaskManager.prevTask();

			displayedCards[TaskManager.CardIndex].MarkTask(nTaskIdnex, false);
        }
        else if(TaskManager.CardIndex > 0)
        {
			if (OneTaskController.Instance.IsChangingCard())
			{
				OneTaskController.Instance.CancelChangingCard();
			}
            TaskManager.prevTask();
        }
    }
    #endregion

    #region Sound
    void playCheckSound()
    {
        GetComponent<AudioSource>().Play();
    }
    #endregion
}
