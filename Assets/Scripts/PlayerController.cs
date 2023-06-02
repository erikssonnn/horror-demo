using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float startHealth = 0;
    [SerializeField] private GameObject deathPanel = null;

    private float health = 0;

    public float globalHealth {
        get => health;
        set => health = value;
    }

    private MovementController mc = null;
    private ConsoleController cc = null;
    private PostProcessing[] postProcessing = null;

    private void Start() {
        postProcessing = FindObjectsOfType<PostProcessing>();
        mc = FindObjectOfType<MovementController>();
        cc = FindObjectOfType<ConsoleController>();

        health = startHealth;
    }

    public void ChangeHealth(float amount) {
        health += amount;

        if (amount < 0 && health > 0) {
            //blood effect on screen
        }

        if (!(health <= 0)) return;
        foreach (PostProcessing postProcess in postProcessing) {
            postProcess.globalWhirlNoise.amount = 0.0f;
        }
        deathPanel.SetActive(true);
        mc.enabled = false;
        cc.enabled = false;
        Time.timeScale = 0.0f;
    }
}
