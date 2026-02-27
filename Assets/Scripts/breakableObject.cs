using UnityEngine;
public class breakableObject : MonoBehaviour
{
    private AudioSource aud;
    private SpriteRenderer sr;

    private bool playerInRange = false;
    private float holdTimer = 0f;
    public float breakTime = 0.3f;
    public GameObject breakEffect;

    public bool hasKey = false;
    public bool hasPowerUp;
    public GameObject powerUpPrefab;
    public int keyIndex = 0;
    public Sprite[] keySprites = new Sprite[10];
    public GameObject keyPrefab;

    public GameObject entity;
    private EntityScript entityScript;


    void Start()
    {
        entity = GameObject.FindWithTag("Entity");
        entityScript = entity.GetComponent<EntityScript>();
        aud = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            if(holdTimer >= breakTime)
            {
                Break();
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    void Break()
    {
        GameObject audioHost = new GameObject("BreakSound");
        audioHost.transform.position = transform.position;

        AudioSource audioSourceClone = audioHost.AddComponent<AudioSource>();
        audioSourceClone.clip = aud.clip;
        audioSourceClone.Play();
        Destroy(audioHost, aud.clip.length);

        if(EntityScript.canHear)
        {
            entityScript.ListenForSound(transform.position);
        }

        Instantiate(breakEffect, transform.position, Quaternion.identity);

        if(hasKey)
        {
            GameObject keyInstance = Instantiate(keyPrefab, transform.position, Quaternion.identity);
            keyInstance.GetComponent<SpriteRenderer>().sprite = keySprites[keyIndex];
        }

        ObjectScript.objList.Remove(gameObject);
        Destroy(gameObject);

        if(hasPowerUp)
        {
            GameObject powerUpInstance = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            sr.color = Color.orange;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            holdTimer = 0f;
            sr.color = Color.white;
        }
    }
}
