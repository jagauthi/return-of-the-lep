using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour {
    
    GameObject user;
    protected int damage, baseDamage, addedDamage;
    private Transform startTransform;
    private Quaternion startRotation;

    void Start()
    {
        baseDamage = 50;
    }

    void Update()
    {
        
    }

    public void sheathe() {
        GetComponent<Renderer>().enabled = false;
        GetComponent<CapsuleCollider>().isTrigger = false;
    }

    public void unSheathe() {
        GetComponent<Renderer>().enabled = true;
        GetComponent<CapsuleCollider>().isTrigger = true;
    }

    public void setUser(GameObject user)
    {
        this.user = user;        
    }

    public GameObject getUser()
    {
        return this.user;
    }

    public int getDamage() {
        return baseDamage + addedDamage;
    }

    public void setBaseDamage(int baseDamage) {
        this.baseDamage = damage;
    }

    public void setAddedDamage(int addedDamage) {
        this.addedDamage = addedDamage;
    }
}
