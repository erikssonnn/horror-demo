using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {
    [SerializeField] private Image interactImg = null;
    [SerializeField] private Image fadeImg = null;
    [SerializeField] private Text text = null;
    [SerializeField] private GameObject pauseObj = null;
    [SerializeField] private AudioSource musicSource = null;

    private AudioSource[] audioSources = null;
    private ConsoleController consoleController = null;
    private PlayerController playerController = null;
    private MovementController movementController = null;
    private CharacterController characterController = null;
    private bool fadeRunning = false;
    private bool paused = false;
    private InputKey input = new InputKey();

    private void Start() {
        input = InputController.Instance.InputKey;
        consoleController = FindObjectOfType<ConsoleController>();
        playerController = FindObjectOfType<PlayerController>();
        movementController = FindObjectOfType<MovementController>();
        characterController = movementController.GetComponent<CharacterController>();
        audioSources = FindObjectsOfType<AudioSource>();
        paused = false;        
        ShowText("");
    }

    private void Update() {
        if (Input.GetKeyDown(input.PAUSE)) {
            Pause();
        }
    }

    private void Pause() {
        paused = !paused;
        movementController.enabled = !paused;
        consoleController.enabled = !paused;
        Time.timeScale = paused ? 0.0f : 1.0f;
        pauseObj.SetActive(paused);
        MouseController.instance.ToggleMouse();
    }

    public void OnVolumeChange(Slider slider) {
        float temp = musicSource.volume;
        foreach (AudioSource source in audioSources) {
            source.volume = slider.value;
        }

        musicSource.volume = temp;
    }
    
    public void OnMusicChange(Slider slider) {
        musicSource.volume = slider.value;
    }
    
    public void OnSensitivityChange(Slider slider) {
        movementController.globalMouseSensitivity = slider.value;
    }

    private IEnumerator HideText() {
        yield return new  WaitForSeconds(2.0f);
        ShowText("");
    }

    private void ShowText(string str) {
        StopCoroutine(HideText());
        text.text = str;

        if (str == "")
            StartCoroutine(HideText());
    }

    public void ToggleHand (bool onOff) {
        interactImg.enabled = onOff;
    }

    private void OnGUI() {
        if (!consoleController.Debug) return;
        string debugString = "Sanity: " + SanityController.instance.globalSanity.ToString("F2") + 
                             "\nHealth: " + playerController.globalHealth + 
                             "\nVelocity: " + characterController.velocity.magnitude.ToString("F2");
        GUI.Label(new Rect(10, 10, 100, 100), debugString);
    }

    public static IEnumerator EnvFade(Color color, float duration) {
        Color startAmbientColor = RenderSettings.ambientLight;
        Color startFogColor = RenderSettings.fogColor;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            RenderSettings.ambientLight = Color.Lerp(startAmbientColor, color, t);
            RenderSettings.fogColor = Color.Lerp(startFogColor, color, t);
            yield return null;
        }

        RenderSettings.ambientLight = color;
        RenderSettings.fogColor = color;
    }

    public void RunFade(float inSpeed, float outSpeed, float duration) {
        if (fadeRunning)
            return;
        StartCoroutine(Fade(inSpeed, outSpeed, duration));
    }
    
    private IEnumerator Fade(float inSpeed, float outSpeed, float duration) {
        fadeRunning = true;
        fadeImg.color = new Color(0, 0, 0, 0);
        float alpha = fadeImg.color.a;

        while (fadeImg.color.a < 1f) {
            alpha += Time.deltaTime * inSpeed;
            fadeImg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(duration);
        
        fadeImg.color = new Color(0, 0, 0, 1);
        alpha = fadeImg.color.a;

        while (fadeImg.color.a > 0f) {
            alpha -= Time.deltaTime * outSpeed; 
            fadeImg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeRunning = false;
    }
}
