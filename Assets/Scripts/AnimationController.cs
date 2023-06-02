using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
    public void ResetDoorLocked() {
        GetComponent<Animator>().ResetTrigger("locked");
    }
}
