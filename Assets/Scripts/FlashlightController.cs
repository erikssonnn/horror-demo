using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour {
    [SerializeField] private GameObject obj = null;
    [SerializeField] private AudioClip audioClip = null;
    
    private bool active = false;
    private AudioSource source = null;

    private InputKey input = new InputKey();

    private void Start() {
        input = InputController.Instance.InputKey;
    }

    private void Update() {
        if (Input.GetKeyDown(input.ACTIVATE_2)) {
            ToggleFlashlight();
        }
    }

    public void ToggleFlashlight() {
        if(source == null) source = GetComponent<AudioSource>();
        source.pitch = 1.0f + Random.Range(-0.1f, 0.4f);
        source.PlayOneShot(audioClip);
        active = !active;
        obj.SetActive(active);
    }
}
