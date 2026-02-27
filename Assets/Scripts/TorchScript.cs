using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class TorchScript : MonoBehaviour
{
    public Tilemap torchTileMap;
    public List<TileBase> lightTiles;
    public GameObject lightPrefab;
    public static List<GameObject> lightList;
    public static AudioSource extinguishSound;

    void Start() //Instantiate actual lights for each torch/candle tile
    {
        extinguishSound = GetComponent<AudioSource>();
        lightList = new List<GameObject>();
        BoundsInt bounds = torchTileMap.cellBounds;
        for(int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for(int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = torchTileMap.GetTile(tilePos);

                if(lightTiles.Contains(tile))
                {
                    Vector3 worldPos = torchTileMap.CellToWorld(tilePos) + torchTileMap.tileAnchor;
                    var temp = Instantiate(lightPrefab, worldPos, Quaternion.identity, transform);
                    lightList.Add(temp);
                }
            }
        }
    }

    public static void snuffLights()
    {
        for(int i = 0; i < lightList.Count; i++)
        {
            lightList[i].GetComponent<Light2D>().intensity = 0.5f;
        }
        extinguishSound.Play();
    }

}
