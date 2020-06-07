using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    List<Transform> childObjectTransforms;
    Grid mapGrid;
    public int gridX = 15;
    public int gridZ = 12;
    public GameObject centerPrefab;
    public MapSO map;
    public Material grass;
    public Material rock;
    public Material sand;
    bool mapCreated = false;

    private void Awake()
    {
        mapGrid = GetComponent<Grid>();
    }

    void Start()
    {
        InstantiateAllWaypoints();
        GetAllChildTransforms();
        CreateCells();
    }

    
    void Update()
    {
    }

    private void CreateCells()
    {
        int prefabIndex = 0;
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                childObjectTransforms[prefabIndex].localPosition = mapGrid.GetCellCenterLocal((Vector3Int.right * x) + (new Vector3Int(0, 0, 1) * z));
                prefabIndex++;
            }
        }
    }

    public void GetAllChildTransforms()
    {
        childObjectTransforms = new List<Transform>(GetComponentsInChildren<Transform>());
        childObjectTransforms.RemoveAt(0);
    }

    public void InstantiateAllWaypoints()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                GameObject spawned = Instantiate(centerPrefab, transform);
                if(map == null)
                {

                }
                else
                {
                    MeshRenderer renderer = spawned.GetComponent<MeshRenderer>();
                    Tile tile = map.GetTileTypeFromCoords(z * gridX + x);
                    switch (tile)
                    {
                        case Tile.Grass:
                            renderer.material = grass;
                            break;
                        case Tile.Rock:
                            renderer.material = rock;
                            break;
                        case Tile.Sand:
                            renderer.material = sand;
                            break;
                        default:
                            break;
                    }
                }
                spawned.GetComponent<CellManager>().gridPosition.SetGrid(x,z);
            }
        }
    }
}
