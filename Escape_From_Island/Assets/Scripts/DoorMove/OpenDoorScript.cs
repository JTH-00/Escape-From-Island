using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorScript : MonoBehaviour
{
    
    public bool open = false;
    public float doorOpenAngle;
    public float smoot = 3f;
    private int ClipsNum;
    
    AudioSource audioSource;
    public AudioClip[] clips = new AudioClip[2];

    void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }


    public void ChangeDoorState()
    {
        open = !open;
        audioSource.clip = clips[ClipsNum];
        audioSource.Play();
    }
    void OpenDoor()
    {
        if (open)   
        {
            ClipsNum = 0;
            Quaternion targetRotation = Quaternion.Euler(0, doorOpenAngle + 90f, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoot * Time.deltaTime);


        }
        else
        {
            ClipsNum = 1;
            Quaternion targetRotation2 = Quaternion.Euler(0, doorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smoot * Time.deltaTime * 2);
        }
    }

    void Update()
{
        OpenDoor();


}

}