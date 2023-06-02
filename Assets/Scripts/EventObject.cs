using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EventType { CLOSE_DOOR, LOCK_DOOR, ENABLE_OBJECT, SET_ANIMATION_BOOL, PLAY_SOUND, SET_ENV_COLOR, SET_SOUND }

[System.Serializable]
public class Event {
    public EventType eventType = EventType.CLOSE_DOOR;
    public GameObject obj = null;
    public bool onOff;
    public string animationName = "";
    public AudioClip audioClip = null;
    public Color envColor = Color.black;
    public bool muted = false;
}

public class EventObject : MonoBehaviour {
    [SerializeField] private Event[] events = null;

    private AudioController audioController = null;
    private CanvasController canvasController = null;
    private ScreenShake screenShake = null;

    private void Start() {
        audioController = FindObjectOfType<AudioController>();
        canvasController = FindObjectOfType<CanvasController>();
        screenShake = FindObjectOfType<ScreenShake>();
    }

    private void OnTriggerEnter(Collider col) {
        if (!col.CompareTag("Player")) return;
        PlayEvent();
    }

    private void PlayEvent() {
        foreach (Event e in events) {
            switch (e.eventType) {
                case EventType.CLOSE_DOOR:
                    if (e.obj == null) continue;
                    InteractObject i = e.obj.GetComponent<InteractObject>();
                    i.globalMuted = e.muted;
                    if (i.globalOpenDoor != e.onOff) continue;
                    i.Interact();
                    break;
                case EventType.LOCK_DOOR:
                    if (e.obj == null) continue;
                    e.obj.GetComponent<InteractObject>().globalLockedDoor = e.onOff;
                    e.obj.GetComponentInChildren<Animator>().enabled = e.onOff;
                    break;
                case EventType.ENABLE_OBJECT:
                    e.obj.SetActive(e.onOff);
                    break;
                case EventType.SET_ANIMATION_BOOL:
                    if (e.obj == null) continue;
                    e.obj.GetComponent<Animator>().SetBool(e.animationName, e.onOff);
                    break;
                case EventType.PLAY_SOUND:
                    if(e.audioClip == null) continue;
                    FindObjectOfType<AudioController>().PlayAudio(e.audioClip, -0.2f, 0.2f, 2f, e.obj.transform);
                    break;
                case EventType.SET_ENV_COLOR:
                    canvasController.StartCoroutine(canvasController.Fade(3f, 3f, 0.1f));
                    canvasController.StartCoroutine(canvasController.EnvFade(e.envColor, 1f));
                    screenShake.StartCoroutine(screenShake.Shake(2.0f, 2.0f));
                    break;
                case EventType.SET_SOUND:
                    FindObjectOfType<AudioController>().UpdateStepSound();
                    break;
            }

        }
        
        gameObject.SetActive(false);
    }
}
