using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoMapTwo : MonoBehaviour
{
    public GameObject Car;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                rb = Car.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                Car.transform.position = this.transform.position;
                Car.transform.rotation = this.transform.rotation;
                rb.isKinematic = false;
            }
                
        }
    }
}

