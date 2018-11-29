using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

    public bool paused = false; //pauser
    public bool exitLevel = false;
    //Physics/Rigidbody variables
    public Vector3 respawnPoint;
    public float speed = 10.0f, jumpSpeed = 5.0f, distToGround;
    Rigidbody playerRB;
    private float distanceToGround;
    public bool grounded; //Bool for player ground interaction
    RaycastHit hit = new RaycastHit();

    public GameObject playerSword, playerArrow, gameManagerObject;
    Inventory inventory;

    public GameObject passengerObject;
    private Passenger passenger;

    private void Awake()
    {
        passenger = passengerObject.GetComponent<Passenger>();
        inventory = GetComponent<Inventory>();
        playerRB = GetComponent<Rigidbody>();
    }
    // Use this for initialization
    void Start () {


        Physics.Raycast(transform.position, -Vector3.up, out hit);
        distanceToGround = hit.distance;
        
	}
	
	// Update is called once per frame
	void Update () {
        //Physics.IgnoreCollision(playerArrow.GetComponent<Collider>(), GetComponent<Collider>());
        if (!paused)
        {
            float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

            transform.Translate(x, 0, y);
            if (Input.GetKeyDown("space") && isGrounded())
            {
                playerRB.velocity = new Vector3(playerRB.velocity.x, jumpSpeed, playerRB.velocity.z);
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                inventory.TakeDamage(10);
                GetComponent<Rigidbody>().AddForce(transform.forward * -1 * 10000);
                break;

            //Item cheat sheet: 0=Empty, 1=Sword, 2=HealthPot, 3= key, 4=bow, 5=coin, 6=toy, 7=crystal, 8 = restore, 9 staff

            case "swordPickup":
                if (inventory.addToInventory(1) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "healthPotPickup":
                if (inventory.addToInventory(2) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "chestKeyPickup":
                if (inventory.addToInventory(3) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "bowPickup":
                if (inventory.addToInventory(4) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "coinPickup":
                if (inventory.addToInventory(5) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "toyPickup":
                if (inventory.addToInventory(6) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "crystalPickup":
                if (inventory.addToInventory(7) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "restorePickup":
                if (inventory.addToInventory(8) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "staffPickup":
                if (inventory.addToInventory(9) == 1)
                {
                    Destroy(other.gameObject);
                }
                break;
            case "enemyProjectile":
                inventory.TakeDamage(10);
                Destroy(other.gameObject);
                break;
            case "levelTrigger":
                Destroy(other.gameObject);
                passengerMessageSwitch(other.name);
                break;
            case "redKey":
                Destroy(other.gameObject);
                inventory.addKeys(0);
                gameManagerObject.SendMessage("levelEnded");
                Debug.Log("pcExitLevel");
                break;
            case "greenKey":
                Destroy(other.gameObject);
                inventory.addKeys(1);
                gameManagerObject.SendMessage("levelEnded");
                Debug.Log("pcExitLevel");
                break;
            case "blueKey":
                Destroy(other.gameObject);
                inventory.addKeys(2);
                gameManagerObject.SendMessage("levelEnded");
                Debug.Log("pcExitLevel");
                break;

        }
    }

    private void passengerMessageSwitch(string name)
    {
        switch (name)
        {
            //Level triggers
            case "intro1":
                passenger.behaviorSetter("intro1");
                break;
            case "intro2":
                passenger.behaviorSetter("intro2");
                break;
            case "intro3":
                passenger.behaviorSetter("intro3");
                break;
            case "intro4":
                passenger.behaviorSetter("intro4");
                break;
            case "level2_1":
                passenger.behaviorSetter("level2_1");
                break;
            case "level2_2":
                passenger.behaviorSetter("level2_2");
                break;
            case "level2_3":
                passenger.behaviorSetter("level2_3");
                break;
            case "level3_1":
                passenger.behaviorSetter("level3_1");
                break;
            case "level3_2":
                passenger.behaviorSetter("level3_2");
                break;
            case "level3_3":
                passenger.behaviorSetter("level3_2");
                break;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            GetComponent<Transform>().position = respawnPoint;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                inventory.TakeDamage(10);
                GetComponent<Rigidbody>().AddForce(transform.forward * -1 * 5000);
                break;
        }
    }
    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }
}
