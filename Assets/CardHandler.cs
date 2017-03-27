using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour {

    public GameObject cardPrefab;
    private GameObject[] displayedCards;
	// Use this for initialization
	void Start () {
        displayedCards = new GameObject[TaskManager.nCardsNumber];

        for(int nCardIndex = 0; nCardIndex < TaskManager.nCardsNumber; nCardIndex++)
        {
            GameObject parent = nCardIndex == 0 ? gameObject : displayedCards[nCardIndex - 1];
            displayedCards[nCardIndex] = Instantiate(cardPrefab, parent.transform, true);
            displayedCards[nCardIndex].transform.localPosition = cardPrefab.transform.position;
            displayedCards[nCardIndex].transform.localScale = cardPrefab.transform.localScale;
            displayedCards[nCardIndex].GetComponent<SpriteRenderer>().sortingOrder = TaskManager.nCardsNumber - nCardIndex - 1;
        }

        TaskManager.OnCardChanged += nextCard;
	}
	
    private void nextCard()
    {
        if (TaskManager.nCardsNumber - TaskManager.nCardIndex > 0)
        {
            displayedCards[TaskManager.nCardsNumber - TaskManager.nCardIndex].SetActive(false);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
