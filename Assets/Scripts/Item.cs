using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public enum InteractionType { NONE, PickUp, Examine, GrabDrop}
    public enum ItemType { Static, Consumable};
    public InteractionType interactionType;
    public ItemType itemType;
    public bool stackable = false;
 
    [Header("Examine")]
    public string descriptionText;

    [Header("Custom Event")]
    public UnityEvent customEvent;
    public UnityEvent consumeEvent;
    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = 10;
    }

    public void Interact()
    {
        switch(interactionType)
        {
            case InteractionType.PickUp:
                if (!FindObjectOfType<InventorySystem>().CanPickUp())
                    return;
                //Add to pickup list and DISABLE it
                FindObjectOfType<InventorySystem>().PickUp(gameObject);
                gameObject.SetActive(false);
                break;
            case InteractionType.Examine:
                //Call the Examine item in the interaction system
                FindObjectOfType<InteractionSystem>().ExamineItem(this);
                break;
            case InteractionType.GrabDrop:
                //Grab interaction 
                FindObjectOfType<InteractionSystem>().GrabDrop();
                break;
            default:
                Debug.Log("NULL ITEM");
                break;
        }

        //Invoke (call) the custom event
        customEvent.Invoke();
    }
}
 