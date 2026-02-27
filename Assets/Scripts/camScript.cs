using UnityEngine;

public class camScript : MonoBehaviour
{
    public GameObject player;
    
    void Update() //follow player
    {
        transform.position = player.transform.position + new Vector3(0, 0, -10);
    }
}
