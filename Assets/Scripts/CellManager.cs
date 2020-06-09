using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using JetBrains.Annotations;
using System.Runtime.InteropServices;

public class CellManager : MonoBehaviour
{
    GameObject castle; // Amed wrote
    private static GameObject indicator;
    private NetworkManager networkManager;
    private MeshCollider meshCollider;
    private Vector3Int cellPosition;
    private TileManager tileManager;
    private Bounds cellBound;
    public EdgeCenterBounds edgeCenterBounds;
    private GameObject wall;
    private Vector3 indicatorPos;
    private int indicatorPosIndex = -1;
    public EdgeAvailability edgeAvailability;
    public GridPosition gridPosition;
    public EdgeObjectPlace[] edges;
    public static List<CellManager> allCells;

    private void Awake()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        wall = networkManager.spawnPrefabs[0];
        castle = networkManager.spawnPrefabs[1]; // Amed wrote
        meshCollider = GetComponent<MeshCollider>();
        tileManager = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>();
        Grid grid = tileManager.gameObject.GetComponent<Grid>();
        cellPosition = grid.LocalToCell(transform.localPosition);
        cellBound = grid.GetBoundsLocal(cellPosition);
        edgeCenterBounds = new EdgeCenterBounds(cellBound);
        indicator = GameObject.FindGameObjectWithTag("Indicator");
        if(allCells != null)
        {
            allCells.Add(this);
        }
        else
        {
            allCells = new List<CellManager>();
            allCells.Add(this);
        }
    }

    void Start()
    {
        /*
        GridPosition g1 = new GridPosition();
        g1.gridX = 1;
        g1.gridZ = 1;

        GridPosition g2 = new GridPosition();
        g2.gridX = 1;
        g2.gridZ = 1;

        if(g1 == g2)
        {
            Debug.Log(true);
        }
        */
    }

    void Update()
    {
        
    }

    public static CellManager CellFromGridPosition(GridPosition pos)
    {
        for (int i = 0; i < allCells.Count; i++)
        {
            if (allCells[i].gridPosition == pos)
            {
                return allCells[i];
            }
        }
        return null;
    }

    public static CellManager CellFromGridPosition(int gridX,int gridZ)
    {
        GridPosition pos = new GridPosition();
        pos.gridX = gridX;
        pos.gridZ = gridZ;
        for (int i = 0; i < allCells.Count; i++)
        {
            if (allCells[i].gridPosition == pos)
            {
                return allCells[i];
            }
        }
        return null;
    }

    public Tuple<Vector3,Quaternion> PlaceCastle() // Amed wrote
    {
        Vector3 castlePos;
        castlePos = indicatorPos;
        if (indicatorPos.x%10!=0)
        {
            castlePos.x = indicatorPos.x + 5;
        }
        if (indicatorPos.z%10!=0)
        {
            castlePos.z = indicatorPos.z + 5;
        }

        Tuple<Vector3, Quaternion> tuple = new Tuple< Vector3, Quaternion>(castlePos, Quaternion.identity);

        return tuple;
    }



    public void FindNearestCenterEdge(Vector3 point)
    {
        point = transform.InverseTransformPoint(point);
        float nearestEdgeDistance = (point - edgeCenterBounds.edges[0]).magnitude;
        Tuple<float, int> distanceTuple = new Tuple<float, int>(nearestEdgeDistance, 0);
        for (int i = 1; i < edgeCenterBounds.edges.Length; i++)
        {
            if(nearestEdgeDistance > (point - edgeCenterBounds.edges[i]).magnitude)
            {
                distanceTuple = new Tuple<float, int>(0f, i);
                nearestEdgeDistance = (point - edgeCenterBounds.edges[i]).magnitude;
            }
        }
        indicator.transform.position = transform.TransformPoint(edgeCenterBounds.edges[distanceTuple.Item2]);
        indicatorPos = indicator.transform.position;
        indicatorPosIndex = distanceTuple.Item2;
    }

    public int FindNearestCenterEdgeForCastle(Vector3 point)
    {
        point = transform.InverseTransformPoint(point);
        float nearestEdgeDistance = (point - edgeCenterBounds.edges[0]).magnitude;
        Tuple<float, int> distanceTuple = new Tuple<float, int>(nearestEdgeDistance, 0);
        for (int i = 1; i < edgeCenterBounds.edges.Length; i++)
        {
            if (nearestEdgeDistance > (point - edgeCenterBounds.edges[i]).magnitude)
            {
                distanceTuple = new Tuple<float, int>(0f, i);
                nearestEdgeDistance = (point - edgeCenterBounds.edges[i]).magnitude;
            }
        }
        //indicator.transform.position = transform.TransformPoint(edgeCenterBounds.edges[distanceTuple.Item2]);
        //indicatorPos = indicator.transform.position;
        return distanceTuple.Item2;
    }

    public GameObject PlaceWall()
    {
        if (indicatorPosIndex == 0 || indicatorPosIndex == 1)
        {
            GameObject spawnedWall = Instantiate(wall, indicatorPos, Quaternion.Euler(0, 0, 0));
            ActionPointsAndDiceUI.decreaseActionPoints.Invoke();
            return spawnedWall;
        }
        else if (indicatorPosIndex == 2 || indicatorPosIndex == 3)
        {
            GameObject spawnedWall = Instantiate(wall, indicatorPos, Quaternion.Euler(0, 90, 0));
            ActionPointsAndDiceUI.decreaseActionPoints.Invoke();
            return spawnedWall;
        }
        else
        {
            return null;
        }

    }

    public Tuple<Vector3,Quaternion,int,GridPosition> PlaceWallInfo()
    {
        if (indicatorPosIndex == 0 || indicatorPosIndex == 1)
        {
            Tuple<Vector3, Quaternion,int,GridPosition> tuple = new Tuple<Vector3, Quaternion,int,GridPosition>(indicatorPos, Quaternion.Euler(0, 0, 0),indicatorPosIndex,gridPosition);
            ActionPointsAndDiceUI.decreaseActionPoints.Invoke();
            return tuple;
        }
        else if (indicatorPosIndex == 2 || indicatorPosIndex == 3)
        {
            Tuple<Vector3, Quaternion,int,GridPosition> tuple = new Tuple<Vector3, Quaternion,int,GridPosition>(indicatorPos, Quaternion.Euler(0, 90, 0),indicatorPosIndex,gridPosition);
            ActionPointsAndDiceUI.decreaseActionPoints.Invoke();
            return tuple;
        }
        else
        {
            return null;
        }
    }

    public static void ResetIndicatorPos()
    {
        indicator.transform.position = new Vector3(0, 1000, 0);
    }

    public bool IsEdgePlaceable(int edgeIndex,PlayerTeam team,GridPosition gridPos)
    {
        CellManager cellManager = CellFromGridPosition(gridPos);
        if(cellManager.edges[edgeIndex].gameObject != null) 
        {
            return false;
        }

        CellManager up = CellFromGridPosition(gridPos.gridX, gridPos.gridZ + 1);
        CellManager down = CellFromGridPosition(gridPos.gridX, gridPos.gridZ - 1);
        CellManager left = CellFromGridPosition(gridPos.gridX - 1, gridPos.gridZ);
        CellManager right = CellFromGridPosition(gridPos.gridX + 1, gridPos.gridZ);
        CellManager upLeft = CellFromGridPosition(gridPos.gridX - 1, gridPos.gridZ + 1);
        CellManager upRight = CellFromGridPosition(gridPos.gridX + 1, gridPos.gridZ + 1);
        CellManager downLeft = CellFromGridPosition(gridPos.gridX - 1, gridPos.gridZ - 1);
        CellManager downRight = CellFromGridPosition(gridPos.gridX + 1, gridPos.gridZ - 1);

        CellManager[] edgesToCheck0 = new CellManager[] {up,upLeft,left,downLeft,down,cellManager };
        CellManager[] edgesToCheck1 = new CellManager[] { up, upRight, right, downRight, down,cellManager };
        CellManager[] edgesToCheck2 = new CellManager[] { up, upLeft, left, upRight, right,cellManager };
        CellManager[] edgesToCheck3 = new CellManager[] { down, downLeft, left, downRight, right,cellManager };
        int times = 0;

        switch (edgeIndex)
        {
            case 0:
                for (int i = 0; i < edgesToCheck0.Length-1; i++)
                {
                    int edgeIndex1 = -1;
                    int edgeIndex2 = -1;

                    if(edgesToCheck0[i] == up && up != null)
                    {
                        edgeIndex1 = 0;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck0[i] == upLeft && upLeft != null)
                    {
                        edgeIndex1 = 1;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck0[i] == left && left != null)
                    {
                        edgeIndex1 = 2;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck0[i] == downLeft && downLeft != null)
                    {
                        edgeIndex1 = 1;
                        edgeIndex2 = 2;
                    }
                    if (edgesToCheck0[i] == down && down != null)
                    {
                        edgeIndex1 = 0;
                        edgeIndex2 = 2;
                    }

                    TeamPlaceable tp1;
                    if (edgesToCheck0[i].edges[edgeIndex1].gameObject == null)
                    {
                        tp1 = null;
                    }
                    else
                    {
                        tp1 = edgesToCheck0[i].edges[edgeIndex1].gameObject.GetComponent<TeamPlaceable>();
                    }

                    TeamPlaceable tp2;
                    if (edgesToCheck0[i].edges[edgeIndex2].gameObject == null)
                    {
                        tp2 = null;
                    }
                    else
                    {
                        tp2 = edgesToCheck0[i].edges[edgeIndex2].gameObject.GetComponent<TeamPlaceable>();
                    }

                    if (tp1 != null) 
                    { 
                        if(tp1.team == team)
                        {
                            times++;
                        }
                    }
                    if (tp2 != null) 
                    {
                        if (tp2.team == team)
                        {
                            times++;
                        }
                    }
                }
                //Fix
                for (int i = 0; i < 4; i++)
                {
                    TeamPlaceable tp1;
                    if (edgesToCheck0[5].edges[i].gameObject == null)
                    {
                        tp1 = null;
                    }
                    else
                    {
                        tp1 = edgesToCheck0[5].edges[i].gameObject.GetComponent<TeamPlaceable>();
                    }
                    if (tp1 != null)
                    {
                        if (tp1.team == team)
                        {
                            times++;
                        }
                    }
                }
                //Fix
                if (times == 2)
                {
                    return true;
                }
                return false;
            case 1:
                for (int i = 0; i < edgesToCheck1.Length-1; i++)
                {
                    int edgeIndex1 = -1;
                    int edgeIndex2 = -1;

                    if (edgesToCheck1[i] == up && up != null)
                    {
                        edgeIndex1 = 1;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck1[i] == upRight && upRight != null)
                    {
                        edgeIndex1 = 0;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck1[i] == right && right != null)
                    {
                        edgeIndex1 = 2;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck1[i] == downRight && downRight != null)
                    {
                        edgeIndex1 = 0;
                        edgeIndex2 = 2;
                    }
                    if (edgesToCheck1[i] == down && down != null)
                    {
                        edgeIndex1 = 1;
                        edgeIndex2 = 2;
                    }

                    TeamPlaceable tp1;
                    if (edgesToCheck1[i].edges[edgeIndex1].gameObject == null)
                    {
                        tp1 = null;
                    }
                    else
                    {
                        tp1 = edgesToCheck1[i].edges[edgeIndex1].gameObject.GetComponent<TeamPlaceable>();
                    }

                    TeamPlaceable tp2;
                    if (edgesToCheck1[i].edges[edgeIndex2].gameObject == null)
                    {
                        tp2 = null;
                    }
                    else
                    {
                        tp2 = edgesToCheck1[i].edges[edgeIndex2].gameObject.GetComponent<TeamPlaceable>();
                    }

                    if (tp1 != null)
                    {
                        if (tp1.team == team)
                        {
                            times++;
                        }
                    }
                    if (tp2 != null)
                    {
                        if (tp2.team == team)
                        {
                            times++;
                        }
                    }
                }

                //Fix
                for (int i = 0; i < 4; i++)
                {
                    TeamPlaceable tp1;
                    if (edgesToCheck0[5].edges[i].gameObject == null)
                    {
                        tp1 = null;
                    }
                    else
                    {
                        tp1 = edgesToCheck0[5].edges[i].gameObject.GetComponent<TeamPlaceable>();
                    }
                    if (tp1 != null)
                    {
                        if (tp1.team == team)
                        {
                            times++;
                        }
                    }
                }
                //Fix

                if (times == 2)
                {
                    return true;
                }
                return false;
            case 2:
                for (int i = 0; i < edgesToCheck2.Length-1; i++)
                {
                    int edgeIndex1 = -1;
                    int edgeIndex2 = -1;

                    if (edgesToCheck2[i] == up && up != null)
                    {
                        edgeIndex1 = 0;
                        edgeIndex2 = 1;
                    }
                    if (edgesToCheck2[i] == upLeft && upLeft != null)
                    {
                        edgeIndex1 = 1;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck2[i] == left && left != null)
                    {
                        edgeIndex1 = 1;
                        edgeIndex2 = 2;
                    }
                    if (edgesToCheck2[i] == upRight && upRight != null)
                    {
                        edgeIndex1 = 0;
                        edgeIndex2 = 3;
                    }
                    if (edgesToCheck2[i] == right && right != null)
                    {
                        edgeIndex1 = 0;
                        edgeIndex2 = 2;
                    }

                    TeamPlaceable tp1;
                    if (edgesToCheck2[i].edges[edgeIndex1].gameObject == null)
                    {
                        tp1 = null;
                    }
                    else
                    {
                        tp1 = edgesToCheck2[i].edges[edgeIndex1].gameObject.GetComponent<TeamPlaceable>();
                    }

                    TeamPlaceable tp2;
                    if (edgesToCheck2[i].edges[edgeIndex2].gameObject == null)
                    {
                        tp2 = null;
                    }
                    else
                    {
                        tp2 = edgesToCheck2[i].edges[edgeIndex2].gameObject.GetComponent<TeamPlaceable>();
                    }

                    if (tp1 != null)
                    {
                        if (tp1.team == team)
                        {
                            times++;
                        }
                    }
                    if (tp2 != null)
                    {
                        if (tp2.team == team)
                        {
                            times++;
                        }
                    }
                }

                //Fix
                for (int i = 0; i < 4; i++)
                {
                    TeamPlaceable tp1;
                    if (edgesToCheck0[5].edges[i].gameObject == null)
                    {
                        tp1 = null;
                    }
                    else
                    {
                        tp1 = edgesToCheck0[5].edges[i].gameObject.GetComponent<TeamPlaceable>();
                    }
                    if (tp1 != null)
                    {
                        if (tp1.team == team)
                        {
                            times++;
                        }
                    }
                }
                //Fix

                if (times == 2)
                {
                    return true;
                }
                return false;
            case 3:
                {
                    for (int i = 0; i < edgesToCheck3.Length-1; i++)
                    {
                        int edgeIndex1 = -1;
                        int edgeIndex2 = -1;

                        if (edgesToCheck3[i] == down && down != null)
                        {
                            edgeIndex1 = 0;
                            edgeIndex2 = 1;
                        }
                        if (edgesToCheck3[i] == downLeft && downLeft != null)
                        {
                            edgeIndex1 = 1;
                            edgeIndex2 = 2;
                        }
                        if (edgesToCheck3[i] == left && left != null)
                        {
                            edgeIndex1 = 1;
                            edgeIndex2 = 3;
                        }
                        if (edgesToCheck3[i] == downRight && downRight != null)
                        {
                            edgeIndex1 = 0;
                            edgeIndex2 = 2;
                        }
                        if (edgesToCheck3[i] == right && right != null)
                        {
                            edgeIndex1 = 0;
                            edgeIndex2 = 3;
                        }

                        TeamPlaceable tp1;
                        if (edgesToCheck3[i].edges[edgeIndex1].gameObject == null)
                        {
                            tp1 = null;
                        }
                        else
                        {
                            tp1 = edgesToCheck3[i].edges[edgeIndex1].gameObject.GetComponent<TeamPlaceable>();
                        }

                        TeamPlaceable tp2;
                        if (edgesToCheck3[i].edges[edgeIndex2].gameObject == null)
                        {
                            tp2 = null;
                        }
                        else
                        {
                            tp2 = edgesToCheck3[i].edges[edgeIndex2].gameObject.GetComponent<TeamPlaceable>();
                        }

                        if (tp1 != null)
                        {
                            if (tp1.team == team)
                            {
                                times++;
                            }
                        }
                        if (tp2 != null)
                        {
                            if (tp2.team == team)
                            {
                                times++;
                            }
                        }
                    }

                    //Fix
                    for (int i = 0; i < 4; i++)
                    {
                        TeamPlaceable tp1;
                        if (edgesToCheck0[5].edges[i].gameObject == null)
                        {
                            tp1 = null;
                        }
                        else
                        {
                            tp1 = edgesToCheck0[5].edges[i].gameObject.GetComponent<TeamPlaceable>();
                        }
                        if (tp1 != null)
                        {
                            if (tp1.team == team)
                            {
                                times++;
                            }
                        }
                    }
                    //Fix

                    if (times == 2)
                    {
                        return true;
                    }
                    return false;
                }
            default:
                return false;
        }
    }

    public class EdgeCenterBounds
    {
        public Vector3 left { get; }
        public Vector3 right { get; }
        public Vector3 top { get; }
        public Vector3 bottom { get; }
        public Vector3[] edges { get; private set; }

        public EdgeCenterBounds(Bounds bound)
        {
            left = bound.center - new Vector3(bound.extents.x,0,0);
            right = bound.center + new Vector3(bound.extents.x, 0, 0);
            top = bound.center + new Vector3(0, 0, bound.extents.z);
            bottom = bound.center - new Vector3(0, 0, bound.extents.z);
            AddCornersToArray();
        }

        private void AddCornersToArray()
        {
            edges = new Vector3[4];
            edges[0] = left;
            edges[1] = right;
            edges[2] = top;
            edges[3] = bottom;
        }
    }

    public class EdgeAvailability
    {
        /*
        public List<IPlaceable> leftEdge = new List<IPlaceable>();
        public List<IPlaceable> rightEdge = new List<IPlaceable>();
        public List<IPlaceable> topEdge = new List<IPlaceable>();
        public List<IPlaceable> bottomEdge = new List<IPlaceable>();

        private List<IPlaceable> GetEdge(int indicatorIndex)
        {
            switch (indicatorIndex)
            {
                case 0:
                    return leftEdge;
                case 1:
                    return rightEdge;
                case 2:
                    return topEdge;
                case 3:
                    return bottomEdge;
                default:
                    return null;
            }
        }

        public void AddObject(int indicator,IPlaceable TKObject)
        {
            if (!CheckAvailability(indicator, TKObject))
            {
                return;
            }
            GetEdge(indicator).Add(TKObject);
        }

        
        public bool CheckAvailability(int indicator,IPlaceable TKObject)
        {
            List<ObjectsOnEdge> currentEdge = GetEdge(indicator);

            if (currentEdge.Contains(ObjectsOnEdge.Castle))
            {
                return false;
            }
            else if (currentEdge.Contains(ObjectsOnEdge.Wall))
            {
                return false;
            }
        }
        */
    }

    [System.Serializable]
    public class GridPosition : IComparable
    {
        public int gridX;
        public int gridZ;

        public static bool operator ==(GridPosition g1,GridPosition g2)
        {
            if (g1.gridX == g2.gridX && g1.gridZ == g2.gridZ)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(GridPosition g1, GridPosition g2)
        {
            if (g1.gridX == g2.gridX && g1.gridZ == g2.gridZ)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetGrid(int x,int z)
        {
            gridX = x;
            gridZ = z;
        }

        public int CompareTo(object obj)
        {
            GridPosition pos = obj as GridPosition;
            if (pos.gridX == this.gridX && pos.gridZ == this.gridZ)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }

    [System.Serializable]
    public class EdgeObjectPlace
    {
        public int edgeIndex;
        public GameObject gameObject;
    }
}
