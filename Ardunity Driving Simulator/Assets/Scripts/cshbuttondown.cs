using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshbuttondown : MonoBehaviour
{
    public GameObject car;
    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject arduino;
    public bool first = true;
    public bool second = false;
    public bool ard = false;

    public Rigidbody rb;

    public GameObject board;
    public GameObject scoreboard;
    public GameObject track1;
    public GameObject track2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            if (first == true)
            {
                rb = car.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                car.transform.position = spawn2.transform.position;
                car.transform.rotation = spawn2.transform.rotation;
                rb.isKinematic = false;
                board.SetActive(true);
                scoreboard.SetActive(false);
                track2.SetActive(true);
                track1.SetActive(false);
                first = false;
                second = true;
            }
            else if(second == true)
            {
                rb = car.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                car.transform.position = arduino.transform.position;
                car.transform.rotation = arduino.transform.rotation;
                rb.isKinematic = false;
                board.SetActive(false);
                track2.SetActive(false);
                track1.SetActive(false);
                second = false;
                ard = true;
            }
            else if(ard == true)
            {
                rb = car.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                car.transform.position = spawn1.transform.position;
                car.transform.rotation = spawn1.transform.rotation;
                rb.isKinematic = false;
                board.SetActive(true);
                scoreboard.SetActive(false);
                track2.SetActive(false);
                track1.SetActive(true);
                ard = false;
                first = true;
            }
            
        }
    }
}
