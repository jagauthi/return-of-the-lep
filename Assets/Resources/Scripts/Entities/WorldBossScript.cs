using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBossScript : EnemyScript {

	// Use this for initialization
	protected new void Start () {
        npcName = "Enemy";
        basicInits();
        initStats();
        initValues();
        initAbilities();
        getGameScript();
	}

    protected new void initAbilities()
    {
        GameObject holder = (GameObject)Resources.Load("Prefabs/MeteorStrike");
        fireball = holder.GetComponent<Rigidbody>();
    }

	protected new void initStats()
    {
        nextTime = 0;
        interval = 1;
        maxHealth = 2000;
        currentHealth = 2000;
        damage = 100;
        expWorth = 300;
        range = 100f;
        moveSpeed = 10f;
        rotationSpeed = 5f;
        aggrod = false;
        canShoot = true;
    }
	
	protected new void Update()
    {
        enemyUpdates();
    }

	protected new void Fire()
    {
		Rigidbody bulletClone = Instantiate(fireball, transform.position + new Vector3(0.0f, .5f, 0.0f), transform.rotation);
		BulletScript bulletScript = (BulletScript)bulletClone.gameObject.GetComponent("BulletScript");
		bulletScript.setShooter(this.gameObject);
		bulletClone.velocity = transform.forward * bulletScript.bulletSpeed * 5;
		canShoot = false;
    }
}
