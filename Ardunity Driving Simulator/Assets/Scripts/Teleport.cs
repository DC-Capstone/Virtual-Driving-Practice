using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject Car;
    public GameObject One;
    public GameObject Two;
    public GameObject Arduino;
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
        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.F2))
            {
                rb = Car.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                Car.transform.position = One.transform.position;
                Car.transform.rotation = One.transform.rotation;
                rb.isKinematic = false;
                board.SetActive(true);
                scoreboard.SetActive(false);
                track2.SetActive(true);
                track1.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.F1))
            {
                rb = Car.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                Car.transform.position = Two.transform.position;
                Car.transform.rotation = Two.transform.rotation;
                rb.isKinematic = false;
                board.SetActive(true);
                scoreboard.SetActive(false);
                track2.SetActive(false);
                track1.SetActive(true);
                track1.GetComponent<cshAlertPos>().isStarted = false;
                track1.GetComponent<cshAlertExitPos>().isStarted = false;
                track1.GetComponent<cshResetScore>().isStarted = false;
            }
            if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.F3))
            {
                rb = Car.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                Car.transform.position = Arduino.transform.position;
                Car.transform.rotation = Arduino.transform.rotation;
                rb.isKinematic = false;
                board.SetActive(false);
                track2.SetActive(false);
                track1.SetActive(false);
            }
        }
    }
}
