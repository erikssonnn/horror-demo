using System;
using UnityEngine;

public class SanityPoint : MonoBehaviour {
    [SerializeField] private float sanityChange;
    [SerializeField] private bool once;
    [SerializeField] private bool continuous;
    
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return; 
        SanityController.instance.ChangeSanity(sanityChange);
        if (once) gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Player")) return;
        if (!continuous) return; 
        SanityController.instance.ChangeSanity(sanityChange);
    }
}
