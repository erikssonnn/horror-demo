using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float startHealth = 0;
    [SerializeField] private GameObject deathPanel = null;
    [SerializeField] private GameObject flashlight = null; // skit lösning

    private float health = 0;

    public float globalHealth {
        get => health;
        set => health = value;
    }

    private MovementController mc = null;
    private ConsoleController cc = null;
    private CanvasController canvasController = null;
    private PostProcessing[] postProcessing = null;

    private void Start() {
        postProcessing = FindObjectsOfType<PostProcessing>();
        mc = FindObjectOfType<MovementController>();
        cc = FindObjectOfType<ConsoleController>();
        canvasController = FindObjectOfType<CanvasController>();
        
        health = startHealth;
    }

    public void ChangeHealth(float amount) {
        health += amount;
        
        if (!(health <= 0)) return;
        foreach (PostProcessing postProcess in postProcessing) {
            postProcess.globalWhirlNoise.amount = 0.0f;
        }

        deathPanel.SetActive(true);
        canvasController.enabled = false;
        mc.enabled = false;
        cc.enabled = false;
        flashlight.SetActive(false);
        Time.timeScale = 0.0f;
        
        StartCoroutine(Exit());
    }

    private static IEnumerator Exit() {
        yield return new WaitForSecondsRealtime(5f);
        Application.Quit();
    }
}
