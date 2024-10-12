using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("Interaction Point")]
    public Transform detectionPoint;
    public LayerMask detectionLayer;
    [SerializeField] float detectionRadius = 0.1f;

    [Header("Pick Up Item")]
    public GameObject detectedObject;

    [Header("Examine Fields")]
    public GameObject examineWindow;
    public Image examineImage;
    public TextMeshProUGUI examineText;
    public bool isExamining;
    public bool isGrabbing;
    public GameObject grabbedObject;
    public Transform grabPoint;
    public float grabbedObjectYValue;


    void Update()
    {
        if (isExamining && Input.GetKeyDown(KeyCode.E))
        {
            ExamineItem(null); // Press E again to close window
            return;
        }

        if (DetectObject())
        {
            if(InteractInput())
            {
                //If we are grabbing something dont interact with other items, must drop first
                if (isGrabbing)
                {
                    GrabDrop();
                    return ;
                }
                detectedObject.GetComponent<Item>().Interact();
            }
        }    
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
        
    }

    bool DetectObject()
    {
        

        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer); ;
        if (obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }


    public void ExamineItem(Item item)
    {
        if (!isExamining)
        {       //Stand still
                FindObjectOfType<Fox>().ResetPlayer();
                //Display an window, show image, text
                examineImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
                examineText.text = item.descriptionText;
                examineWindow.SetActive(true);
                isExamining = true;
        } else
        {
            examineWindow.SetActive(false);
            isExamining=false;
        }
    }
    
    public void GrabDrop()
    {
        //check if we have nothing grabbed ==> grab detected item
        //check if we grabbed ==> drop item
        if (isGrabbing)
        {
            //unparent the object
            //set y position to its origin
            //null the grabbed object reference
            isGrabbing = false; 
            grabbedObject.transform.parent = null;
            grabbedObject.transform.position = new Vector3(
                    grabbedObject.transform.position.x,
                    grabbedObjectYValue,
                    grabbedObject.transform.position.z);
            grabbedObject = null;

        } 
        else
        {
            //Parent the onject to player, adjust its position
            //Cache the object's y value 
            //assign the grab object to the object itself
            isGrabbing = true;
            grabbedObject = detectedObject;
            grabbedObject.transform.parent = transform;
            grabbedObjectYValue = grabbedObject.transform.position.y;
            grabbedObject.transform.localPosition = grabPoint.transform.localPosition; 
        
        }

    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);

    }

}
