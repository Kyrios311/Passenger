using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class Inventory : MonoBehaviour
{
    
    public bool paused;

    private float maxHealth = 100;
    public float health = 100;
    private float damageTimer = 2f; 
    public bool canBeDamaged = true;
    public Slider healthSlider;
    public GameObject healthSliderFill;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.1f);
    public bool isDead, damaged;

    AudioSource playerAudio;
    public GameObject player;
    playerController playerController;
    public GameObject playerCam;
    public Camera cam;
    mouseLook mouse;

    //Inventory
    public string playerName;
    public bool showInventory;
    private Rect inventoryRect = new Rect((Screen.width / 2) - (Screen.width/3), (Screen.height / 2) - (Screen.height / 3), 250, 350);
    private int InventorySize = 12;
    private int itemEquipped = 0; //1=Sword, 2=bow, 3=staff
    private int[] keys = new int[3];
    private int[] invItems = new int[12]; //0=Empty, 1=Sword, 2=HealthPot, 3= key, 4=bow, 5=coin, 6=toy, 7=crystal, 8 = restore, 9 staff
    private int[] quickItems = new int[4];
    private int[] quickItemHold;
    public Texture[] invTextures; //0=Empty, 1=Sword, 2=HealthPot, 3= key, 4=bow, 5=coin, 6=toy 7=crystal, 8 = restore, 9 staff

    RaycastHit hit = new RaycastHit();
    public Text inventoryText;
    private float inventoryTextTimer = 5f;
    private float fireTimer = 1f;
    private bool canFire = false;
    public float isPullingTimer = 0f;

    public GameObject playerSword, playerBow, playerStaff, playerArrow, playerBowArrow, playerFireball, fireballSpawn;
    public BowString playerBowString;

    public Animator swordAnim, staffAnim;
    int swordAttackHash = Animator.StringToHash("SwordSwing");
    int staffAttackHash = Animator.StringToHash("StaffAttack");

    //dev variables
    bool godMode = false; //Invulnerable

    void Awake()
    {
        swordAnim = playerSword.GetComponent<Animator>();
        staffAnim = playerStaff.GetComponent<Animator>();
        playerController = player.GetComponent<playerController>();
        mouse = playerCam.GetComponent<mouseLook>();
        playerAudio = GetComponent<AudioSource>();
        quickItemHold = new int[1];
        quickItemHold[0] = 0;
    }

    void Start()
    {
        healthSlider.value = health;
        /*for (int i = 0; i < InventorySize; i++)
        {
            setInventory(i, 0);

            if (i < quickItems.Length)
            {
                quickItems[i] = getInventory(i);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (damageTimer < 2)
            {
                damageTimer = damageTimer + Time.deltaTime;
            }
            else
            {
                canBeDamaged = true;
            }
            if (fireTimer < 1)
            {
                fireTimer = fireTimer + Time.deltaTime;
            }
            else
            {
                canFire = true;
            }
            if (damaged)
            {
                damageImage.color = flashColor;
            }
            else
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
            damaged = false;

            if (health > maxHealth)
            {
                health = 100;
            }

            if (Input.GetButtonDown("Inventory"))
            {
                if (showInventory == false)
                {
                    showInventory = true;
                    mouse.inventory = true;
                }
                else
                {
                    showInventory = false;
                    mouse.inventory = false;
                }
            }

            //Player Quickitem Handling
            if (Input.GetKeyDown("1") && quickItems[0] != 0)
            {
                UseQuickItem(0);
            }
            if (Input.GetKeyDown("2") && quickItems[1] != 0)
            {
                UseQuickItem(1);
            }
            if (Input.GetKeyDown("3") && quickItems[2] != 0)
            {
                UseQuickItem(2);
            }
            if (Input.GetKeyDown("4") && quickItems[3] != 0)
            {
                UseQuickItem(3);
            }

            if(!showInventory && itemEquipped != 0)
            {
                //1=Sword, 2=bow, 3=staff
                if (itemEquipped == 1 && Input.GetButtonDown("Fire1") && canFire)
                {
                    if (Input.GetButtonDown("Fire1") && canFire)
                    {
                        canFire = false;
                        fireTimer = 0;
                        swordAnim.SetTrigger(swordAttackHash);
                        //playerSword.GetComponent<Collider>().enabled = true;

                    }

                }

                else if(itemEquipped == 2)
                {
                    if (Input.GetButton("Fire1") && canFire)
                    {
                        playerBowString.releasing = false;
                        playerBowString.stretching = true;
                        isPullingTimer = isPullingTimer + Time.deltaTime;
                    }
                    if (Input.GetButtonUp("Fire1") && isPullingTimer >= 1)
                    {
                        playerBowString.releasing = true;
                        GameObject newArrow;
                        newArrow = Instantiate(playerArrow, playerBowArrow.transform.position, playerBowArrow.transform.rotation);
                        newArrow.GetComponent<Rigidbody>().AddRelativeForce(0,0,-2000);
                        isPullingTimer = 0;
                    }
                    if (Input.GetButtonUp("Fire1") && isPullingTimer < 1)
                    {
                        isPullingTimer = 0;
                        playerBowString.releasing = true;

                    }
                }

                else if(itemEquipped == 3)
                {
                    if((Input.GetButtonDown("Fire1") && canFire))
                    {
                        canFire = false;
                        fireTimer = 0;
                        staffAnim.SetTrigger(staffAttackHash);
                        GameObject newFireball;
                        newFireball = Instantiate(playerFireball, fireballSpawn.transform.position, fireballSpawn.transform.rotation);
                        newFireball.GetComponent<Rigidbody>().AddRelativeForce(0, 0, 2000);
                    }
                }

            }


            if (Input.GetKeyDown("o") && Input.GetKey("p"))
            {
                if (!godMode)
                {
                    inventoryText.text = "God mode on";
                    inventoryTextTimer = 0f;
                    godMode = true;
                    healthSliderFill.GetComponent<Image>().color = new Color(0, 0, 1, 255);
                }
                else
                {
                    inventoryText.text = "God mode off";
                    inventoryTextTimer = 0f;
                    godMode = false;
                    healthSliderFill.GetComponent<Image>().color = new Color(0, 1, 0, 255);
                }

            }

            if (godMode)
            {
                health = 100;
                healthSlider.value = health;
            }

            //Inventory messages
            if (inventoryTextTimer <= 5)
            {
                inventoryTextTimer = inventoryTextTimer + Time.deltaTime;
            }
            else
            {
                inventoryText.text = "";
            }
        }
        else
        {
            damageImage.color = flashColor;
        }
    }

    public void TakeDamage(int amount)
    {
        if (canBeDamaged == true)
        {
            damaged = true;

            health -= amount;
            healthSlider.value = health;
            playerAudio.Play();
            canBeDamaged = false;
            damageTimer = 0f;
            if (health <= 0 && !isDead)
            {
                Death();
            }
            
        }

    }
    void Death()
    {
        isDead = true;

        transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        playerAudio.clip = deathClip;
        playerAudio.Play();
        playerController.enabled = false;
        mouse.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    //Key functions
    public void setKeys(int[] newKeys)
    {
        keys = newKeys;
    }
    public int[] getKeys()
    {
        return keys;
    }
    public void addKeys(int value)
    {
        keys[value] = 1;
    }

    //Health functions
    public void setHealth(float hp)
    {
        health = hp;
    }
    public float getHealth()
    {
        return health;
    }
    public void addHealth(int add)
    {
        health = health + add;
        healthSlider.value = health;
    }

    //Inventory code begin --------------------
    public int addToInventory(int newItem)
    {
        for (int i = 0; i < invItems.Length; i++)
        {
            if (invItems[i] == 0)
            {
                invItems[i] = newItem;
                itemPickupMessage(newItem);
                inventoryTextTimer = 0;
                return 1;
            }
        }
        inventoryText.text = "Inventory full";
        inventoryTextTimer = 0;
        return 0;
    }

    public void itemPickupMessage(int newItem) //0=Empty, 1=Sword, 2=HealthPot, 3= key, 4=bow, 5=coin, 6=toy, 7=crystal, 8 = restore, 9 staff
    {
        switch(newItem)
        {
            case 1:
                inventoryText.text = "You have picked up a sword!";
                break;
            case 2:
                inventoryText.text = "You have picked up a health potion!";
                break;
            case 3:
                inventoryText.text = "You have picked up a chest key!";
                break;
            case 4:
                inventoryText.text = "You have picked up a bow!";
                break;
            case 5:
                inventoryText.text = "Perhaps someone wants this...?";
                break;
            case 6:
                inventoryText.text = "Perhaps someone wants this...?";
                break;
            case 7:
                inventoryText.text = "Perhaps someone wants this...?";
                break;
            case 8:
                inventoryText.text = "You have picked up a restore potion!";
                break;
            case 9:
                inventoryText.text = "A staff of great power!";
                break;
        }
    }
    public void setInventory(int itemSlot, int itemSet)
    {
        if (invItems != null)
        {
            invItems[itemSlot] = itemSet;
        }
        else
        {
            Debug.Log("Warning, invItems null");
        }
            
    }

    public int getInventory(int itemSlot)
    {
        if (invItems != null)
        {
            if (itemSlot < InventorySize)
            {
                return invItems[itemSlot];
            }
            else return 0;

        }
        else
        return 0;
    }

    void RemoveFromInventory(int Item)
    {
        for (int i = 0; i < invItems.Length; i++)
        {
            if (invItems[i] != 0)
            {
                if (invItems[i] == Item)
                {
                    invItems[i] = 0;
                    break;
                }
            }
        }

    }

    public int[] getInventory()
    {
        return invItems;
    }

    public void setNewInventory(int[] newInv)
    {
        invItems = newInv;
        Debug.Log(invItems[0]);
    }
    //Check to see if item is present in inventory
    bool checkInventory(int itemCheck)
    {
        for (int i = 0; i < invItems.Length; i++)
        {
            if (invItems[i] == itemCheck)
            {
                return true;
            }

        }
        return false;
    }

    public int[] getQuickItems()
    {
        return quickItems;
    }
    public void setQuickItems(int[] newQuickItems) //Sets quickitems from oad
    {
        quickItems = newQuickItems;
    }

    public void setQuickItem(int quickItem)
    {
        for (int x = 0; x < quickItems.Length; x++)
        {
            if (quickItems[x] == quickItem)
            {
                break;
            }
            else if (quickItems[x] != quickItem && quickItems[x] == 0)
            {
                quickItems[x] = quickItem;
                break;
            }
        }
    }
    void UseQuickItem(int itemNum)
    {
        switch (itemNum)
        {
            case 0:
                itemType(quickItems[0]);
                if (checkInventory(quickItems[0]) == false)
                {
                    quickItems[0] = 0;
                }
                break;
            case 1:
                itemType(quickItems[1]);
                if (checkInventory(quickItems[1]) == false)
                {
                    quickItems[1] = 0;
                }
                break;
            case 2:
                itemType(quickItems[2]);
                if (checkInventory(quickItems[2]) == false)
                {
                    quickItems[2] = 0;
                }
                break;
            case 3:
                itemType(quickItems[3]);
                if (checkInventory(quickItems[3]) == false)
                {
                    quickItems[3] = 0;
                }
                break;
        }
    }
    void itemType(int item)
    {
        //0=Empty, 1=Sword, 2=HealthPot, 3= key, 4=bow, 5=coin, 6=toy, 7=crystal, 8 = restore, 9 staff
        switch (item)
        {
            case 0:
                break;
            case 1:
                playerStaff.SetActive(false);
                playerBow.SetActive(false);
                playerSword.SetActive(true);
                swordAnim.Play("SwordEquip");
                itemEquipped = 1;
                break;
            case 2:
                addHealth(30);
                RemoveFromInventory(2);
                break;
            case 3:
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 5f) && hit.transform.tag == "Chest")
                {
                    hit.transform.SendMessage("Open");
                    RemoveFromInventory(3);
                }
                break;
            case 4:
                playerStaff.SetActive(false);
                playerSword.SetActive(false);
                playerBow.SetActive(true);
                itemEquipped = 2;
                break;
            case 5:
                Ray ray1 = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray1, out hit, 5f) && hit.transform.tag == "isGreed")
                {
                    hit.transform.SendMessage("Open");
                }
                break;
            case 6:
                Ray ray2 = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray2, out hit, 5f) && hit.transform.tag == "isGreed")
                {
                    hit.transform.SendMessage("Open");
                }
                break;
            case 7:
                Ray ray3 = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray3, out hit, 5f) && hit.transform.tag == "isGreed")
                {
                    hit.transform.SendMessage("Open");
                }
                break;
            case 8:
                addHealth(100);
                RemoveFromInventory(8);
                break;
            case 9:
                playerSword.SetActive(false);
                playerBow.SetActive(false);
                playerStaff.SetActive(true);
                staffAnim.Play("StaffEquip");
                itemEquipped = 3;
                break;
        }
    }
    //Inventory code end --------------------


    public void OnGUI()
    {
        if (showInventory && !paused)
        {
            inventoryRect = GUI.Window(11, inventoryRect, InventoryGUI, "Inventory");
        }

        if(!paused)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 275, Screen.height / 2 + 175, 50f, 50f), "<-"))
            {
                quickItemHold[0] = quickItems[0];
                quickItems[0] = quickItems[1];
                quickItems[1] = quickItems[2];
                quickItems[2] = quickItems[3];
                quickItems[3] = quickItemHold[0];
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 225, Screen.height / 2 + 175, 100f, 100f), invTextures[quickItems[0]]) && quickItems[0] != 0)
            {
                UseQuickItem(0);
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2 + 175, 100f, 100f), invTextures[quickItems[1]]) && quickItems[1] != 0)
            {
                UseQuickItem(1);
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 25, Screen.height / 2 + 175, 100f, 100f), invTextures[quickItems[2]]) && quickItems[2] != 0)
            {
                UseQuickItem(2);
            }
            if (GUI.Button(new Rect(Screen.width / 2 + 75, Screen.height / 2 + 175, 100f, 100f), invTextures[quickItems[3]]) && quickItems[3] != 0)
            {
                UseQuickItem(3);
            }
            if (GUI.Button(new Rect(Screen.width / 2 + 175, Screen.height / 2 + 175, 50f, 50f), "->"))
            {
                quickItemHold[0] = quickItems[3];
                quickItems[3] = quickItems[2];
                quickItems[2] = quickItems[1];
                quickItems[1] = quickItems[0];
                quickItems[0] = quickItemHold[0];
            }
            if (GUI.Button(new Rect(Screen.width / 2 + 175, Screen.height / 2 + 125, 50f, 50f), "Clear"))
            {
                quickItems[0] = 0;
                quickItems[1] = 0;
                quickItems[2] = 0;
                quickItems[3] = 0;
            }
        }
    }

    void InventoryGUI(int ID)
    {
        //0=Empty, 1=Sword, 2=HealthPot, 3= key, 4=bow, 5=coin, 6=toy 7=crystal, 8 = restore, 9 staff
        GUILayout.BeginArea(new Rect(5, 25, 300, 400));
        GUILayout.BeginHorizontal(); //Row 1
        if (GUILayout.Button(invTextures[invItems[0]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if(invItems[0] != 1 && invItems[0] != 4 && invItems[0] != 9)
            {
                setQuickItem(invItems[0]);
            }
            else
            {
                itemType(invItems[0]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[1]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[1] != 1 && invItems[1] != 4 && invItems[1] != 9)
            {
                setQuickItem(invItems[1]);
            }
            else
            {
                itemType(invItems[1]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[2]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[2] != 1 && invItems[2] != 4 && invItems[2] != 9)
            {
                setQuickItem(invItems[2]);
            }
            else
            {
                itemType(invItems[2]);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); //Row 2
        if (GUILayout.Button(invTextures[invItems[3]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[3] != 1 && invItems[3] != 4 && invItems[3] != 9)
            {
                setQuickItem(invItems[3]);
            }
            else
            {
                itemType(invItems[3]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[4]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[4] != 1 && invItems[4] != 4 && invItems[4] != 9)
            {
                setQuickItem(invItems[4]);
            }
            else
            {
                itemType(invItems[4]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[5]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[4] != 1 && invItems[4] != 4 && invItems[4] != 9)
            {
                setQuickItem(invItems[5]);
            }
            else
            {
                itemType(invItems[5]);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); //Row 3
        if (GUILayout.Button(invTextures[invItems[6]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[5] != 1 && invItems[5] != 4 && invItems[5] != 9)
            {
                setQuickItem(invItems[6]);
            }
            else
            {
                itemType(invItems[6]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[7]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[6] != 1 && invItems[6] != 4 && invItems[6] != 9)
            {
                setQuickItem(invItems[7]);
            }
            else
            {
                itemType(invItems[7]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[8]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[7] != 1 && invItems[7] != 4 && invItems[7] != 9)
            {
                setQuickItem(invItems[8]);
            }
            else
            {
                itemType(invItems[8]);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); //Row 4
        if (GUILayout.Button(invTextures[invItems[9]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[8] != 1 && invItems[9] != 4 && invItems[8] != 9)
            {
                setQuickItem(invItems[9]);
            }
            else
            {
                itemType(invItems[9]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[10]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[9] != 1 && invItems[9] != 4 && invItems[9] != 9)
            {
                setQuickItem(invItems[9]);
            }
            else
            {
                itemType(invItems[9]);
            }
        }
        if (GUILayout.Button(invTextures[invItems[11]], GUILayout.Height(75), GUILayout.Width(75)))
        {
            if (invItems[9] != 1 && invItems[9] != 4 && invItems[9] != 9)
            {
                setQuickItem(invItems[9]);
            }
            else
            {
                itemType(invItems[9]);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

    }







    }




