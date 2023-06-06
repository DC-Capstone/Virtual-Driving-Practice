using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshSpedUp : MonoBehaviour
{
    public GameObject score;
    public GameObject Main;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("bodycoll"))   
        {
            if(Main.GetComponent<Rigidbody>().velocity.z >= -5.7f)
            {
                score.GetComponent<cshScore>().Minus10();
                this.gameObject.SetActive(false);
            }
            
        }
    }
}
