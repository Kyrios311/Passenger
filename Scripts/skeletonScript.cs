using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum skeletonBehaviors { Idle, Guard, Combat };

public class skeletonScript : MonoBehaviour
{

    public float hp = 100;
    public bool isSuspicious = false;
    public skeletonBehaviors aiBehaviors = skeletonBehaviors.Idle;

    public bool hasItem = false;
    public GameObject dropItem;
    public bool isInRange = false;
    private bool FightsRanged = false;
    public bool isStationary;
    private float nextShot;
    public float shootTimer = 3f;

    public List<KeyValuePair<string, int>> Stats = new List<KeyValuePair<string, int>>();
    public GameObject Projectile;
    public GameObject Player;
    private float distanceToPlayer;
    public Transform spellSpawn;

    public Transform[] Waypoints;
    public int curWaypoint = 0;
    bool ReversePath = false;

    UnityEngine.AI.NavMeshAgent navAgent;
    Vector3 Destination;
    float Distance;

    private Animator anim;
    int IdleHash = Animator.StringToHash("IdleAnim");
    int AttackHash = Animator.StringToHash("AttackAnim");
    int WalkHash = Animator.StringToHash("WalkAnim");
    int DeathHash = Animator.StringToHash("DeathAnim");
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        RunBehaviors();
        distanceToPlayer = Vector3.Distance(gameObject.transform.position, Player.transform.position);
        if (Vector3.Distance(gameObject.transform.position, Player.transform.position) < 10)
        {
            isSuspicious = true;
        }
        else
        {
            isSuspicious = false;
        }
        if (hp <= 0)
        {
            tag = "Untagged";
            Destroy(navAgent);
            Destroy(GetComponent<BoxCollider>());
            anim.SetTrigger(DeathHash);
            DropItem();
            Destroy(GetComponent<skeletonScript>());
        }
    }

    void RunBehaviors()
    {
        switch (aiBehaviors)
        {
            case skeletonBehaviors.Idle:
                RunIdleNode(); break;
            case skeletonBehaviors.Guard:
                RunGuardNode(); break;
            case skeletonBehaviors.Combat:
                RunCombatNode(); break;
        }
    }
    void ChangeBehavior(skeletonBehaviors newBehavior)
    {
        aiBehaviors = newBehavior;
        RunBehaviors();
    }

    //States
    void RunIdleNode()
    {
        Idle();
    }
    void RunGuardNode()
    {
        Guard();
    }
    void RunCombatNode()
    {
        Combat();
    }

    //State code
    void Idle()
    {
        anim.SetTrigger(IdleHash);
        if (isSuspicious)
        {
            SearchForTarget();
        }
    }
    void Guard()
    {
        if (isSuspicious)
        {
            SearchForTarget();
        }
        else
        {
            Patrol();
        }
    }
    void Combat()
    {
        transform.LookAt(Player.transform);
        if (CanShoot())
        {
            if (FightsRanged && distanceToPlayer <= 5)
            {
                Destination = transform.position;
                navAgent.SetDestination(Destination);
                RangedAttack();
                nextShot = Time.time + shootTimer;
            }
            if (FightsRanged && distanceToPlayer > 5)
            {
                Destination = Player.transform.position;
                navAgent.SetDestination(Destination);
            }
            if (!FightsRanged && distanceToPlayer < 2)
            {
                anim.SetTrigger(AttackHash);
                Destination = transform.position;
                navAgent.SetDestination(Destination);
                nextShot = Time.time + shootTimer;
            }
            if (!FightsRanged && distanceToPlayer > 2)
            {
                anim.SetTrigger(WalkHash);
                Destination = Player.transform.position;
                navAgent.SetDestination(Destination);
            }
        }
        if (distanceToPlayer > 10)
        {
            ChangeBehavior(skeletonBehaviors.Guard);
        }

    }

    void SearchForTarget()
    {
        anim.SetTrigger(WalkHash);
        Destination = Player.transform.position;
        navAgent.SetDestination(Destination);
        Distance = Vector3.Distance(gameObject.transform.position, Destination);
        if (Distance < 10) ChangeBehavior(skeletonBehaviors.Combat);

    }

    void Patrol()
    {

        Distance = Vector3.Distance(gameObject.transform.position, Waypoints[curWaypoint].position);
        if (Distance > 2.00f)
        {
            anim.SetTrigger(WalkHash);
            Destination = Waypoints[curWaypoint].position;
            navAgent.SetDestination(Destination);
        }
        else
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
            else if(isStationary)
            {
                anim.SetTrigger(IdleHash);
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

    void RangedAttack()
    {
        /*GameObject newProjectile;
        newProjectile = Instantiate(Projectile, spellSpawn.position, spellSpawn.rotation);*/
    }

    void ChangeHealth(int Amount)
    {
        if (Amount < 0)
        {
            if (!isSuspicious)
            {
                isSuspicious = true;
                ChangeBehavior(skeletonBehaviors.Guard);
            }
        }
        for (int i = 0; i < Stats.Capacity; i++)
        {
            if (Stats[i].Key == "Health")
            {
                int tempValue = Stats[i].Value; Stats[i] = new KeyValuePair<string, int>(Stats[i].Key, tempValue += Amount);
                if (Stats[i].Value <= 0)
                {
                    Destroy(gameObject);
                }
                break;
            }
        }
    }

    public bool CanShoot()
    {
        bool canShoot = true;
        if (Time.time < nextShot)
        {
            canShoot = false;
        }
        return canShoot;
    }

    void takeDamage(int x)
    {
        hp = hp - x;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            takeDamage(50);
        }
    }

    void DropItem()
    {
        if(hasItem)
        {
            Instantiate(dropItem, this.transform);
        }
    }
}
