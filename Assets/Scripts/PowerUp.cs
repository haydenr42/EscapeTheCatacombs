using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Freeze, StaminUp, Invis, Warp };
    public PowerUpType type;
    private SpriteRenderer sr;
    public List<Sprite> powerUpSprites;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        AssignSprite();
    }

    void AssignSprite()
    {
        int typeCount = System.Enum.GetValues(typeof(PowerUpType)).Length;
        type = (PowerUpType)Random.Range(0, typeCount);
        switch (type)
        {
            case PowerUpType.Freeze:
                sr.sprite = powerUpSprites[0]; break;
            case PowerUpType.Invis:
                sr.sprite = powerUpSprites[1]; break;
            case PowerUpType.StaminUp:
                sr.sprite = powerUpSprites[2]; break;
            case PowerUpType.Warp:
                sr.sprite = powerUpSprites[3]; break;
        }

    }
}
