using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedProjectileScript : ProjectileScript {

    Transform target;

    void Start()
    {
        baseDamage = 50;
        startTime = Time.time;
    }

    void Update()
    {
        basicUpdates();
        if(target != null) {
            Vector3 forward = target.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(forward), 20f * Time.deltaTime);
                
            GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        }
    }

    public void setTarget(Transform target) {
        this.target = target;
    }
}
