using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject compass;
    public GameObject howToPlayScreen;
    public List<AudioClip> ambienceList;
    private AudioSource ambiencePlayer;
    public static bool newGame = true;
    public Image powerUpContainer;

   
    void Start()
    {
        Color temp = powerUpContainer.color;
        temp.a = 0f;
        powerUpContainer.color = temp;
        ambiencePlayer = GetComponent<AudioSource>();
        howToPlayScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!compass.activeSelf && playerScript.keysCollected > 5)
            compass.SetActive(true);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartGame()
    {
        newGame = false;
        howToPlayScreen.SetActive(false);
        StartCoroutine(PlayAmbienceLoop());
        Time.timeScale = 1f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator PlayAmbienceLoop()
    {
        int index = 0;

        while (true)
        {
            ambiencePlayer.clip = ambienceList[index];
            ambiencePlayer.Play();

            // Wait for the current clip to finish
            yield return new WaitForSeconds(ambienceList[index].length);

            // Alternate to next clip
            index = (index + 1) % ambienceList.Count;
        }
    }
}
