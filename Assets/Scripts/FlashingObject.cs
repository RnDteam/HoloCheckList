using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingObject : MonoBehaviour {

    public int FlashesNumber;
    public float FlashDelay;

	public void Flash(GameObject go) {
        StartCoroutine(Flash(go, null, FlashesNumber, FlashDelay));
	}

    public void Flash(GameObject go, GameObject displayedAfter)
    {
        StartCoroutine(Flash(go, displayedAfter, FlashesNumber, FlashDelay));
    }
    
    IEnumerator Flash(GameObject go, GameObject displayedAfter, int FlashesNumber, float FlashDelay)
    {
        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        if (displayedAfter != null) displayedAfter.SetActive(false);

        for (int nFlash = 0; nFlash < FlashesNumber; nFlash++)
        {
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(FlashDelay);
        }
        renderer.enabled = false;
        if (displayedAfter != null) displayedAfter.SetActive(true);
    }
}
