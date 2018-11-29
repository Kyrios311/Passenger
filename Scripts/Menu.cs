using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Menu : MonoBehaviour {
    //Default player stats
    public bool paused;

    private float health = 100;
    private int[] keys = new int[3];
    private string playerName = "John";
    private int InventorySize = 12;
    private int[] invItems = new int[12]; //0=Empty, 1=Sword, 2=HealthPot, 3= key, 4=bow, 5=coin, 6=toy, 7=crystal, 8 = HealthFill, 9 staff
    private int[] quickItems = new int[4];
    //------------------

    bool showMenu = true;
    bool showNew = false;
    bool showCredits = false;
    public Texture titlePic;
    public Texture newGame;
    public Texture loadGame;
    // Use this for initialization
    void Start () {
        InitializeInventory();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitializeInventory()    //Initialize/Load Inventory/Quickslots
    {
        for (int i = 0; i < InventorySize; i++)
        {
            invItems[i] = 0;

            if (i < quickItems.Length)
            {
                quickItems[i] = invItems[i];
            }
        }
    } 

    public void OnGUI()
    {
        if (showMenu)
        {
            GUI.Label(new Rect((Screen.width / 2) - (Screen.width / 3), Screen.height / 2 - 200, 300f, 300f), titlePic);
            if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width / 4), Screen.height / 2 - 75, 100f, 50f), newGame))
            {
                showMenu = false;
                showNew = true;
            }

            if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
            {
                if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width / 4), Screen.height / 2, 100f, 50f), loadGame))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
                    PlayerData data = (PlayerData)bf.Deserialize(file);
                    keys = data.saveKeys;
                    file.Close();
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
            // }
            if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width / 4), Screen.height / 2 + 75, 50f, 50f), "Credits"))
            {
                showMenu = false;
                showCredits = true;
            }
            if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width / 4) + 50, Screen.height / 2 + 75, 50f, 50f), "Quit"))
            {
                Application.Quit();
            }
        }
        else if (showNew)
        {
            playerName = GUI.TextField(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 60), playerName, 25);
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 75, 50f, 50f), "Accept"))
            {

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
                PlayerData data = new PlayerData();

                data.saveHealth = health;
                data.saveKeys = keys;
                data.playerName = playerName;
                data.saveInvItems = invItems;
                data.saveQuickItems = quickItems;
                bf.Serialize(file, data);
                file.Close();
                SceneManager.LoadScene("level_01");
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 + 75, 50f, 50f), "Back"))
            {
                showNew = false;
                showMenu = true;
            }
        }
        else if (showCredits)
        {
            if(GUI.Button(new Rect((Screen.width / 2) - (Screen.width / 4), Screen.height / 2 + 75, 50f, 50f), "Back"))
            {
                showCredits = false;
                showMenu = true;
            }
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 60, 200, 60), "https://www.freesound.org/people/Audionautics/sounds/133901/ (Audionautics) - Lava");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 60), "https://www.freesound.org/people/RensvdMeijs/sounds/376771/ (RensvdMeijs) - Choir Rise");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 60, 200, 60), "Level 2, A Timelapse - DL Sounds");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 120, 200, 60), "Thanks to Yughues - Terrain Materials and Rock packs");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 -180, 200, 60), "Fi Silva - Low Poly RPG Item Pack");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 -240, 200, 60), "Unity Answers - Code Assistance");

        }
    }
}
