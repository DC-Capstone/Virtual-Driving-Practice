using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshCheck : MonoBehaviour
{
    public GameObject beep;
    public GameObject exit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("bodycoll")){
            exit.SetActive(true);
            Invoke("Check", 1.5f);
            beep.SetActive(false);
        }
    }


    private void Check()
    {
        exit.SetActive(false);
        beep.SetActive(true);
        
    }
}
