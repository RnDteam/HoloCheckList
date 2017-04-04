using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingsManager : MonoBehaviour {

	private const string PLACE_RECORDING_PATH = "haklatot\\other\\say-place";
	private const string NEXT_RECORDING_PATH = "haklatot\\other\\say-next-to-continue";
	private const string DONE_RECORDING_PATH = "haklatot\\other\\done";

	public static RecordingsManager Instance;

	private AudioSource audioSource;

	// Use this for initialization
	void Awake () {
		Instance = this;
		audioSource = gameObject.AddComponent<AudioSource>();
	}
	
	void PlaySoundFromResources(string path)
	{
		audioSource.Stop();
		audioSource.clip = (AudioClip)Resources.Load(path);
		audioSource.Play();
	}

	public void PlayPlaceRecording()
	{
		PlaySoundFromResources(PLACE_RECORDING_PATH);
	}

	public void PlayDoneRecording()
	{
		PlaySoundFromResources(DONE_RECORDING_PATH);
	}

	public void PlayNextRecording()
	{
		PlaySoundFromResources(NEXT_RECORDING_PATH);
	}

	public void PlayStartCardRecording(int cardID)
	{
		PlaySoundFromResources("haklatot\\" + TaskManager.GetCard(cardID).folder + "\\" + TaskManager.GetCard(cardID).start);
	}

	public void PlayFinishCardRecording(int cardID)
	{
		PlaySoundFromResources("haklatot\\" + TaskManager.GetCard(cardID).folder + "\\" + TaskManager.GetCard(cardID).finish);
	}

	public void PlayTaskRecording(int cardID, int taskID)
	{
		PlaySoundFromResources("haklatot\\" + TaskManager.GetCard(cardID).folder + "\\" + TaskManager.GetCard(cardID).tasks[taskID].file);
	}

	public void PlayCurrentTaskRecording()
	{
		StopCoroutine("PlayNextCardSequence");
		PlaySoundFromResources("haklatot\\" + TaskManager.CurrentCard.folder + "\\" + TaskManager.CurrentTask.file);
	}

	public void PlayNextCardRecording()
	{
		StopCoroutine("PlayNextCardSequence");
		StartCoroutine("PlayNextCardSequence");
	}

	IEnumerator PlayNextCardSequence()
	{
		int cardIndex = TaskManager.CardIndex;
		PlayFinishCardRecording(cardIndex-1);
		yield return new WaitForSeconds(audioSource.clip.length);
		PlayStartCardRecording(cardIndex);
		yield return new WaitForSeconds(audioSource.clip.length + OneTaskController.Instance.enterSpeed);
		PlayTaskRecording(TaskManager.CardIndex, 0);
	}

	public float GetLengthForNextCard()
	{
		float length = 0;
		length += GetLengthForClip("haklatot\\" + TaskManager.GetCard(TaskManager.CardIndex-1).folder + "\\" + TaskManager.GetCard(TaskManager.CardIndex-1).finish);
		length += GetLengthForClip("haklatot\\" + TaskManager.GetCard(TaskManager.CardIndex).folder + "\\" + TaskManager.GetCard(TaskManager.CardIndex).start);
		return length;
	}

	private float GetLengthForClip(string path)
	{
		return ((AudioClip)Resources.Load(path)).length;
	}

	public float GetLengthForCurrentClip()
	{
		if (audioSource.clip)
		{
			return audioSource.clip.length;
		}
		return 0;
	}
}
