using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cshScore : MonoBehaviour
{
    public int score = 100;
    public Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        score = 100;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString() + "/100";
    }
    public void Minus3()
    {
        score -= 3;
        if(score <= 0)
            score = 0;
    }
    public void Minus5()
    {
        score -= 5;
        if (score <= 0)
            score = 0;
    }
    public void Minus10()
    {
        score -= 10;
        if (score <= 0)
            score = 0;
    }
    public void Minus15()
    {
        score -= 15;
        if (score <= 0)
            score = 0;
    }
    public void Minus100()
    {
        score -= 100;
        if (score <= 0)
            score = 0;
    }
    public void resetScore()
    {
        score = 100;
    }
}
