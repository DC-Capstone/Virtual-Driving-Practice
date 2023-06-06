using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshCheckout : MonoBehaviour
{
    public GameObject score;
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
        if (collision.gameObject.CompareTag("bodycoll"))
        {
            score.GetComponent<cshScore>().Minus10();
        }
    }
}
