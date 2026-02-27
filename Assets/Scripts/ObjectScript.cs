using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectScript : MonoBehaviour //Creates actual gameobjects for each breakable object tile
{
    public Tilemap objectTileMap;
    public List<TileBase> objectTiles;
    public List<Sprite> objSprites;
    public static List<GameObject> objList;
    public GameObject breakablePrefab;

    public AudioClip boxClip;
    public AudioClip vaseClip;

    void Start()
    {
        objList = new List<GameObject>();

        BoundsInt bounds = objectTileMap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = objectTileMap.GetTile(tilePos);

                if (objectTiles.Contains(tile))
                {
                    Vector3 worldPos = objectTileMap.CellToWorld(tilePos) + objectTileMap.tileAnchor;
                    GameObject temp = Instantiate(breakablePrefab, worldPos, Quaternion.identity);
                    int randomIndex = Random.Range(0, objSprites.Count);
                    AudioSource audio = temp.GetComponent<AudioSource>();
                    temp.GetComponent<SpriteRenderer>().sprite = objSprites[randomIndex];
                    objList.Add(temp);
                    objectTileMap.SetTile(tilePos, null); // Remove tile from tilemap
                    if (randomIndex <= 3) //is box object
                    {
                        audio.clip = boxClip;
                    }
                    else //is vase object
                    {
                        audio.clip = vaseClip;
                    }
                }
            }
        }

        List<GameObject> keyObjList = pickKeyPositions();
        for(int i=0;i<keyObjList.Count;i++) //Set flags for each object containing key/powerup
        {
            if (i < 10)
            {
                keyObjList[i].GetComponent<breakableObject>().hasKey = true;
                keyObjList[i].GetComponent<breakableObject>().keyIndex = i;
            }
            else
            {
                keyObjList[i].GetComponent<breakableObject>().hasPowerUp = true;
            }
        }

    }
    List<GameObject> pickKeyPositions()
    {
        return objList.OrderBy(x => Random.value).Take(18).ToList(); //First 10 = keys, >10 = powerups
    }
}
