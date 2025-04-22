using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;

public class FollowWP : MonoBehaviour
{
    public GameObject[] waypoints;
    int currentWP = 0;
    public float speed = 10.0f;
    public float rotSpeed = 5.0f;
    // bool reverse = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (currentWP >= waypoints.Length - 1 || currentWP <= 0)
        // {
        //     reverse = !reverse;
        // }

        // if (!reverse && Vector3.Distance(transform.position, waypoints[currentWP].transform.position) < 1)
        // {
        //     currentWP++;
        // } 
        // else if (reverse && Vector3.Distance(transform.position, waypoints[currentWP].transform.position) < 1)
        // {
        //     currentWP--;
        // }

        if (Vector3.Distance(transform.position, waypoints[currentWP].transform.position) < 3)
        {
            currentWP++;
        }

        if (currentWP >= waypoints.Length)
        {
            currentWP = 0;
        }

        // transform.LookAt(waypoints[currentWP].transform);
        Quaternion lookatWP = Quaternion.LookRotation(waypoints[currentWP].transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookatWP, rotSpeed * Time.deltaTime);

        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
