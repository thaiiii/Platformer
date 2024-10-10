using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LifeCount : MonoBehaviour
{
    public Image[] lives;
    public int livesRemaining;

    //4 lives
    public void LoseLife()
    {
        if (livesRemaining == 0)
            return;
        //decrease livesRemaining, hide 1 heart 
        livesRemaining--;
        lives[livesRemaining].enabled = false;
        //Run out of lives, lose
        if (livesRemaining == 0)
        {
            FindObjectOfType<Fox>().Die();
        }
    }

    private void Start()
    {
        livesRemaining = lives.Length;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            LoseLife();
        }
    }

    public void ResetLifeCount()
    {
        for (int i = 0; i < lives.Length; i++)
        {
            lives[i].enabled = true;
        } 
        livesRemaining = lives.Length;

    }
}
