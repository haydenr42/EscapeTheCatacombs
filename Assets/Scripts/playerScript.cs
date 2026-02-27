using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PowerUp;

public class playerScript : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    private Vector2 moveInput;

    public bool startedMoving = false;

    public int playerDirection;

    public GameObject faceD;
    public GameObject faceU;
    public GameObject faceL;
    public GameObject faceR;

    public Animator playerAnim;
    private AudioSource footstepAudio;

    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public Image staminaBar;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float runCost = 20f;
    public float rechargeRate = 15f;

    private Coroutine recharge;

    public GameObject temp;

    public static int keysCollected;

    public GameObject entity;
    public static PowerUpType? currentPowerUp = null;
    public static PowerUpType? nearPowerUp = null;
    private Sprite powerUpSprite;
    private GameObject powerUpObject;
    private bool playerInRange = false;
    public Image powerUpContainer;
    public TextMeshProUGUI countdown;
    private float powerUpDuration = 15f;
    private float powerUpTimer = 15f;
    public bool powerUpActive = false;
    private bool staminUpActive = false;
    private AudioSource powerUpAudioPlayer;
    public AudioClip powerUpSound;
    public SpriteRenderer[] faceSprites = new SpriteRenderer[4];
    public bool freezeActive = false;


    void Start()
    {
        keysCollected = 0;
        footstepAudio = GetComponent<AudioSource>();
        GameObject audioHost = new GameObject("PowerUpUse");
        powerUpAudioPlayer = audioHost.AddComponent<AudioSource>();
        powerUpAudioPlayer.clip = powerUpSound;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if(Input.GetKeyDown(KeyCode.K) && Input.GetKey(KeyCode.RightShift)) //temporary dev cheat, avert your eyes
        {
            keysCollected += 1;
            GameObject.Find("keyCount").GetComponent<TextMeshProUGUI>().text = keysCollected.ToString() + "/10";
            temp.GetComponent<EntityScript>().IncreaseDifficulty();
        }

        if (stamina > 0 && Input.GetKey(KeyCode.LeftShift) && moveInput.magnitude > 0.01f) //Sprint & stamina bar drain logic
        {
            moveSpeed = runSpeed;
            footstepAudio.pitch = 1.6f;
            footstepAudio.volume = 1;

            if (!staminUpActive)
                stamina -= runCost * Time.deltaTime;

            if (stamina < 0) 
                stamina = 0;

            staminaBar.fillAmount = stamina / maxStamina;

            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
        else
        {
            moveSpeed = walkSpeed;
            footstepAudio.pitch = 1.2f;
            footstepAudio.volume = 0.8f;
        }

        rb.linearVelocity = moveInput * moveSpeed; //actual movement

        if (moveInput.x > 0) //right
        {
            faceR.SetActive(true);
            faceL.SetActive(false);
            faceD.SetActive(false);
            faceU.SetActive(false);
            playerAnim.Play("playerWalkR");
        }
        else if(moveInput.x < 0) //left
        {
            faceR.SetActive(false);
            faceL.SetActive(true);
            faceD.SetActive(false);
            faceU.SetActive(false);
            playerAnim.Play("playerWalkL");
        }
        else if (moveInput.y > 0) //up
        {
            faceR.SetActive(false);
            faceL.SetActive(false);
            faceD.SetActive(false);
            faceU.SetActive(true);
            playerAnim.Play("playerWalkU");
        }
        else if (moveInput.y < 0) //down
        {
            faceR.SetActive(false);
            faceL.SetActive(false);
            faceD.SetActive(true);
            faceU.SetActive(false);
            playerAnim.Play("playerWalkD"); 
        }

        if (moveInput.y == 0 && moveInput.x == 0) //idle state
        {
            playerAnim.Play("playerIdle");
            footstepAudio.Stop();
            rechargeRate = 30f;
        }
        else if (!footstepAudio.isPlaying)
        {
            footstepAudio.Play();
            rechargeRate = 15f;
        }

        if (startedMoving || transform.position.x != 0 || transform.position.y != 0) //disable initial sprite once started
        {
            sr.enabled = false;
            startedMoving = true;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E)) //pick up power-up
        {
            if (currentPowerUp != null)
            {
                UsePowerUp(currentPowerUp.Value);
            }
            currentPowerUp = nearPowerUp;
            powerUpContainer.sprite = powerUpSprite;
            Color temp = powerUpContainer.color;
            temp.a = 1f;
            powerUpContainer.color = temp;
            Destroy(powerUpObject);
        }

        if(!powerUpActive && currentPowerUp != null && Input.GetKeyDown(KeyCode.Q))
        {
            UsePowerUp(currentPowerUp.Value);
            currentPowerUp = null;
            powerUpContainer.GetComponent<Image>().sprite = null;
        }

    }

    public void UsePowerUp(PowerUp.PowerUpType type)
    {
        powerUpAudioPlayer.Play();
        switch (type)
        {
            case PowerUpType.Freeze:
                StartCoroutine(FreezeEntity());
                StartCoroutine(InitiateCountdown());
                break;
            case PowerUpType.Invis:
                StartCoroutine(ActivateInvisibility());
                StartCoroutine(InitiateCountdown());
                break;
            case PowerUpType.StaminUp:
                StartCoroutine(ActivateStaminUp());
                StartCoroutine(InitiateCountdown());
                break;
            case PowerUpType.Warp:
                int randomIndex = Random.Range(0, 8);
                transform.position = entity.GetComponent<EntityScript>().patrolPoints.GetChild(randomIndex).transform.position;
                break;
        }
        Color temp = powerUpContainer.color;
        temp.a = 0f;
        powerUpContainer.color = temp;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerUp"))
        {
            playerInRange = true;
            nearPowerUp = collision.GetComponent<PowerUp>().type;
            powerUpSprite = collision.GetComponent<SpriteRenderer>().sprite;
            powerUpObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerUp"))
        {
            playerInRange = false;
            nearPowerUp = null;
            powerUpSprite = null;
            powerUpObject = null;
        }
    }

    //*********Coroutines***********

    private IEnumerator RechargeStamina() //coroutine to recharge stamina after not waiting for 2 seconds
    {
        yield return new WaitForSeconds(2f);

        while (stamina < maxStamina)
        {
            stamina += rechargeRate / 10f;
            if (stamina > maxStamina) stamina = maxStamina;
            staminaBar.fillAmount = stamina / maxStamina;
            yield return new WaitForSeconds(.1f);
        }
    }

    private IEnumerator FreezeEntity()
    {
        EntityScript entityScript = entity.GetComponent<EntityScript>();
        entityScript.agent.isStopped = true;
        freezeActive = true;
        yield return new WaitForSeconds(powerUpDuration);

        entityScript.agent.isStopped = false;
        freezeActive = false;
    }

    private IEnumerator ActivateInvisibility()
    {
        Color transparent = new Color();
        transparent.a = 0.4f;
        FOVScript.invisActive = true;
        foreach (var f in faceSprites) f.color = transparent;
        yield return new WaitForSeconds(powerUpDuration);

        FOVScript.invisActive = false;
        transparent = Color.white;
        transparent.a = 1f;
        foreach (var f in faceSprites) f.color = transparent;
    }

    private IEnumerator ActivateStaminUp()
    {
        staminUpActive = true;
        stamina = maxStamina;
        staminaBar.fillAmount = stamina / maxStamina;
        yield return new WaitForSeconds(powerUpDuration);

        staminUpActive = false;
    }

    private IEnumerator InitiateCountdown()
    {
        powerUpTimer = powerUpDuration;
        powerUpActive = true;
        while(powerUpTimer >= 0)
        {
            powerUpTimer -= Time.deltaTime;
            countdown.text = Mathf.Ceil(powerUpTimer).ToString();
            yield return null;
        }
        countdown.text = "";
        powerUpActive = false;

    }
}
