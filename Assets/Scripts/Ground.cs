using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Ground : MonoBehaviour
{
    void Start()
    {
        NavMeshLink[] allLinks = GameObject.Find("Tilemap_ground").GetComponentsInChildren<NavMeshLink>(); //disable enemy shortcuts until key threshold
        foreach (NavMeshLink link in allLinks)
            link.enabled = false;
    }
}
