using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    public GameObject currentPlayer { get; private set; }
    [SerializeField]
    GameObject[] allPlayers = new GameObject[3];
    List<uint> connectionIDs = new List<uint>();
    [SyncVar] public uint turnConnectionID;
    private bool isGameReady = false;

    private void Start()
    {
        turnConnectionID = uint.MinValue;
        StartGameSession();
    }

    public void RegisterPlayer(GameObject player)
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            if(allPlayers[i] == null)
            {
                allPlayers[i] = player;
                break;
            }
        }

        if(allPlayers[2] != null)
        {
            StartGameSession();
        }
    }

    public GameObject GetPlayerFromConnectionID(uint connectionID)
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            if(allPlayers[i].GetComponent<NetworkIdentity>().netId.Value == connectionID)
            {
                return allPlayers[i];
            }
        }
        return null;
    }

    private void StartGameSession()
    {
        RpcThrowDiesForPlayers();
        RpcPutAndSelectRandomConnectionID();
    }

    [ClientRpc]
    private void RpcThrowDiesForPlayers()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            allPlayers[i].GetComponent<Player>().RollDie();
            Debug.Log(allPlayers[i].GetComponent<Player>().dieAmount);
        }

        for (int i = 0; i < allPlayers.Length; i++)
        {

        }
    }

    [ClientRpc]
    public void RpcPutAndSelectRandomConnectionID()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            connectionIDs.Add(allPlayers[i].GetComponent<NetworkIdentity>().netId.Value);
        }
        turnConnectionID = connectionIDs[0];
    }

    [Command]
    public void CmdTurnFinished(uint connectionID)
    {
        int index = connectionIDs.FindIndex(c => c == connectionID);
        index = index == 2 ? 0 : index + 1;
        turnConnectionID = connectionIDs[index];
    }
}
