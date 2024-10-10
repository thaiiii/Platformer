using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    //List of items picked up
    public List<GameObject> items = new List<GameObject>();
    public bool isOpen;
    //Inventory window
    [Header("UI Items Section")]
    public GameObject ui_Window;
    public Image[] items_Images;
        
    [Header("UI Items Description")]
    public GameObject ui_Description_Window;
    public Image description_Images;
    public TextMeshProUGUI description_Title;
    public TextMeshProUGUI description_Text;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            ToggleInventory();
            
        }
    }

    void ToggleInventory()
    {
        //Stand still
        FindObjectOfType<Fox>().ResetPlayer();
        isOpen = !isOpen;
        ui_Window.SetActive(isOpen);
        Update_UI();
    }

    public void PickUp(GameObject item)
    {
        items.Add(item);
        Update_UI();
    }

    void Update_UI()
    {
        HideAll();
        //for each item in the items list,
        //show it in the respective slot in the items_images
        for (int i = 0; i < items.Count; i++)
        {
            items_Images[i].sprite = items[i].GetComponent<SpriteRenderer>().sprite;
            items_Images[i].gameObject.SetActive(true);
            
        }

    }

    void HideAll()
    {
        foreach(var i in items_Images)
        {
            i.gameObject.SetActive(false);
        }
        HideDescription();
    }

    public void ShowDescription(int id)
    {
        description_Images.sprite = items_Images[id].sprite;
        description_Title.text = items[id].GetComponent<Item>().name;
        description_Text.text = items[id].GetComponent<Item>().descriptionText;
        //Show 2 above info
        description_Images.gameObject.SetActive(true);
        description_Title.gameObject.SetActive(true);
        description_Text.gameObject.SetActive(true);
    }

    public void HideDescription()
    {
        description_Images.gameObject.SetActive(false);
        description_Title.gameObject.SetActive(false);
        description_Text.gameObject.SetActive(false);
    }

    public void Consume(int id)
    {
        if (items[id].GetComponent<Item>().itemType == Item.ItemType.Consumable)
        {
            Debug.Log($"Consumed {items[id].name}");
            //Invoke the consumeEvent
            items[id].GetComponent<Item>().consumeEvent.Invoke();
            //Destroy the item then clear it form the list
            Destroy(items[id], 0.1f);
            items.RemoveAt(id);
            Update_UI();
        }

    }


}
