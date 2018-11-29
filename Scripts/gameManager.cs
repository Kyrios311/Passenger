using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
public class gameManager : MonoBehaviour {
    public bool paused;

    public GameObject invObject; //Reference to object holding inventory
    Inventory inventory;
    private int InventorySize = 12;
    private int[] keys;

    public GameObject controllerObject;
    playerController playerController;

    public GameObject cameraObject;
    mouseLook mouse;

    //Level specific intro screen
    private bool intro, outro, nextLevel;
    public Image whiteImage;
    private Color whiteColor = new Color(1f, 1f, 1f, 1f);
    private float timer = 4f;

    private void Awake()
    {
        inventory = invObject.GetComponent<Inventory>();
        playerController = controllerObject.GetComponent<playerController>();
        mouse = cameraObject.GetComponent<mouseLook>();
    }

    // Use this for initialization
    void Start () {
        whiteImage.color = whiteColor;
        intro = true;
        Load();
        keys = inventory.getKeys();
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("escape"))
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
                paused = true;
                inventory.paused = true;
                playerController.paused = true;
                mouse.paused = true;
            }
                
            /* if(Time.timeScale == 0)
             {
                 Time.timeScale = 1;
                 paused = false;
                 inventory.paused = false;
                 playerController.paused = false;
                 mouse.paused = false;
             }
             else
             {
                 Time.timeScale = 0;
                 paused = true;
                 inventory.paused = true;
                 playerController.paused = true;
                 mouse.paused = true;
             }
             */
        }
        if(!paused)
        {
            InventoryScan();
        }

        if (intro)
        {
            whiteImage.color = Color.Lerp(whiteImage.color, Color.clear, Time.deltaTime * 1);
            if (whiteImage.color == Color.clear)
            {
               
                intro = false;
            }
        }
        else if (outro)
        {
            timer = timer + Time.deltaTime;
            if (inventory.isDead)
            {
                outro = false;
            }
            whiteImage.color = Color.Lerp(whiteImage.color, whiteColor, Time.deltaTime * 4);
            if (timer >= 4)
            {
                outro = false;
                nextLevel = true;
            }
        }
        else if (nextLevel)
        {
            Save();
            keys = inventory.getKeys();
            if (keys[2] == 1)
            {
                SceneManager.LoadScene("menu"); //Forgiveness
            }
            else if (keys[1] == 1)
            {
                SceneManager.LoadScene("menu"); //Time
            }
            else if (keys[0] == 1)
            {
                SceneManager.LoadScene("level_02"); //Acceptance
            }
            else
            {
                SceneManager.LoadScene("level_01"); //Realization
            }
        }
	}

    public void levelEnded()
    {
        intro = false;
        outro = true;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.playerName = inventory.playerName;
        data.saveHealth = inventory.getHealth();
        data.saveKeys = inventory.getKeys();
        data.saveInvItems = inventory.getInventory();
        Debug.Log(data.saveInvItems[0]);
        data.saveQuickItems = inventory.getQuickItems();
        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            Debug.Log(data.saveInvItems[0]);
            inventory.playerName = data.playerName;
            inventory.setHealth(data.saveHealth);
            inventory.setKeys(data.saveKeys);
            inventory.setNewInventory(data.saveInvItems);
            inventory.setQuickItems(data.saveQuickItems);
            keys = data.saveKeys;
            file.Close();
        }
    }

    void InitializeInventory()    //Initialize/Load Inventory/Quickslots
    {
        for (int i = 0; i < InventorySize; i++)
        {
            inventory.setInventory(i, 0);

            if (i < 4)
            {
                inventory.setQuickItem(i);
            }
        }
    }

    public void InventoryScan()
    {
        for (int i = 0; i < InventorySize; i++) //Replaces empty slots with occupied slots
        {
            int y = i + 1;
            if (y < InventorySize)
            {
                if (inventory.getInventory(i) == 0 && inventory.getInventory(y) != 0)
                {
                    inventory.setInventory(i, inventory.getInventory(y));
                    inventory.setInventory(y, 0);
                }
            }
        }
    }

    public void OnGUI()
    {
        if (paused)
        {
            GUI.Window(0, (new Rect((Screen.width * 0.45f), (Screen.height /4), 100, 250)), pausedGUI, "Paused");
        }
        if (inventory.isDead)
        {
            GUI.Window(0, (new Rect((Screen.width * 0.45f), (Screen.height / 4), 100, 250)), deadGUI, "...You died...");
        }

    }

    void pausedGUI(int ID)
    {
        GUILayout.BeginArea(new Rect(0, 0, 100, 600));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Resume", GUILayout.Height(75)))
        {
            Time.timeScale = 1;
            paused = false;
            inventory.paused = false;
            playerController.paused = false;
            mouse.paused = false;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Quit to Menu", GUILayout.Height(75)))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("menu");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Exit to Desktop", GUILayout.Height(75)))
        {
            Application.Quit();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void deadGUI(int ID)
    {
        GUILayout.BeginArea(new Rect(0, 0, 100, 600));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load", GUILayout.Height(75)))
        {
            if (keys[2] == 1)
            {
                SceneManager.LoadScene("menu"); //Forgiveness
            }
            else if (keys[1] == 1)
            {
                SceneManager.LoadScene("level_03"); //Time
            }
            else if (keys[0] == 1)
            {
                SceneManager.LoadScene("level_02"); //Acceptance
            }
            else
            {
                SceneManager.LoadScene("level_01"); //Realization
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Quit to Menu", GUILayout.Height(75)))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("menu");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Exit to Desktop", GUILayout.Height(75)))
        {
            Application.Quit();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

}
[Serializable]
class PlayerData
{
    public string playerName;
    public float saveHealth;
    public int[] saveKeys;
    public int[] saveInvItems;
    public int[] saveQuickItems;
}


