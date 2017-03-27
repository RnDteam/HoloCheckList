using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour {

    public GameObject cardPrefab;
    private GameObject[] displayedCards;
    private int CardsNumber;

	void Start () {
        CardsNumber = TaskManager.CardsNumber;
        displayedCards = new GameObject[CardsNumber];

        for(int nCardIndex = 0; nCardIndex < CardsNumber; nCardIndex++)
        {
            GameObject parent = nCardIndex == 0 ? gameObject : displayedCards[nCardIndex - 1];
            displayedCards[nCardIndex] = Instantiate(cardPrefab, parent.transform, true);
            displayedCards[nCardIndex].transform.localPosition = cardPrefab.transform.position;
            displayedCards[nCardIndex].transform.localScale = cardPrefab.transform.localScale;
            displayedCards[nCardIndex].GetComponent<SpriteRenderer>().sortingOrder = CardsNumber - nCardIndex - 1;
        }

        TaskManager.OnCardChanged += nextCard;
	}
	
    private void nextCard()
    {
        if (TaskManager.CardIndex < CardsNumber)
        {
            displayedCards[CardsNumber - TaskManager.CardIndex].SetActive(false);
        }
    }

    private void OnDestroy()
    {
        TaskManager.OnCardChanged -= nextCard;
    }
}
