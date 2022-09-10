using UnityEngine;
using System.Collections;

public class NpcScript : MonoBehaviour
{
    protected bool invulnerable;
    protected float invulnerableStartTime;
    protected string npcName;
    protected GameObject player;
    protected PlayerScript playerScript;
    protected GameScript gameScript;

    protected void Start()
    {
        npcName = "NpcName";
        initValues();
    }

    protected void basicInits() {
        invulnerable = false;
        invulnerableStartTime = 0;
        getGameScript();
    }

    protected void initValues()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null && player.GetComponent<MageScript>() != null) {
            playerScript = player.GetComponent<MageScript>();
        }
        else if(player != null && player.GetComponent<RangerScript>() != null) {
            playerScript = player.GetComponent<RangerScript>();
        }
        else if(player != null && player.GetComponent<WarriorScript>() != null){
            playerScript = player.GetComponent<WarriorScript>();
        }
    }

    protected void getGameScript()
    {
        GameObject[] objects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject o in objects)
        {
            if (o.name.Equals("GameEngine"))
            {
                gameScript = o.GetComponent<GameScript>();
            }
        }
    }

    protected void Update()
    {
        
    }

    protected void basicUpdates() {
        if(player == null || playerScript == null) {
            initValues();
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    protected void OnTriggerEnter(Collider collision)
    {
        BulletScript bulletScript = (BulletScript)collision.gameObject.GetComponent("BulletScript");
        if (bulletScript != null && bulletScript.getShooter() != this.gameObject)
        {
            print("Ouch! Cut it out!");
        }
    }

    public string getName() {
        return npcName;
    }

    protected void Die()
    {
        Destroy(this.gameObject);
    }
}
