using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment {

    bool open;
    Hashtable equipment;

    public Equipment() {
        equipment = new Hashtable();
        equipment.Add("Helm", null);
        equipment.Add("Chest", null);
        equipment.Add("Legs", null);
        equipment.Add("Feet", null);
        open = false;
    }

    public Equipment(Hashtable equipment) {
        this.equipment = equipment;
        open = false;
    }

    public bool equipArmor(PlayerScript player, Armor item) {
        //if(player.meetsRequirements(item)) {
            if(equipment[item.getSlot()] != null) {
                if(!player.getInventory().addItem(item)) { 
                    Debug.Log("Inventory full");
                    return false;
                }
            }
            equipment[item.getSlot()] = item;
            return true;
        //}
        //else { return false; }
    }

    public List<Item> getItems() {
        List<Item> items = new List<Item>();
        foreach (Item item in equipment.Values) {
            items.Add(item);
        }
        return items;
    }

    public Hashtable getItemMap() {
        return equipment;
    }

    public bool isOpen() {
        return open;
    }

    public void toggle() {
        open = !open;
    }

    public void close() {
        open = false;
    }
}
