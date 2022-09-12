using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarriorScript : PlayerScript
{
    protected float rageBarLength;
    protected int currentRage;

    protected new void Start()
    {
        basicInits();
        initStats();
        initAnimations();
        inventory.addItem(new Consumable(0, "Rage Potion", "ResourceHeal", (Texture2D)Resources.Load("Images/RagePotion"), 50));
        equipment.equipArmor(this, new Armor(0, "Armor", "Iron Helm", "Head", (Texture2D)Resources.Load("Images/IronHelm")));
        equipment.equipArmor(this, new Armor(0, "Armor", "Iron Chest", "Chest", (Texture2D)Resources.Load("Images/IronChest")));
        equipment.equipArmor(this, new Armor(0, "Armor", "Iron Legs", "Legs", (Texture2D)Resources.Load("Images/IronLegs")));
        equipment.equipArmor(this, new Armor(0, "Armor", "Iron Boots", "Feet", (Texture2D)Resources.Load("Images/IronBoots")));
    }

    new protected void initStats()
    {
        damage = DEFAULT_DAMAGE;
        speed = DEFAULT_SPEED;
        jumpSpeed = DEFAULT_JUMP_SPEED;
        rageBarLength = regularBarLength;
        if(strength == 0) {
            strength = 5;
        }
        if(intelligence == 0) {
            intelligence = 1;
        }
        if(agility == 0) {
            agility = 3;
        }
        nextTime = 0;
        interval = 1;
        currentHealth = getMaxHealth();
        currentRage = getMaxResource();
    }

    protected new void Update()
    {
        basicUpdates();
        rageBarLength = (regularBarLength) * (currentRage / (float)getMaxResource());
    }

    protected new void OnGUI()
    {
        drawBasics();
        drawRageBar();
    }

    protected void drawRageBar()
    {
        GUIStyle redStyle = new GUIStyle(GUI.skin.box);
        redStyle.normal.background = MakeTex(2, 2, new Color(.8f, .2f, 0f, 0.75f));
        if (rageBarLength > 0)
        {
            GUI.Box(new Rect(10, 60, rageBarLength, 20), "", redStyle);
        }
        GUI.Box(new Rect(10, 60, regularBarLength, 20), currentRage + "/" + getMaxResource());
    }

    protected override void loadAbilities()
    {
        abilities.Add( (Ability)gameScript.getAbilityMap()["Melee Attack"] );
        selectedAbility = abilities[0];
    }
    
    protected new void checkInput()
    {
        basicInputs();
        extraInputs();
    }
    
    protected override void extraInputs() {
        if (Input.GetButtonDown("Fire2"))
        {
            Attack();
        }
    }

    protected new void Attack()
    {
        if(loseResource(selectedAbility.getResourceCost())) {
            weaponAnimator.Play("SwordSwing");
        }
    }

    protected new bool loseResource(int x) {
        if(currentRage >= x) {
            currentRage -= x;
            return true;
        }
        else {
            return false;
        }
    }

    protected override void fillResource() {
        currentRage = getMaxResource();
    }

    public override bool gainResource(int x) { 
        if(currentRage >= getMaxResource()) {
            return false;
        }
        else {
            if(currentRage + x > getMaxResource()) {
                this.fillResource();
            }
            else {
                currentRage += x;
            }
            return true;
        }
    }

    protected override int getMaxResource()
    {
        return 100 + (level * 20) + (strength * 20);
    }
}
