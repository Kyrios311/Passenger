using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum Behaviors { Idle, Speech, Move };


public class Passenger : MonoBehaviour {

    //Message handles
    bool isPassenger = true, isGreed = false, isUnknown = false;
    public bool inLevel = false;
    public Texture passengerImage, greedImage, unknownImage;
    public Text messageText;
    private float messageTimer = 5f;
    private bool isLooking; //Talking to/looking at player
    private Rect speechRect = new Rect((Screen.width * 0.5f) - 100, 0, 300, 100);

    //Movement handles
    public Transform[] Waypoints;
    public int curWaypoint = 0;
    public bool ReversePath = false;
    UnityEngine.AI.NavMeshAgent navAgent;
    Vector3 Destination;
    float distance;
    
    public Behaviors aiBehaviors = Behaviors.Idle;
    public GameObject player;
    public Inventory playerInv;

    Animator anim;
    int idleHash = Animator.StringToHash("Idle");
    int speechHash = Animator.StringToHash("Speech");

    // Use this for initialization
    void Start () {
        anim = GetComponentInChildren<Animator>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        messageText.text = "";
    }
	
	// Update is called once per frame
	void Update () {

        if(isLooking)
        {
            //transform.LookAt(player);
            //transform.rotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), Time.deltaTime);
        }
        RunBehaviors();
        //If message timer > 5, hide message, else add
        if (messageTimer < 5)
        {
            messageTimer = messageTimer + Time.deltaTime;
        }
        else if(aiBehaviors != Behaviors.Idle)
        {
            aiBehaviors = Behaviors.Move;
            isLooking = false;
            messageText.text = "";
        }
    }

    public void behaviorSetter(string playerMessage)
    {
        speechSet(playerMessage);
        aiBehaviors = Behaviors.Speech;
    }

    void RunBehaviors()
    {
        switch (aiBehaviors)
        {
            case Behaviors.Idle:
                RunIdleNode(); break;
            case Behaviors.Speech:
                RunSpeechNode(); break;
            case Behaviors.Move:
                RunMoveNode(); break;
        }

    }

    void ChangeBehavior(Behaviors newBehavior)
    {
        aiBehaviors = newBehavior;
        RunBehaviors();
    }

    //State nodes
    void RunIdleNode()
    {
        Idle();
    }
    void RunSpeechNode()
    {
        Speech();
    }
    void RunMoveNode()
    {
        Move();
    }

    //State code
    void Idle()
    {
        anim.SetTrigger(idleHash);
    }
    void Speech()
    {
        isLooking = true;
        anim.SetTrigger(speechHash);
    }
    void Move()
    {
        anim.SetTrigger(idleHash);
        distance = Vector3.Distance(gameObject.transform.position, Waypoints[curWaypoint].position);
        Destination = Waypoints[curWaypoint].position;
        navAgent.SetDestination(Destination);
        if (distance > 6.5f)
        {
            Destination = Waypoints[curWaypoint].position;
            navAgent.SetDestination(Destination);
        }
        else if(inLevel && distance <= 7.4)
        {
            aiBehaviors = Behaviors.Idle;
        }
        else if(!inLevel)
        {
            if (ReversePath)
            {
                if (curWaypoint <= 0)
                {
                    ReversePath = false;
                }
                else
                {
                    curWaypoint--;
                    Destination = Waypoints[curWaypoint].position;
                }
            }
            else
            {
                if (curWaypoint >= Waypoints.Length - 1)
                {
                    ReversePath = true;
                }
                else
                {
                    curWaypoint++;
                    Destination = Waypoints[curWaypoint].position;
                }
            }
        }
        
    }

    void OnGUI()
    {
        var centeredStyle = GUI.skin.GetStyle("Window");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.wordWrap = true;
        if (messageTimer < 5 && isPassenger)
        {
            speechRect = GUI.Window(7, speechRect, SpeechGUI, "Passenger", centeredStyle);
        }
        else if (messageTimer < 5 && isUnknown)
        {
            speechRect = GUI.Window(7, speechRect, SpeechGUI, "Unknown", centeredStyle);
        }
        else if (messageTimer < 5 && isGreed)
        {
            speechRect = GUI.Window(7, speechRect, SpeechGUI, "Passenger", centeredStyle);
        }
    }

    void SpeechGUI(int ID)
    {
        GUILayout.BeginArea(new Rect(0, 0, 500, 500));
        if(isPassenger)
        {
            GUI.DrawTexture(new Rect(10, 20, 75, 75), passengerImage);
        }
        else if(isGreed)
        {
            GUI.DrawTexture(new Rect(10, 20, 75, 75), greedImage);
        }
        else if(isUnknown)
        {
            GUI.DrawTexture(new Rect(10, 20, 75, 75), unknownImage);
        }
        GUI.Label(new Rect(90, 20, 200, 200), messageText.text);
        GUILayout.EndArea();
    }

    void speechSet(string speechType)
    {
        isLooking = true;
        switch (speechType)
        {
            //Intro
            case "intro1":
                messageTimer = 0;
                isPassenger = false;
                isUnknown = true;
                messageText.text = (playerInv.playerName + "...Awaken...");
                break;
            case "intro2":
                messageTimer = 0;
                isUnknown = false;
                isPassenger = true;
                messageText.text = "Look who got up at last. Stay here " + playerInv.playerName + ", you still seem tired. Let me stay in control of outside...";
                curWaypoint = curWaypoint + 1;
                break; 
            case "intro3":
                messageTimer = 0;
                messageText.text = "We both know there's no point in going for the keys, just lie back down.";
                curWaypoint = curWaypoint + 1;
                break;
            case "intro4":
                messageTimer = 0;
                messageText.text = "No!... Put that back before you get hurt...";
                curWaypoint = curWaypoint + 1;
                break;
            case "level2_1":
                messageTimer = 0;
                messageText.text = "Listen " + playerInv.playerName + ", if you won't turn back, I will hurt you. Not in here. Out THERE...";
                break;
            case "level2_2":
                messageTimer = 0;
                messageText.text = playerInv.playerName + " you don't get it do you?";
                curWaypoint = curWaypoint + 1;
                break;
            case "level2_3":
                messageTimer = 0;
                messageText.text = "PUT THE KEYS BACK...";
                break;
            case "level3_1":
                messageTimer = 0;
                messageText.text = "If you won't listen to threats against yourself, I will threaten the \"others\" outside...";
                curWaypoint = curWaypoint + 1;

                break;
            case "level3_2":
                messageTimer = 0;
                messageText.text = "You still persist you weak fool?! Fine, come then.";
                curWaypoint = curWaypoint + 1;

                break;
            case "level3_3":
                messageTimer = 0;
                messageText.text = "After you wither " + playerInv.playerName + ", I will destroy your life outside!";
                break;
            case "level3_4":
                messageTimer = 0;
                messageText.text = "Trust me, I won't kill you, I'll just make it hurt... A LOT.";
                break;
            case "level3_5":
                messageTimer = 0;
                messageText.text = "Damn you " +playerInv.playerName + "... but know, I will always be inside of you, gnawing on the inside. This isn't over.";
                break;


        }
    }

}
