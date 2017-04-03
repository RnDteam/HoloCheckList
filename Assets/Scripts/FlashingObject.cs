using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingObject : MonoBehaviour {

    public int FlashesNumber;
    public float FlashDelay;

	public void Flash(GameObject go) {
        StartCoroutine(Flash(go, FlashesNumber, FlashDelay));
	}

    IEnumerator Flash(GameObject go, int FlashesNumber, float FlashDelay)
    {
        MeshRenderer renderer = go.GetComponent<MeshRenderer>();

        for (int nFlash = 0; nFlash < FlashesNumber; nFlash++)
        {
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(FlashDelay);
        }

        renderer.enabled = false;
    }
}
