using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

    protected string weaponName;
    protected GameObject weaponGameObject;
    protected WeaponScript weaponScript;

    public Weapon(int itemNumber, string type, string weaponName, Texture2D icon) 
    : base(itemNumber, type, icon) {
        this.weaponName = weaponName;
        basicInits();
    }

    public void basicInits() {
        loadWeaponInfo(weaponName);
    }
    
    void loadWeaponInfo(string weaponName) {
        weaponGameObject = (GameObject)Resources.Load("Prefabs/Weapon");
        weaponScript = weaponGameObject.GetComponent<WeaponScript>();
        // baseDamage = 10;
        // addedDamage = 0;
    }

    // public int getDamage() {
    //     return baseDamage + addedDamage;
    // }

    // public void setBaseDamage(int baseDamage) {
    //     this.baseDamage = damage;
    // }

    // public void setAddedDamage(int addedDamage) {
    //     this.addedDamage = addedDamage;
    // }
}
