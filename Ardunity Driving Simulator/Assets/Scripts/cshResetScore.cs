using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshResetScore : MonoBehaviour
{
    public GameObject score;
    public GameObject scoreboard;
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
            if (isStarted == false)
            {
                score.GetComponent<cshScore>().resetScore();
                scoreboard.SetActive(true);
                isStarted = true;
            }
            else
            {
                isStarted = false;
                Invoke("scoreboardOut", 3);
            }
        }
    }
    private void scoreboardOut()
    {
        scoreboard.SetActive(false);
    }
}
