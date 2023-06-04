using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum InteractionType { DOOR, HATCH }

public class InteractObject : MonoBehaviour {
    [SerializeField] private InteractionType interactionType = InteractionType.DOOR;

    [Header("HATCH: ")]
    [SerializeField] private AudioClip openHatchClip = null;
    [SerializeField] private GameObject teleportObject = null;
    [Header(("FIRST ROOM: "))]
    [SerializeField] private bool firstHatch = false;
    [SerializeField] private GameObject particleEffect = null;
    [SerializeField] private GameObject startRoom = null;
    [SerializeField] private GameObject flashlight = null;
    [SerializeField] private GameObject tutorialObj = null;

    [Header("DOOR: ")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private AudioClip closeDoorClip = null;
    [SerializeField] private AudioClip openDoorClip = null;
    [SerializeField] private AudioClip lockedDoorClip = null;
    [SerializeField] private bool lockedDoor = false;

    private AudioSource source = null;
    private Animator anim = null;
    private Quaternion openRotation;
    private Quaternion closeRotation;
    private bool isOpening = false;
    private bool muted = false;
    private bool isClosing = false;
    private Collider col = null;
    private bool openDoor = false;
    private MovementController mc = null;
    private Transform player = null;
    private CanvasController canvasController = null;
    private AudioController audioController = null;

    public bool globalOpenDoor {
        get => openDoor;
        set => openDoor = value;
    }
    
    public bool globalMuted {
        get => muted;
        set => muted = value;
    }
    
    public bool globalLockedDoor {
        get => lockedDoor;
        set => lockedDoor = value;
    }
    
    public InteractionType InteractionType {
        get {
            return InteractionType;
        }
    }

    private void Start() {
        source = GetComponent<AudioSource>();
        canvasController = FindObjectOfType<CanvasController>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponentInChildren<Collider>();
        mc = FindObjectOfType<MovementController>();
        player = FindObjectOfType<MovementController>().transform;
        audioController = FindObjectOfType <AudioController>();
        openRotation = Quaternion.Euler(0, openAngle, 0);
        closeRotation = Quaternion.Euler(0, closeAngle, 0);
        if(interactionType == InteractionType.DOOR && !lockedDoor) GetComponentInChildren<Animator>().enabled = false;
    }

    public void Interact() {
        switch (interactionType) {
            case InteractionType.DOOR:
                InteractDoor();
                break;
            case InteractionType.HATCH:
                StartCoroutine(InteractHatch());
                break;
            default:
                break;
        }
    }

    private IEnumerator InteractHatch() {
        anim.SetBool("open", true);
        if (!muted) {
            source.pitch = 1f + UnityEngine.Random.Range(-0.2f, 0.3f);
            source.PlayOneShot(openHatchClip);
        }
        muted = false;
        mc.enabled = false;
        mc.GetComponent<CharacterController>().enabled = false;
        canvasController.RunFade(1f, 1f, 1f);
        yield return new WaitForSeconds(1f);
        if (firstHatch) {
            particleEffect.SetActive(true);
            audioController.StartAmbience();
            startRoom.SetActive(false);
            RenderSettings.ambientLight = new Color(0, 0, 0, 255);
        }

        if (firstHatch) tutorialObj.SetActive(true);
        player.position = teleportObject.transform.position;
        mc.GetComponent<CharacterController>().enabled = true;
        mc.enabled = true;
    }

    private void InteractDoor() {
        if (lockedDoor) {
            if (!muted) {
                source.Stop();
                source.pitch = 1f + UnityEngine.Random.Range(-0.1f, 0.3f);
                source.PlayOneShot(lockedDoorClip);
            }
            anim.SetTrigger("locked");
            return;
        }

        if (!isOpening && !isClosing) {
            if (transform.localRotation == closeRotation) {
                isOpening = true;
                col.enabled = false;
                if (!muted) {
                    source.pitch = 1f + UnityEngine.Random.Range(-0.3f, 0.3f);
                    source.PlayOneShot(openDoorClip);
                }
                muted = false;
            } else if (transform.localRotation == openRotation) {
                isClosing = true;
                col.enabled = false;
            }
        }
    }

    private void Update() {
        if(interactionType == InteractionType.DOOR) {
            if (isOpening) {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, openRotation, smoothTime * Time.deltaTime * 60);
                if (transform.localRotation == openRotation) {
                    isOpening = false;
                    globalOpenDoor = true;
                    col.enabled = true;
                }
            } else if (isClosing) {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, closeRotation, smoothTime * Time.deltaTime * 60);
                if (transform.localRotation == closeRotation) {
                    isClosing = false;
                    globalOpenDoor = false;
                    col.enabled = true;
                    if (!muted) {
                        source.pitch = 1f + UnityEngine.Random.Range(-0.3f, 0.3f);
                        source.PlayOneShot(closeDoorClip);
                    }
                    muted = false;
                }
            }
        }
        
        if (tutorialObj == null) return;
        if (tutorialObj.activeInHierarchy) {
            if (firstHatch) {
                if (Input.GetKey(InputController.Instance.InputKey.ACTIVATE_2)) {
                    flashlight.SetActive(true);
                    flashlight.GetComponent<FlashlightController>().ToggleFlashlight();
                    tutorialObj.SetActive(false);
                }
            }
        }
    }
}
