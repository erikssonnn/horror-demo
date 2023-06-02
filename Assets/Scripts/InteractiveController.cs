using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveController : MonoBehaviour {
    [SerializeField] private LayerMask lm = 0;
    [SerializeField] private float range = 0.0f;

    private InputKey input = new InputKey();

    private void Start() {
        input = InputController.Instance.InputKey;
    }

    private void Update() {
        InteractionChecker();
    }

    private void InteractionChecker() {
        RaycastHit hit;
        Ray forwardRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(forwardRay, out hit, range, lm)) {
            FindObjectOfType<CanvasController>().ToggleHand(true);
            if (Input.GetKeyDown(input.USE)) {
                if(hit.transform.GetComponentInParent<InteractObject>() != null)
                    hit.transform.GetComponentInParent<InteractObject>().Interact();
            }
        } else {
            FindObjectOfType<CanvasController>().ToggleHand(false);
        }
    }
}
