using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Gen : MonoBehaviour
{
    [System.Serializable]
    public struct TerrainType // Terrain struct
    {
        public string name;
        public float height;
        public GameObject tile;
    }

    // Resource rarity - [[Later should be changed to a struct due to many resources]]
    public GameObject Abyss;
    public int abyss;
    public GameObject Ruins;
    public int ruins;

    // Prefabs
    public GameObject townHall;
    public GameObject fog;

    // Map settings
    public int mapSize;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public Vector2 offset;

    // Control the falloff map.
    public int falloffA;
    public float falloffB;

    public TerrainType[] regions;

    // Extra settings
    public bool autoUpdate;
    public bool useFalloff;
    public bool FogMap;

    float[,] falloffMap;
    GameObject[,] TileArray;

    void Awake()
    {
        falloffMap = Map_FalloffGen.generateFalloffMap(mapSize, falloffA, falloffB);
    }

    //Generating the map.
    public GameObject generateMap()
    {
        int seed = Random.Range(int.MinValue, int.MaxValue);//Seed get a random value.
        int randP1, randP2;
        TileArray = new GameObject[mapSize, mapSize];

        //Create a noise map.
        float[,] noiseMap = Map_Noise.noiseMapGen(mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        //Create a dictionary of possible possitions for the Town Hall.
        Dictionary<int, Vector2Int> PosiblePos = new Dictionary<int, Vector2Int>();
        int count = 0;

        //Delete the map if such exist.
        deleteTileMap();

        for (int y = 0; y < mapSize; y++)
        {
            for( int x = 0; x < mapSize; x++)
            {
                if (useFalloff)
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);

                float currentHeight = noiseMap[x, y];

                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height) 
                    {
                        if (CheckValidPos(regions[i]))
                            PosiblePos.Add(count++, new Vector2Int(x, y));

                        randP1 = Random.Range(1, abyss);
                        randP2 = Random.Range(1, ruins);

                        if (regions[i].name == "P1" && randP1 == 1)
                            TileArray[x, y] = Instantiate(Abyss, new Vector3(10 * x, 1, 10 * y), Quaternion.Euler(0, 180, 0));
                        else if (regions[i].name == "P2" && randP2 == 1)
                            TileArray[x, y] = Instantiate(Ruins, new Vector3(10 * x, 1, 10 * y), Quaternion.Euler(0, 180, 0));
                        else
                            TileArray[x, y] = Instantiate(regions[i].tile, new Vector3(10 * x, 1, 10 * y), Quaternion.Euler(0, 180, 0));

                        if (FogMap)
                        {
                            GameObject thisFog = Instantiate(fog, new Vector3(0, 1, 0), Quaternion.Euler(0, 180, 0), TileArray[x, y].transform);
                            thisFog.transform.localPosition = new Vector3(0, 1, 0);
                        }
                   
                        TileArray[x, y].transform.parent = this.transform;
                        TileArray[x, y].name = string.Format("tile_x{0}_y{1}", x, y);
                        break;
                    }
                }
            }
        }
        return PlaceStartPos(PosiblePos);
    }

    //Check if from this region there is access to the over tiles.
    public bool CheckValidPos(TerrainType tile)
    {
        if (tile.height > 0.60 && tile.height < 0.69)
            return true;
        return false;
    }

    //Choosing randomly the player's starting point and adding the Town Hall to the map.
    public GameObject PlaceStartPos(Dictionary<int, Vector2Int> PosiblePos)
    {
        int size = PosiblePos.Count, k = 0;
        int randNum = Random.Range(0, size - 1);

        int x = PosiblePos[randNum].x;
        int y = PosiblePos[randNum].y;

        while (regions[k].tile.name != "Plains") { k++; }
        List<Vector3> PeasentPos = new List<Vector3>();
        for (int i = y - 1; i <= y + 1; i++)
        {
            for (int j = x - 1; j <= x + 1; j++)
            {
                //Creating an list of the tiles surrounding the town hall.
                if (!(i == y && j == x))
                    PeasentPos.Add(new Vector3(j * 10, 1, i * 10));

                TileArray[j, i] = GameObject.Find(string.Format("tile_x{0}_y{1}", j, i));
                if (TileArray[j, i].name != "Plains")
                { 
                    Vector3 pos = TileArray[x, y].transform.position;
                    GameObject.DestroyImmediate(TileArray[j, i]);
                    TileArray[j, i] = Instantiate(regions[k].tile, new Vector3(j * 10, 1, i * 10), Quaternion.Euler(0, 180, 0));
                    TileArray[j, i].transform.parent = this.transform;
                    TileArray[j, i].name = string.Format("tile_x{0}_y{1}", j, i);

                    if (FogMap)//Create fog if it enabled.
                    {
                        GameObject thisFog = Instantiate(fog, new Vector3(0, 1, 0), Quaternion.Euler(0, 180, 0), TileArray[j, i].transform);
                        thisFog.transform.localPosition = new Vector3(0, 1, 0);
                    }
                }
            }
        }
        Player_SpawnBuilding TownHall = FindObjectOfType<Player_SpawnBuilding>();
        TownHall.Spawn(townHall, TileArray[x, y]);
        RandomPeasents(PeasentPos);
        return TileArray[x, y];
    }


    //Create 3 random peasents at random places around the town hall.
    private void RandomPeasents(List<Vector3> PeasentsPos) 
    {
       Unit_List unitList = FindObjectOfType<Unit_List>();

        int size = 8;
        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(0, size);
            unitList.AddUnit(PeasentsPos[rand], 0);
            PeasentsPos.RemoveAt(rand);
            size--;
        }
    }


    //Delete all the tiles on the map.
    public void deleteTileMap()
    {
        while (this.transform.childCount != 0)
        {
            foreach (Transform child in this.transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
    }

    //Limits editing to valid values.
    private void OnValidate()
    {
        if (mapSize < 1)
            mapSize = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
        if (falloffA < 1)
            falloffA = 1;
        if (falloffB < 0.1f)
            falloffB = 0.1f;
        if (abyss < 1)
            abyss = 1;
        if (ruins < 1)
            ruins = 1;

        falloffMap = Map_FalloffGen.generateFalloffMap(mapSize, falloffA, falloffB);
    }
}