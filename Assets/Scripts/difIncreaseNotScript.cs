using TMPro;
using System.Collections;
using UnityEngine;

public class difIncreaseNotScript : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float typeSpeed = 0.05f;  //seconds between each character
    public float displayTime = 2f;

    private Coroutine messageRoutine;

    public void ShowMessage(string message)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(TypeMessage(message));
    }

    private IEnumerator TypeMessage(string message) //Coroutine for typewriter effect
    {
        messageText.text = "";
        messageText.alpha = 1f;

        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(displayTime);
        messageText.text = "";
    }
}
