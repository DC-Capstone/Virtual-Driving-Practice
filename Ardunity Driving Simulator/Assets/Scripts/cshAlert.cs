using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshAlert : MonoBehaviour
{
    public GameObject alert;
    public GameObject exit;
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
            alert.SetActive(true);
            Invoke("DeActive", 2);
        }
    }
    private void DeActive()
    {
        this.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);
        alert.SetActive(false);
    }
}
