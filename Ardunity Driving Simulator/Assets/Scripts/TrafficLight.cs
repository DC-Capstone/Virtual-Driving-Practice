using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public GameObject RedLight;
    public GameObject YellowLight;
    public GameObject GreenLight;
    public bool red = false;
    bool yellow = false;
    bool green = false;
    float redtime = 5.0f;
    float yellowtime = 2.0f;
    float greentime = 8.0f;

    float updatetime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        updatetime += Random.Range(0.0f, 15.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (red == false)
            RedLight.SetActive(false);
        if (yellow == false)
            YellowLight.SetActive(false);
        if (green == false)
            GreenLight.SetActive(false);

        if (red == true)
            RedLight.SetActive(true);
        if (yellow == true)
            YellowLight.SetActive(true);
        if (green == true)
            GreenLight.SetActive(true);

        if(updatetime < greentime)
        {
            red = false;
            yellow = false;
            green = true;
        }
        else if (greentime < updatetime)
        {
            if (updatetime < greentime + yellowtime)
            {
                red = false;
                yellow = true;
                green = false;
            }
            else if (greentime + yellowtime < updatetime)
            {
                if (updatetime < greentime + yellowtime + redtime)
                {
                    red = true;
                    yellow = false;
                    green = false;
                }
                else if (redtime + yellowtime + greentime < updatetime)
                {
                    updatetime = 0.0f;
                }
            }
        }
        updatetime += Time.deltaTime;

    }
}
