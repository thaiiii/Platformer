using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    //Reference to waypoints
    public List<Transform> points = new List<Transform>();
    public int nextID = 0;
    public int idChangeValue = 1; // Like offset to change ID

    //Speed
    public float speed = 0.2f;
    Animator animator;
    private void Reset()
    {
        animator = GetComponent<Animator>();    
        Init();
    }

    private void Init()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        //Create root object Reset position to enemy
        GameObject root = new GameObject(name + "_Root");
        root.transform.position = transform.position;
        
        //set enemy object as a child of root
        transform.SetParent(root.transform);

        //Set waypoints position to root, as a child of root
        //Make the points of waypoints  
        GameObject waypoints = new GameObject("Waypoints"); waypoints.transform.SetParent(root.transform); waypoints.transform.position = root.transform.position;
        GameObject point_1 = new GameObject("Point 1"); point_1.transform.SetParent(waypoints.transform); point_1.transform.position = root.transform.position;
        GameObject point_2 = new GameObject("Point 2"); point_2.transform.SetParent(waypoints.transform); point_2.transform.position = root.transform.position;

        //init points list and add points to it
        points.Add(point_1.transform);
        points.Add(point_2.transform);
    }

    private void Update()
    {
        MoveToNextPoint();
    }

    void MoveToNextPoint()
    {
        //animator.SetFloat("xVelocity", speed);
        //get the next point transform
        Transform goalPoint = points[nextID];
        
        //flip enemy, move toward the goal point
        if(goalPoint.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (goalPoint.transform.position.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        transform.position = Vector2.MoveTowards(transform.position, goalPoint.transform.position, speed * Time.fixedDeltaTime);
        
        //check distant to trigger at goal point
        if(Vector2.Distance(transform.position, goalPoint.position) < 0.1f)
        {
            //Check if enemy at the end of the line (make the change -1)
            if (nextID == points.Count - 1)
                idChangeValue = -1;
            //Check if enemy at the start of the line (make the change +1)
            if (nextID == 0)
                idChangeValue = 1;
            //Apply the change to ID
            nextID += idChangeValue; 
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<LifeCount>().LoseLife();

        }
    }
}
