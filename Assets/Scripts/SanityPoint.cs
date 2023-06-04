using System;
using UnityEngine;

public class SanityPoint : MonoBehaviour {
    [SerializeField] private float sanityChange;
    [SerializeField] private bool once;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return; 
        SanityController.instance.ChangeSanity(sanityChange);
        if (once) gameObject.GetComponent<Collider>().enabled = false;
    }
}
