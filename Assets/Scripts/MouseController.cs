using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
    public static MouseController instance { get; private set; }

    private bool locked = false;

    private void Start() {
        ToggleMouse();
    }

    public void ToggleMouse() {
        locked = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
