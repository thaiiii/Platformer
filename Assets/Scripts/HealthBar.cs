using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillBar;
    public float health;

    public void LoseHealth(int damage )
    {
        if (health <= 0)
            return;
        //Reduce hrealth, refresh UI fillBar
        health -= damage;
        fillBar.fillAmount = health / 100;
        Debug.Log(fillBar.fillAmount);
        //Check if health reaches 0
        if (health <= 0)
        {
            FindObjectOfType<Fox>().Die();
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            LoseHealth(1);
        }
    }
}
