using System.Collections.Generic;
using UnityEngine;

public class TilePool : MonoBehaviour
{
    public static TilePool Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    public List<GameObject> pooledTiles;
    public GameObject tilePrefab;
    public int maxTileWidth = 64;

    
    void Start()
    {
        maxTileWidth = 2 * (CameraCtrl.maxCameraTileHeight + CameraCtrl.camLimitPonder + 5);
        pooledTiles = new List<GameObject>();
        for (int i = 0; i < maxTileWidth*maxTileWidth; i++)
        {
            GameObject tempGO = GameObject.Instantiate(tilePrefab, new Vector3(), new Quaternion(), transform);
            tempGO.SetActive(false);
            pooledTiles.Add(tempGO);
        }
    }

    public GameObject GetTile()
    {
        for (int i = 0; i < pooledTiles.Count; i++)
        {
            if (!pooledTiles[i].activeInHierarchy)
                return pooledTiles[i];
        }
        // safety, we must allways return a tile
        GameObject tempGO = GameObject.Instantiate(tilePrefab, new Vector3(), new Quaternion(), transform);
        tempGO.SetActive(false);
        pooledTiles.Add(tempGO);
        return pooledTiles[pooledTiles.Count - 1];
    }
    
}
