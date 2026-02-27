using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class EntityScript : MonoBehaviour
{
    public enum state { Patrolling, Chasing, Investigating }
    private state currentState = state.Patrolling;

    public Transform player;
    public Transform patrolPoints;
    private Transform spawnpoint;
    private Transform nextPoint;
    public NavMeshAgent agent;

    public static Vector2 facingDirection = Vector2.down;
    public static bool canHear = false;
    public float hearingDistance = 45f;
    public float auraRadius = 30f;
    public GameObject screenEffect;
    public float chaseDuration = 10f;
    public float chaseTimer = 10f;
    private Vector3 lastSetDestination;
    private float destinationUpdateThreshold = 0.5f;
    public static bool playerSpotted = false;
    private bool wasPlayerSeenLastFrame = false;
    private Vector3 investigateTarget;

    public Transform FOV;

    private Vector3 prevPos;
    private SpriteRenderer sr;
    public GameObject faceD;
    public GameObject faceU;
    public GameObject faceL;
    public GameObject faceR;
    public Animator entityAnim;

    public difIncreaseNotScript msgScript;
    public GameObject gameOverScreen;

    private bool onStairs = false;
    private AudioSource[] entitySounds;
    private AudioSource deathSound;
    private AudioSource difIncreaseSound;

    public Tilemap Saferoom;
    public Tilemap Ground;
    private bool playerInSafeRoom;



    void Start() //choose random spawnpoint and first patrol point
    { 
        entitySounds = GetComponents<AudioSource>();
        deathSound = entitySounds[0];
        difIncreaseSound = entitySounds[1];

        msgScript = FindFirstObjectByType<difIncreaseNotScript>();
        agent = GetComponent<NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
       
        int randomIndex = Random.Range(0, patrolPoints.childCount - 3); //last 3 are too close to player for initial spawn
        spawnpoint = patrolPoints.GetChild(randomIndex);
        agent.Warp(spawnpoint.position);
        prevPos = transform.position;
        pickNextPoint();
    }

    void Update()
    {
        Vector3 direction = transform.position - prevPos;
        if (!onStairs)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) //play left or right animation
            {
                if (direction.x > 0) //right
                {
                    faceD.SetActive(false);
                    faceU.SetActive(false);
                    faceL.SetActive(false);
                    faceR.SetActive(true);
                    entityAnim.Play("entityWalkR");
                    facingDirection = Vector2.right;
                }
                else //left
                {
                    faceD.SetActive(false);
                    faceU.SetActive(false);
                    faceL.SetActive(true);
                    faceR.SetActive(false);
                    entityAnim.Play("entityWalkL");
                    facingDirection = Vector2.left;
                }
            }
            else //play up or down animation
            {
                if (direction.y > 0) //up
                {
                    faceD.SetActive(false);
                    faceU.SetActive(true);
                    faceL.SetActive(false);
                    faceR.SetActive(false);
                    entityAnim.Play("entityWalkU");
                    facingDirection = Vector2.up;
                }
                else //down
                {
                    faceD.SetActive(true);
                    faceU.SetActive(false);
                    faceL.SetActive(false);
                    faceR.SetActive(false);
                    entityAnim.Play("entityWalkD");
                    facingDirection = Vector2.down;
                }
            }
        }
        else //Hide entity animations if traversing stairs
        {
            faceD.SetActive(false);
            faceU.SetActive(false);
            faceL.SetActive(false);
            faceR.SetActive(false);
        }

            prevPos = transform.position;

        switch (currentState) //entity state machine
        {
            case state.Patrolling:
                if (playerSpotted)
                {
                    Debug.Log("found player");
                    currentState = state.Chasing;
                }
                else
                {
                    Patrol();
                }
                break;
            case state.Chasing:
                ChasePlayer();
                break;
            case state.Investigating:
                Debug.Log("heard sound, investigating");
                Investigate();
                break;
        }

        if (agent.isOnOffMeshLink) //check if entity is on stairs
        {
            onStairs = true;
        }
        else
            onStairs = false;

        if(Vector3.Distance(player.transform.position, agent.transform.position) <= auraRadius) //check if enemy is close enough for screen aura effect on player
        {
            screenEffect.SetActive(true);
        }
        else
            screenEffect.SetActive(false);

        Vector3Int playerCellPos = Ground.WorldToCell(player.position); //check if the player is in a saferoom
        TileBase safeRoomTile = Saferoom.GetTile(playerCellPos);
        if(safeRoomTile)
        {
            playerInSafeRoom = true;
        }
    }

    private void Investigate()
    {
        if (playerSpotted)
        {
            Debug.Log("Player spotted during investigation!");
            currentState = state.Chasing;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) //reached point of origin, nothing found
        {
            currentState = state.Patrolling;
        }
        else if (agent.destination != investigateTarget && agent.isActiveAndEnabled) //not yet reached point of origin, proceed
        {
            agent.SetDestination(investigateTarget);
        }
    }
    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) //pathfinding logic
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
            {
                Debug.Log("Reached point");
                pickNextPoint();
            }
        }
    }

    private void ChasePlayer()
    {
        float dist = Vector3.Distance(player.position, lastSetDestination);
        if (dist > destinationUpdateThreshold)
        {
            lastSetDestination = player.position;
            if(agent.isActiveAndEnabled && !FOVScript.invisActive) agent.SetDestination(lastSetDestination);
        }

        if (!playerSpotted)
        {
            if (wasPlayerSeenLastFrame)
            {
                wasPlayerSeenLastFrame = false;
                chaseTimer = chaseDuration;
            }

            if(FOVScript.invisActive || playerInSafeRoom)
            {
                chaseTimer = 0f;
            }
            else
                chaseTimer -= Time.deltaTime;
            
            if (chaseTimer <= 0)
            {
                Debug.Log("Ending Chase");
                currentState = state.Patrolling;
                chaseTimer = chaseDuration;
            }
        }
        else
        {
            wasPlayerSeenLastFrame = true;
            if (chaseTimer < chaseDuration)
            {
                chaseTimer = chaseDuration;
            }
        }
    }

    public void pickNextPoint()
    {
        Transform newPoint;
        do
        {
            int index = Random.Range(0, patrolPoints.childCount);
            newPoint = patrolPoints.GetChild(index);
        } while (newPoint == spawnpoint || newPoint == nextPoint);

        nextPoint = newPoint;
        if (agent.isActiveAndEnabled) agent.SetDestination(nextPoint.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == player && !FOVScript.invisActive && !player.GetComponent<playerScript>().freezeActive)
        {
            deathSound.Play();
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ListenForSound(Vector3 origin)
    {
        if (Vector3.Distance(origin, transform.position) < hearingDistance)
        {
            currentState = state.Investigating;
            investigateTarget = origin;
            if (agent.isActiveAndEnabled) agent.SetDestination(origin);
        }

    }

    private void enableStairs() //Enable navMeshLinks between stairs, enabling entity to use them
    {
        NavMeshLink[] allLinks = GameObject.Find("Tilemap_ground").GetComponentsInChildren<NavMeshLink>();
        foreach (NavMeshLink link in allLinks)
            link.enabled = true;
    }


    public void IncreaseDifficulty()
    {
        difIncreaseSound.Play();
        switch(playerScript.keysCollected)
        {
            case 1:
                agent.speed = 5.5f;
                msgScript.ShowMessage("The entity grows faster...");
                break; //speed
            case 2:
                agent.speed = 6;
                agent.acceleration = 22;
                msgScript.ShowMessage("The entity grows faster...");
                break; //speed
            case 3:
                msgScript.ShowMessage("The entity sees more than before...");
                FOV.localScale = new Vector3(1.3f, 1.3f, 1f);
                break; //larger FOV
            case 4:
                msgScript.ShowMessage("The entity grows faster...");
                agent.speed = 7;
                agent.acceleration = 24;
                break; //speed
            case 5:
                msgScript.ShowMessage("It can hear your every move...");
                canHear = true;
                break; //sound detection
            case 6:
                msgScript.ShowMessage("It never tires...");
                chaseDuration = 16f;
                break; //longer chase timer
            case 7:
                msgScript.ShowMessage("You can't outrun it any longer...");
                agent.speed = 8;
                agent.acceleration = 26;
                agent.angularSpeed = 1000;
                break; //speed increase
            case 8:
                msgScript.ShowMessage("The entity is adapting...");
                enableStairs();
                break; //can use stairs
            case 9:
                msgScript.ShowMessage("The lights grow dim...");
                TorchScript.snuffLights();
                break; //snuff lights
            case 10:
                msgScript.ShowMessage("No place is safe anymore...");
                GameObject.Find("NavObstacleParent").SetActive(false);
                break; //no safe rooms
        }
    }

}
