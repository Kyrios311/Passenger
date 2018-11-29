using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // Use this for initialization
    void Start () {
        Destroy(this.gameObject, 4f);
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SendMessage("takeDamage", 50);
            if (this.CompareTag("playerFireball"))
            {
                collision.gameObject.SendMessage("takeDamage", 50);
            }
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
        }
        else
        {
            Destroy(GetComponent<Rigidbody>());
            if(this.CompareTag("playerFireball"))
            {
                Destroy(gameObject);
            }
        }
        


    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }
    }
}
