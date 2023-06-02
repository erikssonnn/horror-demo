using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    [SerializeField] private AudioClip stepSound = null;
    [SerializeField] private AudioClip wetStepSound = null;
    [SerializeField] private AudioClip ambienceSound = null;
    [SerializeField] private GameObject soundObject = null;
    
    [SerializeField]  private AudioSource source = null;
    [SerializeField]  private AudioSource ambienceSource = null;

    private bool wetStep = false;

    public AudioClip GetStepSound() {
        return wetStep ? wetStepSound : stepSound;
    }

    public void StartAmbience() {
        PlayAmbience(ambienceSound);
    }
    
    public void UpdateStepSound() {
        wetStep = true;
    }

    public void PlayAudio(AudioClip clip, float minP, float maxP, float volume, Transform origin) {
        if (origin != null) {
            soundObject.transform.position = origin.position;
            AudioSource s = soundObject.GetComponent<AudioSource>();
            s.volume = volume;
            s.pitch = 1.0f + Random.Range(minP, maxP);
            s.PlayOneShot(clip);
        }
        else {
            source.volume = volume;
            source.pitch = 1.0f + Random.Range(minP, maxP);
            source.PlayOneShot(clip);
        }
    }

    private void PlayAmbience(AudioClip clip) {
        ambienceSource.clip = clip;
        ambienceSource.loop = true;
        ambienceSource.Stop();
        ambienceSource.Play();
    }
}
