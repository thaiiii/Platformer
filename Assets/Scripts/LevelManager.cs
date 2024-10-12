using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    Vector2 playerInitPosition;

    private void Start()
    {
        playerInitPosition = FindObjectOfType<Fox>().transform.position;
    }
    public void Restart()
    {
        //1- restart the scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //2- reset player position, health, score, reappear coin,...
        FindObjectOfType<Fox>().ResetPlayer();
        FindObjectOfType<Fox>().isDead = false;
        FindObjectOfType<Fox>().transform.position = playerInitPosition;
        FindObjectOfType<LifeCount>().ResetLifeCount();
        FindObjectOfType<SuperMarioCameraFollow>().Follow();
    }
}
