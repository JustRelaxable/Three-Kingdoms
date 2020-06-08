using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MouseManager : NetworkBehaviour
{
    public Camera mainCamera;
    bool isPlaceable = false;
    CellManager currentCellManager;
    NetworkIdentity networkIdentity;
    Player player;
    GameManager gameManager;
    public GameObject wall;
    [SerializeField] GameObject castle;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        networkIdentity = GetComponent<NetworkIdentity>();
        player = GetComponent<Player>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        CastRayFromMousePos();
    }

    public void CastRayFromMousePos()
    {
        var CastleKey = Input.GetAxis("Jump");  // Amed wrote
        if (isLocalPlayer)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                if (hitInfo.collider.gameObject.CompareTag("Cell"))
                {
                    CellManager cellManager = hitInfo.collider.gameObject.GetComponent<CellManager>();
                    cellManager.FindNearestCenterEdge(hitInfo.point);
                }

                if (hitInfo.collider.gameObject.CompareTag("Cell") && Input.GetMouseButtonDown(0) && CastleKey<=0 ) // Amed Changed
                {
                    isPlaceable = player.IsFoodResourceGreaterThan0();
                    currentCellManager = hitInfo.collider.gameObject.GetComponent<CellManager>();
                    var tuple = currentCellManager.PlaceWallInfo();
                    bool isPlaceable2 = currentCellManager.IsEdgePlaceable(tuple.Item3, player.team, tuple.Item4);

                    if(isPlaceable2 && isPlaceable)
                    {
                        player.PlaceWall();
                    }

                    Debug.Log(isPlaceable);
                    CmdPlaceWall((isPlaceable2 && isPlaceable),tuple.Item1,tuple.Item2,tuple.Item3,tuple.Item4);
                }


                if (hitInfo.collider.gameObject.CompareTag("Cell") && Input.GetMouseButtonDown(0) && CastleKey>0 ) // Amed wrote
                {
                    isPlaceable = true;
                    currentCellManager = hitInfo.collider.gameObject.GetComponent<CellManager>();
                    var tuple = currentCellManager.PlaceCastle();
                    CmdPlaceCastle(tuple.Item1, tuple.Item2);
                }

                else
                {
                    isPlaceable = false;
                }
            }
            else
            {
                CellManager.ResetIndicatorPos();
            }
        }
    }

    [Command]
    public void CmdPlaceWall(bool isPlaceable,Vector3 pos,Quaternion rot,int indicatorPosIndex,CellManager.GridPosition gridPosition)
    {
        if(networkIdentity.netId.Value != gameManager.turnConnectionID)
        {
            return;
        }
        if (isPlaceable)
        {
            player.PlaceWall();
            GameObject spawnedObject = Instantiate(wall, pos, rot);
            AssignWallColor(spawnedObject);
            AssignTeam(spawnedObject);
            NetworkServer.SpawnWithClientAuthority(spawnedObject, connectionToClient);
            RpcAssignColorOnClients(spawnedObject);
            RpcAssignObjectToEdge(indicatorPosIndex, gridPosition,spawnedObject);
            RpcAssignTeam(spawnedObject);
        }
    }
    [ClientRpc]
    private void RpcAssignObjectToEdge(int indicatorPosIndex, CellManager.GridPosition gridPosition,GameObject spawnedObject)
    {
        CellManager cell = CellManager.CellFromGridPosition(gridPosition);
        if(cell != null)
        {
            cell.edges[indicatorPosIndex].gameObject = spawnedObject;

            switch (indicatorPosIndex)
            {
                case 0:
                    gridPosition.gridX -= 1;
                    if (true) 
                    {
                        CellManager c = CellManager.CellFromGridPosition(gridPosition);
                        if(c == null)
                        {
                            return;
                        }
                        c.edges[1].gameObject = spawnedObject; 
                    }
                        break;
                case 1:
                    gridPosition.gridX += 1;
                    if (true)
                    {
                        CellManager c = CellManager.CellFromGridPosition(gridPosition);
                        if (c == null)
                        {
                            return;
                        }
                        c.edges[0].gameObject = spawnedObject;
                    }
                    break;
                case 2:
                    gridPosition.gridZ += 1;
                    if (true)
                    {
                        CellManager c = CellManager.CellFromGridPosition(gridPosition);
                        if (c == null)
                        {
                            return;
                        }
                        c.edges[3].gameObject = spawnedObject;
                    }
                    break;
                case 3:
                    gridPosition.gridZ -= 1;
                    if (true)
                    {
                        CellManager c = CellManager.CellFromGridPosition(gridPosition);
                        if (c == null)
                        {
                            return;
                        }
                        c.edges[2].gameObject = spawnedObject;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void AssignWallColor(GameObject spawnedObject)
    {
        Wall spawnedWall = spawnedObject.GetComponent<Wall>();
        spawnedWall.meshRenderer.material = player.GetColor();
        for (int i = 0; i < spawnedWall.lights.Length; i++)
        {
            spawnedWall.lights[i].color = player.GetColor().color;
        }
    }
    [ClientRpc]
    public void RpcAssignColorOnClients(GameObject GO)
    {
        AssignWallColor(GO);
    }

    [Command]
    public void CmdPlaceCastle( Vector3 pos, Quaternion rot)
    {
        GameObject spawnedObject = Instantiate(castle, pos, rot);
        NetworkServer.SpawnWithClientAuthority(spawnedObject, connectionToClient);
    }

    public void AssignTeam(GameObject GO)
    {
        GO.GetComponent<TeamPlaceable>().team = player.team;
    }
    [ClientRpc]
    public void RpcAssignTeam(GameObject GO)
    {
        GO.GetComponent<TeamPlaceable>().team = player.team;
    }
}
