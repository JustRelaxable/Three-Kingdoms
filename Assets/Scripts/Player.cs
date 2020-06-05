using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Player : NetworkBehaviour
{
    GameManager gameManager;
    NetworkIdentity networkIdentity;
    public Material playerColor;
    public Material[] playerMaterials;

    public int dieAmount { get; private set; } = -1;

    int foodResources = 0;
    int stoneResources = 0;
    int sulphurResources = 0;

    int foodMine = 0;
    int stoneMine = 0;
    int sulphurMine = 0;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        networkIdentity = GetComponent<NetworkIdentity>();
    }

    public void OnRoundStart()
    {
        FillResources();
    }

    public void FillResources()
    {
        foodResources += (foodMine * 2);
        stoneResources += (stoneMine * 2);
        sulphurResources += (sulphurMine);
    }

    public void RollDie()
    {
        int random1 = Random.Range(1, 4);
        int random2 = Random.Range(1, 4);
        dieAmount = (random1 + random2);
        RpcUpdateDieOnClients(dieAmount);
    }

    [ClientRpc]
    public void RpcUpdateDieOnClients(int dieAmount)
    {
        this.dieAmount = dieAmount;
        Debug.Log(dieAmount);
    }

    public void DecreaseDie()
    {
        dieAmount -= 1;
        if(dieAmount == 0)
        {
            gameManager.CmdTurnFinished(networkIdentity.netId.Value);
        }
    }

    [ClientRpc]
    public void RpcAssignColor(int colorIndex)
    {
        playerColor = playerMaterials[colorIndex];
    }

    public Material GetColor()
    {
        return playerColor;
    }
}
