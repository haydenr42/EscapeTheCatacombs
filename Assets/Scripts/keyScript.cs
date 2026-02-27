using TMPro;
using UnityEngine;

public class keyScript : MonoBehaviour
{
    private bool playerInRange;
    private TextMeshProUGUI keyCountUI;
    private AudioSource keyPickUp;
    private GameObject entity;
    private EntityScript entityScript;
    void Start()
    {
        entity = GameObject.FindWithTag("Entity");
        entityScript = entity.GetComponent<EntityScript>();
        keyCountUI = GameObject.Find("keyCount").GetComponent<TextMeshProUGUI>();
        keyPickUp = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            pickUpKey();
        }
    }

    void pickUpKey()
    {
        playerScript.keysCollected += 1;
        GameObject audioHost = new GameObject("KeyPickUp");
        audioHost.transform.position = transform.position;

        AudioSource audioSourceClone = audioHost.AddComponent<AudioSource>();
        audioSourceClone.clip = keyPickUp.clip;
        audioSourceClone.Play();
        Destroy(audioHost, keyPickUp.clip.length);

        Destroy(gameObject);

        keyCountUI.text = playerScript.keysCollected.ToString() + "/10";
        entityScript.IncreaseDifficulty();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
