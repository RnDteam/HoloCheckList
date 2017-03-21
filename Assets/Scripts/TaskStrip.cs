using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskStrip : MonoBehaviour {

	public Text TaskText;
	public Image InfoIcon;
	public Image ValidationIcon;
	public Sprite Validated;
	public Sprite NotValidated;

	public void SetTaskText(string text)
	{
		TaskText.text = text;
	}

	public void ShowInfoIcon(bool show)
	{
		InfoIcon.gameObject.SetActive(show);
	}

	public void ShowValidationIcon(bool show)
	{
		ValidationIcon.gameObject.SetActive(show);
	}

	public void SetValidated(bool validated)
	{
		if (validated)
		{
			ValidationIcon.sprite = Validated; 
		}
		else
		{
			ValidationIcon.sprite = NotValidated; 
		}
	}
}
