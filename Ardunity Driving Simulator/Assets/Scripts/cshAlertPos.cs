using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshAlertPos : MonoBehaviour
{
    public GameObject AlertZone;
    public int rand;
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
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    AlertZone.SetActive(true);
                    AlertZone.transform.position = new Vector3(19.0f, 1.0f, 420.5f);
                    AlertZone.transform.localScale = new Vector3(1, 2, 5);
                }
                else if (rand == 1)
                {
                    AlertZone.SetActive(true);
                    AlertZone.transform.position = new Vector3(76, 1, 406);
                    AlertZone.transform.localScale = new Vector3(5, 2, 1);
                }
                else
                {
                    AlertZone.SetActive(true);
                    AlertZone.transform.position = new Vector3(-74.5f, 1.0f, 444.0f);
                    AlertZone.transform.localScale = new Vector3(5, 2, 1);
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
