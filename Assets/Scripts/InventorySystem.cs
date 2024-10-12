using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [System.Serializable]
    //Inventory Item class
    public class InventoryItem
    {
        public GameObject obj;
        public int stack = 1;

        public InventoryItem(GameObject obj, int stack = 1)
        {
            this.obj = obj; 
            this.stack = stack;
        }
    }



    //List of items picked up
    public List<InventoryItem> items = new List<InventoryItem>();
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

    //Pick an item
    public void PickUp(GameObject item)
    {
        if(item.GetComponent<Item>().stackable)
        {
            //Check if we have an existing item in out inventory, if yes, stack it
            InventoryItem existingItem = items.Find(x => x.obj.name==item.name);
            if(existingItem != null)
            {
                existingItem.stack++;
            }
            else
            {
                InventoryItem i = new InventoryItem(item);
                items.Add(i);
            }
        }
        else
        {
            InventoryItem i = new InventoryItem(item);
            items.Add(i);
        }

        Update_UI();
    }

    public bool CanPickUp()
    {
        if(items.Count >= items_Images.Length)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void Update_UI()
    {
        HideAll();
        //for each item in the items list,
        //show it in the respective slot in the items_images
        for (int i = 0; i < items.Count; i++)
        {
            items_Images[i].sprite = items[i].obj.GetComponent<SpriteRenderer>().sprite;
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
        //If stack == 1 write only name
        if (items[id].stack == 1)
            description_Title.text = items[id].obj.name;
        else if (items[id].stack > 1)
            description_Title.text = items[id].obj.name + " x" + items[id].stack;
        description_Text.text = items[id].obj.GetComponent<Item>().descriptionText;
        
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
        if (items[id].obj.GetComponent<Item>().itemType == Item.ItemType.Consumable)
        {
            Debug.Log($"Consumed {items[id].obj.name}");
            //Invoke the consumeEvent
            items[id].obj.GetComponent<Item>().consumeEvent.Invoke();
            //reduce stack number
            items[id].stack--;
            if(items[id].stack == 0)
            {
                //Destroy the item then clear it form the list
                Destroy(items[id].obj, 0.1f);
                items.RemoveAt(id);
            }
            Update_UI();
        }

    }


}
