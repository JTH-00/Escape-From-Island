using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IteractiveMotion : MonoBehaviour
{
    public float interactDiastance = 10f;
    public GameObject DoorOpenUI;

    void DoorActive()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        DoorOpenUI.SetActive(false);

        if (Physics.Raycast(ray, out hit, interactDiastance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                DoorOpenUI.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hit.collider.transform.GetComponent<OpenDoorScript>().ChangeDoorState();

                    }
                
            }
            if (hit.collider.CompareTag("LockedDoor"))
            {
                DoorOpenUI.SetActive(true);
                if (DoorOpenUI == true) 
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hit.collider.transform.GetComponent<NotOpenDoorScript>().PlayLockedSound();
                    }
                }
            }
        }
       /* IEnumerator DoorDelay()
                {
                    while (true)
                    {
                        DoorOpenUI.SetActive(false);
                        yield return new WaitForSeconds(2.0f);
                    }
         }*/
    }

    void Update()
    {
        DoorActive();

    }
}