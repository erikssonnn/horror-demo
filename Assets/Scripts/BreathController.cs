using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathController : MonoBehaviour {
    [SerializeField] private float verticalRange = 0.0f;
    [SerializeField] private float speed = 0.0f;

    private CharacterController cc = null;
    private Vector3 startPos = Vector3.zero;

    private void Start() {
        cc = FindObjectOfType<CharacterController>();
        startPos = transform.localPosition;
    }

    private void LateUpdate() {
        if(cc.velocity.magnitude < 0.1f && cc.isGrounded) {
            float offset = Mathf.PingPong(Time.time * speed, verticalRange);

            Vector3 newPos = startPos + new Vector3(0f, offset, 0f);
            transform.localPosition = newPos;
        }
    }
}
