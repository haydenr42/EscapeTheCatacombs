using TMPro;
using UnityEngine;

public class ExitDoorScript : MonoBehaviour
{
    private bool playerInRange;
    private SpriteRenderer doorSprite;
    private AudioSource lockSound;
    private AudioSource openSound;
    public GameObject keyPrefab;
    private GameObject tempKey;
    public GameObject winScreen;
    private AudioSource[] doorSounds = new AudioSource[2];
    private AudioSource doorLocked;
    private AudioSource doorOpened;
    void Start()
    {
        doorSounds = GetComponents<AudioSource>();
        doorLocked = doorSounds[0];
        doorOpened = doorSounds[1];
        doorSprite = GetComponent<SpriteRenderer>();
    }

    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if(playerScript.keysCollected == 10)
            {
                doorOpened.Play();
                Escape();
                Debug.Log("Escaped!");
            }
            else
            {
                doorLocked.Play();
                Debug.Log("Need more keys!");
            }
            
        }
    }

    private void Escape()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            doorSprite.color = Color.white;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            doorSprite.color = Color.lightGray;
        }
    }
}
