using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : ProjectileScript {

    void Start()
    {
        baseDamage = 50;
        startTime = Time.time;
    }

    void Update()
    {
        basicUpdates();
    }
}
