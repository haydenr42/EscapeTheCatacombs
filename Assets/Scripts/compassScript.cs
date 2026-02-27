using UnityEngine;
using System.Collections.Generic;

public class compassScript : MonoBehaviour
{
    public GameObject player;
    
    void Start()
    {
        gameObject.SetActive(false); //Inactive until 8 keys are collected, handled in cam script
    }

    void Update() //point to nearest object
    {
        Transform nearestTarget = findNearest();
        Vector3 direction = nearestTarget.position - player.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 45f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Transform findNearest() //find nearest breakable object
    {
        Transform nearest = null;
        float minDistance = float.PositiveInfinity;
        foreach (GameObject obj in ObjectScript.objList)
        {
            float dist = Vector3.Distance(obj.transform.position, player.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = obj.transform;
            }
        }

        return nearest;
    }
}
