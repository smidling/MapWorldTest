using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

    [Serializable]
    public class MapTileInfo
    {
        public string type = "";
        public string name = "";
        public int level = -1;
        public Sprite tileSprite;
    }

public class GetMyMapData : MonoBehaviour
{

    // singleton
    public static GetMyMapData Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    private string mapWidth;
    private string mapHeight;
    private string housesNum;
    private MapTileInfo[] mapTiles;
    public Sprite defaultTileSprite;

    public int MapWidth
    {
        get { return Convert.ToInt32(mapWidth); }
    }

    public int MapHeight
    {
        get { return Convert.ToInt32( mapHeight); }
    }

    public int HousesNum
    {
        get { return Convert.ToInt32(housesNum); }
    }

    public MapTileInfo[] MapTiles
    {
        get { return mapTiles; }
    }


    void Start()
    {
        StartCoroutine(GetLocationInfo());
    }
    
    private IEnumerator GetLocationInfo()
    {
        WWW mapInfoRequest = 
            new WWW("https://gist.githubusercontent.com/anonymous/63de7fecde7289804f95619c9d20c7ad/raw/a6e24694cfbfef42fbb018283b5e570173a2e816/map.json"); //get our location info
        yield return mapInfoRequest;

        if (mapInfoRequest.error == null || mapInfoRequest.error == "")
        {
            var jsonNode = JSON.Parse(mapInfoRequest.text);
            mapWidth = jsonNode["map_width"].Value;
            mapHeight = jsonNode["map_height"].Value;
            housesNum = jsonNode["number_of_houses"].Value;

            int thisMapTiles = jsonNode["tiles"].Count;
            mapTiles = new MapTileInfo[thisMapTiles];
            for (int iter = 0; iter < thisMapTiles; iter++)
            {
                mapTiles[iter ] = new MapTileInfo();
                mapTiles[iter].type = jsonNode["tiles"][iter]["type"].Value;
                mapTiles[iter].name = jsonNode["tiles"][iter]["name"].Value;
                try
                {
                    mapTiles[iter].level = Convert.ToInt32(jsonNode["tiles"][iter]["level"].Value);
                }
                catch
                {
                    // its ok, it doesnt have level
                }
            }
            PopulateTileCollectionSprites();
        }
        else
        {
            Debug.Log("WWW error: " + mapInfoRequest.error);
        }
    }

    private void PopulateTileCollectionSprites()
    {
        for (int iter = 0; iter < mapTiles.Length; iter++)
        {
            try
            {
                string spriteName = mapTiles[iter].type;
                mapTiles[iter].tileSprite = Resources.Load<Sprite>(spriteName);
            }
            catch
            {
                // no tile sprite with that name exists, leave default
                mapTiles[iter].tileSprite = defaultTileSprite;
            }
        }
    }

    public MapTileInfo[] GetTerrainTiles()
    {
        List<MapTileInfo> result = new List<MapTileInfo>();
        for (int iter = 0; iter < mapTiles.Length; iter++)
        {
            if(mapTiles[iter].name == "Empty Tile")
                result.Add(mapTiles[iter]);
        }
        return result.ToArray();
    }
    public MapTileInfo[] GetWaterTiles()
    {
        List<MapTileInfo> result = new List<MapTileInfo>();
        for (int iter = 0; iter < mapTiles.Length; iter++)
        {
            if (mapTiles[iter].type == "water")
                result.Add(mapTiles[iter]);
        }
        return result.ToArray();
    }
    public MapTileInfo[] GetHouseTiles()
    {
        List<MapTileInfo> result = new List<MapTileInfo>();
        for (int iter = 0; iter < mapTiles.Length; iter++)
        {
            if (mapTiles[iter].type.Contains("house"))
                result.Add(mapTiles[iter]);
        }
        return result.ToArray();
    }
}
