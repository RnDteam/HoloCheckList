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

        TaskManager.OnCardChanged += changeCard;
	}
	
    private void changeCard()
    {
        Debug.Log("Curr CardIndex: " + TaskManager.CardIndex);
        for (int nCardIndex = 0; nCardIndex < TaskManager.CardIndex; nCardIndex++)
        {
            displayedCards[nCardIndex].SetActive(TaskManager.CardIndex >= nCardIndex);
        }
    }

    private void OnDestroy()
    {
        TaskManager.OnCardChanged -= changeCard;
    }
}
