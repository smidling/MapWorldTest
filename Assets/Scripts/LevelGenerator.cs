using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LevelGenerator : MonoBehaviour
{
    // singleton
    public static LevelGenerator Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    // exposed members
    [SerializeField] private Camera gameCamera;
    [SerializeField] private GameObject groundTilePrefab;
    [SerializeField] private Transform groundParentTr;
    [SerializeField] private AudioSource myaAudioSource;
    [SerializeField] private Transform explosionTr;

    [SerializeField] private const float explosionDuration = 0.2f;


    // Private members - help pointers
    private CameraCtrl myCameraCtrl;

    // Private members
    public const int minLevelSize = 128;
    public const int maxLevelSize = 512;
    
    private MapTileInfo[] terrainTiles;
    private MapTileInfo[] waterTiles;
    private MapTileInfo[] houseTiles;

    private MapTileCtrl[,] mapTiles;

    private const float waterBias = 0.2f; // how many tiles in one will be water


    private Vector2 startPoint;

    private int levelSizeWidth = 512;
    private int levelSizeHeight = 512;
    private int levelHousesNum = 96;

    public int LevelHousesNum
    {
        get { return levelHousesNum; }
    }

    private bool levelInitialised = false;
    


    public void StartTheGame()
    {
        if (!myaAudioSource)
            myaAudioSource = GetComponent<AudioSource>();
        SettingsMenuCtrl.Instance.RefreshSettings();
        terrainTiles = GetMyMapData.Instance.GetTerrainTiles();
        waterTiles = GetMyMapData.Instance.GetWaterTiles();
        houseTiles = GetMyMapData.Instance.GetHouseTiles();
        GenerateLevel(levelSizeWidth, levelSizeHeight);
    }
    
    void GenerateLevel(int levelWidth, int levelHeight)
    {
        levelWidth = Mathf.Clamp(levelWidth, minLevelSize, maxLevelSize);
        levelHeight = Mathf.Clamp(levelHeight, minLevelSize, maxLevelSize);
        mapTiles = new MapTileCtrl[levelWidth,levelHeight];

        // set cam params
        if (!myCameraCtrl && CameraCtrl.Instance)
            myCameraCtrl = CameraCtrl.Instance;
        if (myCameraCtrl)
        {
            startPoint = new Vector2(levelWidth, levelHeight);
            startPoint /= 2;
            myCameraCtrl.transform.position = new Vector3(startPoint.x, startPoint.y, -5);
            myCameraCtrl.SetMapSize(levelWidth,levelHeight);
        }
        Stopwatch watch = new Stopwatch();
        watch.Start();

        // todo can be dependent on camera current size
        int startX = (int)startPoint.x - CameraCtrl.maxCameraTileHeight - CameraCtrl.camLimitPonder;
        int endX = (int)startPoint.x + CameraCtrl.maxCameraTileHeight + CameraCtrl.camLimitPonder;
        int startY = (int)startPoint.y - CameraCtrl.maxCameraTileHeight - CameraCtrl.camLimitPonder;
        int endY = (int)startPoint.y + CameraCtrl.maxCameraTileHeight + CameraCtrl.camLimitPonder;
        
        //create playing field
        CreateMapPart(startX, startY, endX, endY, true);
      
        if (houseTiles.Length <= 0)
            return; // no houses this time

        if (levelHousesNum > levelWidth*levelHeight)
            return; //not enough place for all houses, input was wrong

        //create spots for buildings / houses
        for (int iter = 0; iter < levelHousesNum; iter++)
        {
            int hor = Random.Range(0, levelWidth);
            int vert = Random.Range(0, levelHeight);
            while (mapTiles[hor, vert] != null && mapTiles[hor, vert].type == MapTileCtrl.TileType.House)
            {
                // repeat choice until we find a plot of land with no house on it
                hor = Random.Range(0, levelWidth);
                vert = Random.Range(0, levelHeight);
            }
            int tileNum = 0;
            if (houseTiles.Length > 1)
            {
                // random tile
                tileNum = Random.Range(0, houseTiles.Length);
            }
            if (mapTiles[hor, vert] == null)
            {
                GameObject go = GameObject.Instantiate(groundTilePrefab, new Vector3(hor, vert, 0), new Quaternion(),
                    groundParentTr);
                mapTiles[hor, vert] = go.GetComponent<MapTileCtrl>();
            }
            mapTiles[hor, vert].type = MapTileCtrl.TileType.House;
            mapTiles[hor, vert].name = houseTiles[tileNum].name;
            mapTiles[hor, vert].tileLvl = houseTiles[tileNum].level;
            mapTiles[hor, vert].SetSprite(houseTiles[tileNum].tileSprite);
        }
        levelInitialised = true;

        watch.Stop();
        Debug.Log(watch.Elapsed.TotalSeconds.ToString());
    }

    public void CameraMoved(float currX, float currY)
    {
        if (!levelInitialised)
            return;

        //check if instantiation needed
        // check four corners
        // todo can be dependent on camera current size
        int startX = (int)currX - CameraCtrl.maxCameraTileHeight - CameraCtrl.camLimitPonder;
        startX = startX < 0 ? 0 : startX;
        int endX = (int)currX + CameraCtrl.maxCameraTileHeight + CameraCtrl.camLimitPonder;
        endX = endX >= levelSizeWidth ? levelSizeWidth - 1 : endX;
        int startY = (int)currY - CameraCtrl.maxCameraTileHeight - CameraCtrl.camLimitPonder;
        startY = startY < 0 ? 0 : startY;
        int endY = (int)currY + CameraCtrl.maxCameraTileHeight + CameraCtrl.camLimitPonder;
        endY = endY >= levelSizeHeight ? levelSizeHeight - 1 : endY;
        
        if (mapTiles[startX, startY] != null && mapTiles[endX, startY] != null
            && mapTiles[startX, endY] != null && mapTiles[endX, endY] != null)
            return;

        CreateMapPart(startX,startY,endX,endY,false);
    }

    private void CreateMapPart(int startX, int startY, int endX, int endY, bool isFirstBuild)
    {
        //create playing field
        for (int vert = startY; vert <= endY; vert++)
        {
            for (int hor = startX; hor <= endX; hor++)
            {
                if (mapTiles[hor, vert] != null && !isFirstBuild)
                    continue;
                // todo object pooling
                GameObject go = GameObject.Instantiate(groundTilePrefab, new Vector3(hor, vert, 0), new Quaternion(),
                    groundParentTr);
                mapTiles[hor, vert] = go.GetComponent<MapTileCtrl>();
                mapTiles[hor, vert].x = hor;
                mapTiles[hor, vert].y = vert;

                // tile type settings
                if (waterTiles.Length > 0)
                {
                    // there is water on this map
                    bool isWater = Random.Range(0f, 1f) < waterBias;
                    //todo grouping of water tiles
                    if (isWater)
                    {
                        // this specific tile is water
                        int tileNum = 0;
                        if (waterTiles.Length > 1)
                        {
                            // random tile
                            tileNum = Random.Range(0, waterTiles.Length);
                        }
                        mapTiles[hor, vert].type = MapTileCtrl.TileType.Water;
                        mapTiles[hor, vert].name = waterTiles[tileNum].name;
                        mapTiles[hor, vert].SetSprite(waterTiles[tileNum].tileSprite);
                        continue;
                    }
                }
                // this specific tile is terain
                if (terrainTiles.Length > 0)
                {
                    int tileNum = 0;
                    if (terrainTiles.Length > 1)
                    {
                        // random tile
                        tileNum = Random.Range(0, terrainTiles.Length);
                        // todo grouping of similar tiles by bias
                    }
                    mapTiles[hor, vert].type = MapTileCtrl.TileType.Terrain;
                    mapTiles[hor, vert].name = terrainTiles[tileNum].name;
                    mapTiles[hor, vert].SetSprite(terrainTiles[tileNum].tileSprite);
                }
            }
        }
    }

    public string GetTileName(int tileX, int tileY)
    {
        string result = "Water - Ocean";
        if (levelInitialised && tileX >= 0 && tileY >= 0 && tileX <= levelSizeWidth && tileY <= levelSizeHeight)
        {
            result = mapTiles[tileX, tileY].type + "";
            int lvl = mapTiles[tileX, tileY].tileLvl;
            if (lvl > 0)
                result += " lvl " + lvl;

            result += " - " + mapTiles[tileX, tileY].name;
        }
        return result;
    }

    public bool DestroyBuilding(int tileX, int tileY)
    {
        if (mapTiles[tileX, tileY].type == MapTileCtrl.TileType.House)
        {
            // only house can be destroyed
            if (myaAudioSource)
                myaAudioSource.PlayOneShot(myaAudioSource.clip);

            explosionTr.position = new Vector3(tileX, tileY, 0);
            StartCoroutine(DisableExplosion());

            mapTiles[tileX, tileY].tileLvl--;
            if (mapTiles[tileX, tileY].tileLvl <= 0)
            {
                // no longer a house, build a pasture here
                mapTiles[tileX, tileY].type = MapTileCtrl.TileType.Terrain;
                mapTiles[tileX, tileY].name = terrainTiles[0].name;
                mapTiles[tileX, tileY].SetSprite(terrainTiles[0].tileSprite);
                // todo lower look with level of building
                levelHousesNum--;
            }
            return true;
        }
        return false;
    }

    IEnumerator DisableExplosion()
    {
        yield return new WaitForSeconds(explosionDuration);
        explosionTr.position = new Vector3(-500, -500, 0);
    }

    public void SetLevelParams(int width, int height, int houses)
    {
        levelSizeWidth = width;
        levelSizeHeight = height;
        levelHousesNum = houses;
    }
}
