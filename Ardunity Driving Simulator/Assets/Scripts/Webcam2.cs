using System;
using UnityEngine;
using System.Collections;

public class Webcam2 : MonoBehaviour
{
    public GameObject display;
    public GameObject btn;
    bool Display = false;
    WebCamTexture camTexture;
    private int currentIndex = 1; //0ÀÌ¸é ³ëÆ®ºÏ À¥Ä·, 1ÀÌ¸é Ãß°¡ÇÑ À¥Ä·(droid cam)

    private void Start()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
            camTexture = null;
        }
        WebCamDevice device = WebCamTexture.devices[currentIndex];
        camTexture = new WebCamTexture(device.name);
        MeshRenderer render = display.GetComponent<MeshRenderer>();
        render.material.mainTexture = camTexture;
        camTexture.Play();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            if (btn.GetComponent<cshbuttondown>().first == true)
            {
                Display = false;
            }
            else if (btn.GetComponent<cshbuttondown>().second == true)
            {
                Display = false;
            }
            else if (btn.GetComponent<cshbuttondown>().ard == true)
            {
                Display = true;
            }

        }
       

        if (Display == true)
        {
            display.gameObject.SetActive(true);
        }
        if (Display == false)
        {
            display.gameObject.SetActive(false);
        }
    }
    
}