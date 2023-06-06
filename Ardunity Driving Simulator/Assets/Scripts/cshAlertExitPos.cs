using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshAlertExitPos : MonoBehaviour
{
    public GameObject AlertExitZone;
    public bool isStarted;
    // Start is called before the first frame update
    void Start()
    {
        isStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bodycoll"))
        {
            if(isStarted == false)
            {
                int pos = this.GetComponent<cshAlertPos>().rand;
                if (pos == 0)
                {
                    AlertExitZone.SetActive(true);
                    AlertExitZone.transform.position = new Vector3(27.0f, 1.0f, 420.5f);
                    AlertExitZone.transform.localScale = new Vector3(1, 2, 5);
                }
                else if (pos == 1)
                {
                    AlertExitZone.SetActive(true);
                    AlertExitZone.transform.position = new Vector3(76, 1, 414);
                    AlertExitZone.transform.localScale = new Vector3(5, 2, 1);
                }
                else
                {
                    AlertExitZone.SetActive(true);
                    AlertExitZone.transform.position = new Vector3(-74.5f, 1.0f, 436.0f);
                    AlertExitZone.transform.localScale = new Vector3(5, 2, 1);
                }
                isStarted = true;
            }
            else
            {
                isStarted = false;
            }
            
        }
    }
}
