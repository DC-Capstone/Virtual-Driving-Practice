using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshStopLine : MonoBehaviour
{
    public GameObject trafficlight;
    public GameObject score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bodycoll"))
        {
            if (trafficlight.GetComponent<TrafficLight>().red == true)
            {
                score.GetComponent<cshScore>().Minus100();
            }
        }
        
    }
}
