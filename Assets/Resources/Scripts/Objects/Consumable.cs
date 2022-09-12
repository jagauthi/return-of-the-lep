using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item {

    protected string consumableType;
    protected int power;

    public Consumable(int itemNumber, string type, string consumableType, Texture2D icon, int power)
         : base(itemNumber, type, icon) {
        this.consumableType = consumableType;
        this.power = power;
        this.tooltip = consumableType + ": " + power;
    }

    public override bool use() {
        getPlayer();
        switch(consumableType) {
            case "Heal": {
                if(playerScript.gainHealth(power)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            case "ResourceHeal": {
                if(playerScript.gainResource(power)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            default: {
                //lol
                return false;
            }
        }
    }
}
