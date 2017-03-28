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
            displayedCards[nCardIndex] = Instantiate(cardPrefab, transform, true);
			displayedCards[nCardIndex].transform.localPosition = cardPrefab.transform.position * (nCardIndex + 1);
            displayedCards[nCardIndex].transform.localScale = cardPrefab.transform.localScale;
			displayedCards[nCardIndex].transform.SetAsFirstSibling();
            //displayedCards[nCardIndex].GetComponent<SpriteRenderer>().sortingOrder = CardsNumber - nCardIndex - 1;
        }

        TaskManager.OnCardChanged += changeCard;
	}
	
    private void changeCard()
    {
        for (int nCardIndex = 0; nCardIndex < CardsNumber; nCardIndex++)
        {
            if(nCardIndex < TaskManager.CardIndex)
            {
                displayedCards[CardsNumber - nCardIndex - 1].SetActive(false);
            } else
            {
                displayedCards[CardsNumber - nCardIndex - 1].SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        TaskManager.OnCardChanged -= changeCard;
    }
}
