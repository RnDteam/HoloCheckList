using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    private static AudioSource audSource;
    private static GameObject asgo;
    private void Start()
    {
        asgo = new GameObject();
        audSource = asgo.AddComponent<AudioSource>();
        asgo.transform.position = Vector3.zero;
        if (SceneManager.GetActiveScene().name.Equals("open-scene"))
        {
            audSource.clip = (AudioClip)Resources.Load("haklatot\\other\\say-next-to-continue");
            audSource.Play();
        }
    }
	public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}