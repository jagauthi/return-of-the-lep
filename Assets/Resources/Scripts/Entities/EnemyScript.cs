using UnityEngine;
using System.Collections;

public class EnemyScript : NpcScript
{
    protected bool aggrod, canShoot;
    protected float range, moveSpeed, rotationSpeed, nextTime, interval;
    public int maxHealth, currentHealth, damage, expWorth, goldWorth;
    protected Rigidbody fireball;

    protected const float maxInvuln = 0.3f;
    
    protected new void Start()
    {
        npcName = "Enemy";
        basicInits();
        initStats();
        initValues();
        initAbilities();
        getGameScript();
    }

    protected void initAbilities()
    {
        GameObject holder = (GameObject)Resources.Load("Prefabs/Fireball");
        fireball = holder.GetComponent<Rigidbody>();
    }

    protected void initStats()
    {
        nextTime = 0;
        interval = 0.2f;
        maxHealth = 100;
        currentHealth = 100;
        damage = 10;
        expWorth = 10;
        goldWorth = 10;
        range = 10f;
        moveSpeed = 5f;
        rotationSpeed = 5f;
        aggrod = false;
        canShoot = true;
    }

    protected new void Update()
    {
        enemyUpdates();
    }

    protected void enemyUpdates() {
        basicUpdates();
        if(invulnerable) {
            if(Time.time - invulnerableStartTime > maxInvuln) {
                invulnerable = false;
            }
        }   
        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer <= range || aggrod)
        {
            //Look at player
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(player.transform.position - transform.position), rotationSpeed * Time.deltaTime);

            GetComponent<Rigidbody>().velocity = transform.forward * moveSpeed;
            if (canShoot)
                Fire();
        }
        if (Time.time >= nextTime)
        {
            reload();
            nextTime += interval;
        }
    }

    protected new void OnTriggerEnter(Collider collision)
    {
        if(!invulnerable) {
            ProjectileScript bulletScript = (BulletScript)collision.gameObject.GetComponent("BulletScript");
            WeaponScript weaponScript = (WeaponScript)collision.gameObject.GetComponent("WeaponScript");
            if(bulletScript == null) {
                bulletScript = (TargetedProjectileScript)collision.gameObject.GetComponent("TargetedProjectileScript");
            }
            if (bulletScript != null && bulletScript.getShooter() != this.gameObject)
            {
                getHit(bulletScript, collision);
            }
            if(weaponScript != null && weaponScript.getUser() != this.gameObject) {
                getHit(weaponScript);
            }
        }
    }

    protected void getHit(ProjectileScript bulletScript, Collider collision) {
        aggrod = true;
        loseHealth(bulletScript.getDamage());
        Destroy(collision.gameObject);
        invulnerable = true;
        invulnerableStartTime = Time.time;
    }

    protected void getHit(WeaponScript weaponScript) {
        aggrod = true;
        loseHealth(weaponScript.getDamage());
        invulnerable = true;
        invulnerableStartTime = Time.time;
    }

    protected new void Die()
    {
        playerScript.killedEnemy(this);
        gameScript.enemyDied();
        if(GetComponent<BossScript>() != null) {
            gameScript.saveAndQuit();
        }
        Destroy(this.gameObject);
    }

    protected void loseHealth(int x)
    {
        currentHealth -= x;
        if (currentHealth <= 0)
            Die();
    }

    protected void Fire()
    {
        RaycastHit hit;

        //Only shoot if it will hit the player
        /**
        if (Physics.Raycast(transform.position, transform.forward, out hit, 15))
        {
            if (hit.collider.gameObject == player.gameObject)
            {
                Rigidbody bulletClone = Instantiate(fireball, transform.position + new Vector3(0.0f, .5f, 0.0f), transform.rotation);
                ProjectileScript bulletScript = (BulletScript)bulletClone.gameObject.GetComponent("BulletScript");
                if(bulletScript == null) {
                    bulletScript = (TargetedProjectileScript)bulletClone.gameObject.GetComponent("TargetedProjectileScript");
                }
                bulletScript.setShooter(this.gameObject);
                bulletClone.velocity = transform.forward * bulletScript.bulletSpeed;
                canShoot = false;
            }
        }
        */

        //Shoot regardless if it will hit
        Rigidbody bulletClone = Instantiate(fireball, transform.position + new Vector3(0.0f, .5f, 0.0f), transform.rotation);
        ProjectileScript bulletScript = (BulletScript)bulletClone.gameObject.GetComponent("BulletScript");
        if (bulletScript == null)
        {
            bulletScript = (TargetedProjectileScript)bulletClone.gameObject.GetComponent("TargetedProjectileScript");
        }
        bulletScript.setShooter(this.gameObject);
        bulletClone.velocity = transform.forward * bulletScript.bulletSpeed;
        canShoot = false;
    }

    protected void reload()
    {
        canShoot = true;
    }
}
