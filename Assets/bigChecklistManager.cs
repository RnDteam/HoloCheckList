using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class bigChecklistManager : MonoBehaviour {

    private List<string> Tasks = new List<string>();
    public List<Color> Colors = new List<Color>();
    public List<Color> TextColors = new List<Color>();
    public GameObject TaskPrefab;
    public float distanceBetweenTasks;
    private Vector3 position = Vector3.zero;
    private int checkAnimationLoops;
    public int numberOfRows;
    public bool AutomaticMode = false;
    private int currentTask = 0;
    private int firstDisplayedTask = 0;

    private struct Task
    {
        public string strText;
        public bool bIsChecked;
    }

    private List<GameObject> allTasks;
    private List<Task> lstTasks;
    private bool lastTaskComplete;
    public Text taskNumberText;
    public VoiceManager voice;
    private GameObject taskParent;

    void Start () {
        Tasks = TextsBridge.GetTasks().ToList();

        distanceBetweenTasks = TaskPrefab.GetComponent<RectTransform>().rect.height;
        taskNumberText.text = string.Format("{0}/{1}", currentTask + 1, Tasks.Count);
        taskParent = new GameObject("TaskParent");
        taskParent.transform.parent = transform;
        taskParent.transform.localPosition = Vector3.zero;

        lastTaskComplete = false;
        checkAnimationLoops = 0;
        lstTasks = new List<Task>();
        allTasks = new List<GameObject>();

        // Create task list
        for (int nTaskIndex = 0; nTaskIndex < Tasks.Count; nTaskIndex++)
        {
            Task t;
            t.bIsChecked = false;
            t.strText = Tasks[nTaskIndex];
            lstTasks.Add(t);
        }

        // Manage shown tasks
        for (int nTaskIndex = 0; nTaskIndex < Tasks.Count; nTaskIndex++)
        {
            allTasks.Add(CreateTask(nTaskIndex, "Task" + (nTaskIndex + 1), Colors[nTaskIndex % 2], TextColors[nTaskIndex % 2]));
            allTasks[nTaskIndex].transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = Tasks[nTaskIndex];

            if (nTaskIndex >= numberOfRows)
            {
                allTasks[nTaskIndex].SetActive(false);
            }
        }
    }

    private GameObject CreateEmptyTask(bool bIsTop)
    {
        GameObject goTask = Instantiate(TaskPrefab, taskParent.transform);
        goTask.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().text = string.Empty;

        goTask.name = "EmptyTask";
        goTask.transform.localScale = Vector3.one;
        ChangeColor(goTask, Colors[0], TextColors[0]);
        int nTaskIndex = bIsTop ? 0 : numberOfRows + 1;
        goTask.transform.localPosition = new Vector3(0, -nTaskIndex * distanceBetweenTasks, 0);
        goTask.transform.localRotation = Quaternion.identity;
        goTask.AddComponent<InteractableTask>();

        return goTask;
    }

    public GameObject CreateTask(int nTaskIndex, string name, Color backColor, Color textColor)
    {
        GameObject goTask = Instantiate(TaskPrefab, taskParent.transform);
        goTask.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().text = currentTask < Tasks.Count - 1 ? Tasks[currentTask + 1] : string.Empty;

        goTask.name = "Task" + nTaskIndex;
        goTask.transform.localScale = Vector3.one;
        ChangeColor(goTask, backColor, textColor);

        goTask.transform.localPosition = new Vector3(0, -nTaskIndex * distanceBetweenTasks, 0);
        goTask.transform.localRotation = Quaternion.identity;
        goTask.AddComponent<InteractableTask>();

        return goTask;
    }

    public void ChangeColor(GameObject gameobject, Color backColor, Color textColor)
    {
        gameobject.transform.FindChild("Background").GetComponent<Image>().color = backColor;
        gameobject.transform.FindChild("Toggle").FindChild("Background").GetComponent<Image>().color = backColor;
        gameobject.transform.FindChild("Toggle").FindChild("Background").FindChild("Checkmark").GetComponent<Image>().color = textColor;
        gameobject.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().color = textColor;
    }

    public void Check()
    {
        if(currentTask < Tasks.Count)
        {
            MarkTask(allTasks[currentTask], true);
            currentTask++;
            StartCoroutine(playCheckSound());
            taskNumberText.text = string.Format("{0}/{1}", currentTask + 1, Tasks.Count);
        }
    }

    public void Uncheck()
    {
        if(0 < currentTask)
        { 
            currentTask--;
            MarkTask(allTasks[currentTask], false);
            StartCoroutine(playCheckSound());
            taskNumberText.text = string.Format("{0}/{1}", currentTask + 1, Tasks.Count);
        }
    }

    public void ScrollDown()
    {
        if (firstDisplayedTask + numberOfRows < Tasks.Count)
        {
            allTasks[firstDisplayedTask].SetActive(false);
            allTasks[firstDisplayedTask + numberOfRows].SetActive(true);
            Scroll(false);
            firstDisplayedTask++;
        }
    }

    public void ScrollUp()
    {
        if (0 < firstDisplayedTask)
        {
            allTasks[firstDisplayedTask - 1].SetActive(true);
            allTasks[firstDisplayedTask + numberOfRows - 1].SetActive(false);
            Scroll(true);
            firstDisplayedTask--;
        }
    }

    public void Scroll(bool bIsUp)
    {
        taskParent.transform.position += new Vector3(0, distanceBetweenTasks * (bIsUp ? -1 : 1), 0);
    }

    private void MarkTask(GameObject goTask, bool bIsChecked)
    {
        goTask.transform.FindChild("Toggle").GetComponent<Toggle>().isOn = bIsChecked;
    }

    #region Sound
    IEnumerator playCheckSound()
    {
        voice.StopAll();
        GetComponent<AudioSource>().Play();

        if (AutomaticMode)
        {
            yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length - 1);
            SayCurrentTask();
        }
    }

    public void SayCurrentTask()
    {
        Debug.Log("say task");
        voice.StopAll();

        if (lastTaskComplete)
        {
            voice.Sounds[voice.Sounds.Length - 1].Play();
        }
        else
        {
            voice.Sounds[currentTask].Play();
        }
    }
    #endregion
}
