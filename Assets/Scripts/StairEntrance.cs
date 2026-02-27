using System.Collections.Generic;
using UnityEngine;

public class StairEntrance : MonoBehaviour
{
    private bool inRadius;
    private AudioSource aud;
    public List<Transform> stairList;
    public GameObject player;
    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inRadius && Input.GetKeyDown(KeyCode.R))
        {
            aud.Play();
            int randomIndex = Random.Range(0, stairList.Count);
            player.transform.position = stairList[randomIndex].transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            inRadius = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRadius = false;
        }
    }

}
