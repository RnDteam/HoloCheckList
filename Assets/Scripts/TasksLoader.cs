using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TasksLoader : MonoBehaviour
{
    const int numRowsInStaticCanvas = 6;
    public List<string> Tasks = new List<string>();
    public List<Color> Colors = new List<Color>();
    public List<Color> TextColors = new List<Color>();
    public GameObject TaskPrefab;
    public string ObjectName = "Task";
    public float distance = 30f;
    public bool isPlaceableCanvas;

    public TextToSpeechManager TextToSpeech;

    private int currentTask = 0;
    private bool lastTaskComplete = false;
    private Vector3 position = Vector3.zero;
    private List<GameObject> TaskGameObjects = new List<GameObject>();
    private int topStaticTask = 0;
    private Vector3 cubeScale;

    public bool AutomaticMode = false;
    public Text taskNumberText;
    public VoiceManager voice;
    public Material SpeechOnMaterial;
    public Material SpeechOffMaterial;
    public Image SpeechIcon;
    private int debug = 0;

    void Start()
    {
        int numberOfRows = 3;
        if (isPlaceableCanvas)
        {
            numberOfRows = numRowsInStaticCanvas;
        }
        for (int i = 0; i < numberOfRows; i++)
        {
            var gameobject = Instantiate(TaskPrefab, transform);
            gameobject.name = ObjectName + i.ToString();
            gameobject.transform.localScale = Vector3.one;

            var color = Colors[i % Colors.Count];
            var textColor = TextColors[i % TextColors.Count];
            gameobject.transform.FindChild("Background").GetComponent<Image>().color = color;
            gameobject.transform.FindChild("Toggle").FindChild("Background").GetComponent<Image>().color = color;
            gameobject.transform.FindChild("Toggle").FindChild("Background").FindChild("Checkmark").GetComponent<Image>().color = textColor;
            gameobject.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().color = textColor;

            gameobject.transform.localPosition = position;
            position = position - distance * transform.up;
            gameobject.AddComponent<InteractableTask>();

            TaskGameObjects.Add(gameobject);
        }
        UpdateTaskNames();
        if(isPlaceableCanvas)
            cubeScale = gameObject.transform.FindChild("Cube").transform.lossyScale;
    }

    public void ScrollDown()
    {
        Debug.Log("Scroll Down " + debug++.ToString());
        //TODO: what do we want the behavior to be when the panel displays the last six items and the user says "scroll down"?
        //should we scroll down, in which case we waste space, or should we not scroll down, which might be confusing to the user?
        if (!isPlaceableCanvas)
            return;
        if (topStaticTask < Tasks.Count - 1)
            topStaticTask++;
        for(int iter = 0; iter < numRowsInStaticCanvas; iter++)
        {
            if (topStaticTask + iter < Tasks.Count)
            {
                TaskGameObjects.ElementAt(iter).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = Tasks.ElementAt(topStaticTask + iter);
            }
            else
            {
                TaskGameObjects.ElementAt(iter).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = string.Empty;
            }
        }
        ShowSelected();
    }

    public void ScrollUp()
    {
        Debug.Log("Scroll Up " + debug++.ToString());
        if (!isPlaceableCanvas)
            return;
        if (topStaticTask > 0)
            topStaticTask--;
        for(int iter = 0; iter < numRowsInStaticCanvas; iter++)
        {
            if (topStaticTask + iter < Tasks.Count)
            {
                TaskGameObjects.ElementAt(iter).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = Tasks.ElementAt(topStaticTask + iter);
            }
            else
            {
                TaskGameObjects.ElementAt(iter).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = string.Empty;
            }
        }
        ShowSelected();
    }

    private void ShowSelected()
    {
        if(currentTask > topStaticTask + numRowsInStaticCanvas || currentTask < topStaticTask)
        {
            gameObject.transform.FindChild("Cube").gameObject.SetActive(false);
        }
        else
        {
            gameObject.transform.FindChild("Cube").gameObject.SetActive(true);
            Debug.Log("currentTask is at " + (currentTask - topStaticTask).ToString());
            gameObject.transform.FindChild("Cube").transform.position = new Vector3(-90, -40 - 30 * (currentTask - topStaticTask), 800);
            //gameObject.transform.FindChild("Cube").transform.localScale = cubeScale;
        }
    }

    private void UpdateTaskNames()
    {
        taskNumberText.text = string.Format("{0}/{1}", currentTask + 1, Tasks.Count);
        var text = string.Empty;
        if (currentTask > 0)
        {
            text = Tasks.ElementAt(currentTask - 1);
            TaskGameObjects.ElementAt(0).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = true;
        }
        else
        {
            TaskGameObjects.ElementAt(0).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = false;
        }
        TaskGameObjects.ElementAt(0).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = text;

        TaskGameObjects.ElementAt(1).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = Tasks.ElementAt(currentTask);

        text = string.Empty;
        if (currentTask < Tasks.Count - 1)
        {
            text = Tasks.ElementAt(currentTask + 1);
        }
        TaskGameObjects.ElementAt(2).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = text;

        if (lastTaskComplete)
        {
            TaskGameObjects.ElementAt(1).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = true;
        } else
        {
            TaskGameObjects.ElementAt(1).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = false;
        }
    }

    private string GetCurrentTask()
    {
        if (currentTask < Tasks.Count && !lastTaskComplete)
            return Tasks[currentTask];
        return string.Empty;
    }

    public void Check()
    {
        if (currentTask < Tasks.Count - 1)
        {
            currentTask++;
        }
        else
        {
            lastTaskComplete = true;
        }
        if (!isPlaceableCanvas)
        {
            UpdateTaskNames();
            StartCoroutine(playCheckSound());
        }
    }

    public void Uncheck()
    {
        if (currentTask == Tasks.Count - 1 && lastTaskComplete)
        {
            lastTaskComplete = false;
        }
        else if (currentTask > 0)
        {
            currentTask--;
        }
        if(!isPlaceableCanvas)
            UpdateTaskNames();
        if (AutomaticMode)
        {
            SayCurrentTask();
        }
    }

    IEnumerator playCheckSound()
    {
        if (!isPlaceableCanvas)
        {
            foreach (var sound in voice.Sounds)
            {
                sound.Stop();
            }
            GetComponent<AudioSource>().Play();
            if (AutomaticMode)
            {
                yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length - 1);
                SayCurrentTask();
            }
        }
    }

    public void SayCurrentTask()
    {
        if (isPlaceableCanvas)
            return;
        Debug.Log("say task");
        foreach (var sound in voice.Sounds)
        {
            sound.Stop();
        }
        var text = GetCurrentTask();
        if (text == string.Empty)
        {
            text = "All done. Good job.";
            voice.Sounds[voice.Sounds.Length - 1].Play();
        }
        else
        {
            voice.Sounds[currentTask].Play();
        }
    }

    public void Hide()
    {
        Debug.Log("Hide");
        transform.parent.parent.GetComponent<Canvas>().enabled = false;
    }

    public void Show()
    {
        Debug.Log("Show");
        transform.parent.parent.GetComponent<Canvas>().enabled = true;
    }

    public void TurnOnAutomaticMode()
    {
        TextToSpeech.SpeakText("Automatic mode");
        AutomaticMode = true;
        SpeechIcon.material = SpeechOnMaterial;
    }

    public void TurnOffAutomaticMode()
    {
        TextToSpeech.SpeakText("Manual mode");
        AutomaticMode = false;
        SpeechIcon.material = SpeechOffMaterial;
    }

    public void ResetApp()
    {
        SceneManager.LoadScene(0);
    }
}

