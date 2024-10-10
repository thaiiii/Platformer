using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);

    }

}
