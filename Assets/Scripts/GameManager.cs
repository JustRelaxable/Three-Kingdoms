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
        //StartGameSession();
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

    public GameObject GetPlayerGOFromConnectionID(uint connectionID)
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

    public Player GetPlayerFromConnectionID(uint connectionID)
    {
        Player[] players = GameObject.FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            NetworkIdentity networkIdentity = players[i].GetComponent<NetworkIdentity>();
            if (networkIdentity.netId.Value == connectionID && networkIdentity.isClient)
            {
                return players[i];
            }
        }
        return null;
    }

    private void StartGameSession()
    {
        ThrowDiesForPlayers();
        PutAndSelectRandomConnectionID();
        AssignPlayerColors();
    }

    private void ThrowDiesForPlayers()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            allPlayers[i].GetComponent<Player>().RollDie();
            Debug.Log(allPlayers[i].GetComponent<Player>().dieAmount);
        }
    }

    public void PutAndSelectRandomConnectionID()
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

    public void AssignPlayerColors()
    {
        for (int i = 0; i < connectionIDs.Count; i++)
        {
            AssignColorToPlayer(connectionIDs[i], i);
        }
    }

    public void AssignColorToPlayer(uint connectionID,int colorIndex)
    {
        Player player = GetPlayerFromConnectionID(connectionID);
        player.RpcAssignColor(colorIndex);
        player.RpcPlayerAssignTeam(colorIndex);
    }
}
