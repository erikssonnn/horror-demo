using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityController : MonoBehaviour {
    public static SanityController instance { get; private set; }
    private float sanity = 0.0f;
    private PostProcessing[] postProcessing = null;
    private CanvasController canvasController = null;
    private PlayerController playerController = null;
    private ScreenShake screenShake = null;

    public float globalSanity {
        get => sanity;
        set => sanity = value;
    }
    
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start() {
        postProcessing = FindObjectsOfType<PostProcessing>();
        canvasController = FindObjectOfType<CanvasController>();
        playerController = FindObjectOfType<PlayerController>();
        screenShake = FindObjectOfType<ScreenShake>();
        sanity = 100f;
    }

    public void ChangeSanity(float amount) {
        sanity += amount;
        
        //shader effects
        float whirlAmount = Mathf.Lerp(0.6f, 0.1f, (sanity + 100) / 200f);
        float vignetteAmount = Mathf.Lerp(0.45f, 0.0f, (sanity + 100) / 200f);
        foreach (PostProcessing postProcess in postProcessing) {
            postProcess.globalWhirlNoise.amount = whirlAmount;
            postProcess.globalVignette.amount = vignetteAmount;
        }

        canvasController.StartCoroutine(canvasController.Fade(1.5f, 1.5f, 0.25f));
        // screenShake.StartCoroutine(screenShake.Shake(0.75f, 1.75f));

        //die out of sanity being low
        if (!(sanity < -100f)) return;
        if (playerController == null) return;
        playerController.ChangeHealth(-100000);
    }
}
