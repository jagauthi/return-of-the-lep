using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {
    
    GameObject shooter;
    protected int damage, baseDamage;
    public float bulletSpeed = 100f;
    protected float startTime; 
    protected Transform startTransform;
    protected Quaternion startRotation;

    void Start()
    {
        baseDamage = 50;
        startTime = Time.time;
    }

    void Update()
    {
        basicUpdates();
    }

    protected void basicUpdates() {
        if(Time.time - startTime > 2)
        {
            Destroy(this.gameObject);
        }
    }

    public void setShooter(GameObject shooter)
    {
        this.shooter = shooter;        
    }

    public GameObject getShooter()
    {
        return this.shooter;
    }

    public void setDamage(int damage) {
        this.damage = damage;
    }

    public int getDamage() {
        if(damage == 0) {
            return baseDamage;
        }
        return damage;
    }

    public void multiplyDamage(int modifier) {
        this.damage *= modifier;
    }
}
