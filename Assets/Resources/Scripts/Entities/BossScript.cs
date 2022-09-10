using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : EnemyScript {

	protected new void Start () {
        npcName = "Enemy";
        basicInits();
        initStats();
        initValues();
        initAbilities();
        getGameScript();
	}

	protected new void initStats()
    {
        nextTime = 0;
        interval = 1;
        maxHealth = 1000;
        currentHealth = 500;
        damage = 50;
        expWorth = 200;
        range = 5f;
        moveSpeed = 7f;
        rotationSpeed = 5f;
        aggrod = false;
        canShoot = true;
    }
	
	
	protected new void Update () {
		enemyUpdates();
	}
}
