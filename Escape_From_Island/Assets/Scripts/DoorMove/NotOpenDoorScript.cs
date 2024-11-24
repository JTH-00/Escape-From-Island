using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotOpenDoorScript : MonoBehaviour
{
    public AudioClip LockedDoorSound;

    AudioSource audioSource;

    void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }


    public void PlayLockedSound()
    {
        audioSource.clip = LockedDoorSound;
        audioSource.Play();
    }


    void Update()
    {
    }

}