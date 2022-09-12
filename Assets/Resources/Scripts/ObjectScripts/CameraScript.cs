using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public GameObject player;
	private Vector3 offset;
    public float turnSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
        if(turnSpeed == 0)
        {
            turnSpeed = 1f;
        }
    }

    void Update()
    {
        transform.position = player.transform.position + offset;
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        Vector3 lookhere = new Vector3();
      
        lookhere = new Vector3(-mouseY * turnSpeed, mouseX * turnSpeed, 0);
        transform.Rotate(lookhere);
        /*
        float turnPoint = 0.9f;
        if (mouseX > turnPoint || mouseX < -turnPoint || mouseY > turnPoint || mouseY < -turnPoint)
        {
            print(0);
            lookhere = new Vector3(-mouseY * turnSpeed * Time.deltaTime, mouseX * turnSpeed * Time.deltaTime, 0);
            transform.Rotate(new Vector3(0, 0, 0));
        }
        else
        {
            print(1);
            transform.Rotate(new Vector3(0, 0, 0));
        }
        */
    }
}
