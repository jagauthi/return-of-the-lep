using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RangerScript : PlayerScript
{

    protected float energyBarLength, startChargeTime, totalChargeTime;
    protected int currentEnergy;

    protected new void Start()
    {
        basicInits();
        initStats();
        initAnimations();
        inventory.addItem(new Consumable(0, "Energy Potion", "ResourceHeal", (Texture2D)Resources.Load("Images/EnergyPotion"), 50));
    }

    new protected void initStats()
    {
        damage = DEFAULT_DAMAGE;
        speed = DEFAULT_SPEED;
        jumpSpeed = DEFAULT_JUMP_SPEED;
        energyBarLength = regularBarLength;
        if(strength == 0) {
            strength = 3;
        }
        if(intelligence == 0) {
            intelligence = 1;
        }
        if(agility == 0) {
            agility = 5;
        }
        nextTime = 0;
        interval = 1;
        currentHealth = getMaxHealth();
        currentEnergy = getMaxResource();
        startChargeTime = 0;
        totalChargeTime = 0;
    }

    protected new void Update()
    {
        basicUpdates();
        energyBarLength = (regularBarLength) * (currentEnergy / (float)getMaxResource());
    }

    protected new void OnGUI()
    {
        drawBasics();
        drawEnergyBar();
    }

    protected void drawEnergyBar()
    {
        GUIStyle orangeStyle = new GUIStyle(GUI.skin.box);
        orangeStyle.normal.background = MakeTex(2, 2, new Color(1f, 1f, .2f, 1f));
        if (energyBarLength > 0)
        {
            GUI.Box(new Rect(10, 60, energyBarLength, 20), "", orangeStyle);
        }
        GUI.Box(new Rect(10, 60, regularBarLength, 20), currentEnergy + "/" + getMaxResource());
    }

    protected override void loadAbilities()
    {
        abilities.Add((Ability)gameScript.getAbilityMap()["Ranged Attack"]);
        abilities.Add((Ability)gameScript.getAbilityMap()["Melee Attack"]);

        selectedAbility = abilities[0];

        GetComponentInChildren<WeaponScript>().sheathe();
    }
    
    protected new void checkInput()
    {
        basicInputs();
        extraInputs();
    }
    
    protected override void extraInputs() {
        if (Input.GetButtonDown("Fire2"))
        {
            startChargeTime = Time.time;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            totalChargeTime = Time.time - startChargeTime;
            Attack(selectedTarget);
        }
    }

    protected void Attack(Transform target)
    {
        if(selectedAbility.getName() != "Weapon" && loseResource(selectedAbility.getResourceCost())) {
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(90f, 0f, 0f) + transform.rotation.eulerAngles;
            
            Rigidbody arrowClone = Instantiate(selectedAbility.getBody(), transform.position + new Vector3(0.0f, .5f, 0.0f), rotation);
            ProjectileScript bulletScript = (BulletScript)arrowClone.gameObject.GetComponent("BulletScript");
            if(bulletScript == null) {
                bulletScript = (TargetedProjectileScript)arrowClone.gameObject.GetComponent("TargetedProjectileScript");
                TargetedProjectileScript tps = (TargetedProjectileScript)bulletScript;
                tps.setTarget(target);
            }
            bulletScript.setShooter(this.gameObject);
            bulletScript.setDamage(selectedAbility.getPower());
            bulletScript.multiplyDamage(getDamageMultiplier(totalChargeTime));
            arrowClone.velocity = myCamera.transform.forward * bulletScript.bulletSpeed;
        }
        else {
            weaponAnimator.Play("SwordSwing");
        }
    }

    protected int getDamageMultiplier(float chargeTime) {
        return (int)chargeTime + 1;
    }

    protected new bool loseResource(int x) {
        if(currentEnergy >= x) {
            currentEnergy -= x;
            return true;
        }
        else {
            return false;
        }
    }

    protected override void fillResource() {
        currentEnergy = getMaxResource();
    }

    public override bool gainResource(int x) { 
        if(currentEnergy >= getMaxResource()) {
            return false;
        }
        else {
            if(currentEnergy + x > getMaxResource()) {
                this.fillResource();
            }
            else {
                currentEnergy += x;
            }
            return true;
        }
    }

    protected override int getMaxResource()
    {
        return 100 + (level * 20) + (agility * 20);
    }
}
