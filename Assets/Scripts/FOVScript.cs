using UnityEngine;

public class FOVScript : MonoBehaviour
{
    private float angle;
    public LayerMask visLayer;
    public Transform entity;
    public Transform player;
    public static bool invisActive = false;
    void Start()
    {
        
    }

    void Update() //rotate FOV cone around pivot point based on entity's current direction
    {
        if (EntityScript.facingDirection == Vector2.left)
        {
            angle = 0f;
        }
        else if (EntityScript.facingDirection == Vector2.down)
        {
            angle = 90f;
        }
        else if (EntityScript.facingDirection == Vector2.right)
        {
            angle = 180f;
        }
        else
            angle = 270f;

        transform.rotation = Quaternion.Euler(0,0,angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!invisActive && collision.CompareTag("Player")) //do raycast from enemy to see if they can actually "see" player
        {
            Vector2 directionToPlayer = (player.position - entity.position).normalized;
            float distanceToPlayer = Vector2.Distance(entity.position, player.position);

            RaycastHit2D hit = Physics2D.Raycast(entity.position, directionToPlayer, distanceToPlayer, visLayer);
            Debug.DrawRay(entity.position, directionToPlayer * distanceToPlayer, Color.red);

            Debug.Log(hit.collider);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                EntityScript.playerSpotted = true;
            }
            else
            {
                EntityScript.playerSpotted = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!invisActive && collision.CompareTag("Player"))
        {
            Vector2 directionToPlayer = (player.position - entity.position).normalized;
            float distanceToPlayer = Vector2.Distance(entity.position, player.position);

            RaycastHit2D hit = Physics2D.Raycast(entity.position, directionToPlayer, distanceToPlayer, visLayer);
            Debug.DrawRay(entity.position, directionToPlayer * distanceToPlayer, Color.red);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                EntityScript.playerSpotted = true;
            }
            else
            {
                EntityScript.playerSpotted = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EntityScript.playerSpotted = false;
        }
    }
}
