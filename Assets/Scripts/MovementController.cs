using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    [SerializeField] private float movementSpeed;
    [SerializeField] private float mouseSensitivity = 2;
    [SerializeField] private float jumpForce;
    [SerializeField] private Vector3 crouchedPos;

    private float minX = -90f;
    private float maxX = 90f;

    private float mouseX = 0f;
    private float mouseY = 0f;

    private float startHeight = 2.5f;
    private float endHeight = 1.0f;

    private float verticalVelocity = 0;
    private float speed;
    private float crouchSpeed;
    private float runSpeed;

    private bool crouched = false;
    private bool canStandUp = false;
    private Vector3 cameraDefaultPos;
    private CharacterController cc;
    private InputKey input = new InputKey();

    private void Start() {
        input = InputController.Instance.InputKey;
        speed = movementSpeed;
        runSpeed = movementSpeed * 1.6f;
        crouchSpeed = movementSpeed * 0.4f;

        cameraDefaultPos = Camera.main.transform.localPosition;
        cc = GetComponent<CharacterController>();
    }

    public float mSpeed {
        set {
            movementSpeed = value;
        }
        get {
            return movementSpeed;
        }
    }

    private void Movement() {
        if (Input.GetKeyDown(input.CROUCH) && canStandUp) {
            crouched = !crouched;
        }

        if (!crouched) {
            float dist = Vector3.Distance(Camera.main.transform.localPosition, cameraDefaultPos);
            speed = movementSpeed;

            if (dist > 0.01f) {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraDefaultPos, 1.3f * Time.fixedDeltaTime);
                cc.height = Mathf.Lerp(startHeight, endHeight, 1.3f * Time.fixedDeltaTime);
            }

            // if (Input.GetKey(input.RUN)) {
            //     speed = runSpeed;
            // }
        } else {
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, crouchedPos, 1.3f * Time.fixedDeltaTime);
            cc.height = Mathf.Lerp(endHeight, startHeight, 1.3f * Time.fixedDeltaTime);
            speed = crouchSpeed;
        }

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        Vector3 forwardMovement = (transform.forward * vertical).normalized;
        Vector3 rightMovement = (transform.right * horizontal).normalized;
        Vector3 upMovement = transform.up * verticalVelocity;

        forwardMovement.y = 0;
        rightMovement.y = 0;

        if (Input.GetKeyDown(KeyCode.Space) && cc.isGrounded) {
            verticalVelocity = jumpForce;
        }

        if (verticalVelocity > 0) {
            verticalVelocity -= 10 * Time.deltaTime;
        }

        upMovement.y -= 9.81f;
        Vector3 dir = forwardMovement + rightMovement;
        dir.Normalize();

        Vector3 totalMovement = dir * (Time.deltaTime * speed);
        totalMovement += upMovement * Time.deltaTime;

        cc.Move(totalMovement);
    }

    private void CameraRotation() {
        mouseX += Input.GetAxis("Mouse Y") * mouseSensitivity * 0.5f;
        mouseY += Input.GetAxis("Mouse X") * mouseSensitivity * 0.5f;

        mouseX = Mathf.Clamp(mouseX, minX, maxX);
        Camera.main.transform.eulerAngles = new Vector3(-mouseX, mouseY, 0);
        transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    }

    private void Update() {
        Movement();
        CameraRotation();
        canStandUp = CanStandUp();
    }

    private bool CanStandUp() {
        RaycastHit hit;
        Ray forwardRay = new Ray(Camera.main.transform.position, transform.up);
        if (Physics.Raycast(forwardRay, out hit, 0.5f)) {
            return false;
        }
        return true;
    }
}
