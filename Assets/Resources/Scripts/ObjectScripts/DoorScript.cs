using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

    GameObject player, gameEngine;
    
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        gameEngine = GameObject.FindGameObjectWithTag("GameEngine");
    }
	
	void Update () {
        player = GameObject.FindGameObjectWithTag("Player");
        gameEngine = GameObject.FindGameObjectWithTag("GameEngine");
    }

    void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    void OnTriggerEnter(Collider collision)
    {
        BulletScript bulletScript = (BulletScript)collision.gameObject.GetComponent("BulletScript");
        WeaponScript weaponScript = (WeaponScript)collision.gameObject.GetComponent("WeaponScript");
        if (bulletScript != null && bulletScript.getShooter() == player.gameObject)
        {
            Destroy(collision.gameObject);
            gameEngine.GetComponent<GameScript>().incrementStage();
        }
        if (weaponScript != null)
        {
            gameEngine.GetComponent<GameScript>().incrementStage();
        }
    }
}
