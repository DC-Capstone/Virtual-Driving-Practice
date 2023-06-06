using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshWallTrigger : MonoBehaviour
{
    public GameObject score;
    public GameObject start;

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
            if(start.GetComponent<cshResetScore>().isStarted == true)
                score.GetComponent<cshScore>().Minus15();
        }
    }

}
