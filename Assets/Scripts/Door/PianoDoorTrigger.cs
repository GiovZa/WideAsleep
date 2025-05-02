using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoDoorTrigger : MonoBehaviour
{
    [SerializeField] PianoDoorController doorController;
    [SerializeField] bool isBackSide;
    bool inTriggerArea;

    void Update()
    {
        if (Input.GetKeyDown("e") && inTriggerArea)
        {
            if (isBackSide)
                doorController.OpenFromBack();
            else
                doorController.OpenFromFront();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            {
                inTriggerArea = true;
            }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            {
                inTriggerArea = false;
            }
    }
}
