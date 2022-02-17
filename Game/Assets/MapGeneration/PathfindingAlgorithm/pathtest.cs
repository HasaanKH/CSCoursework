using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathtest : MonoBehaviour
{

    public int speed;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(5,5,0), step);
            Debug.Log(GetComponent<Renderer>().bounds.center);
        }
    }
}
