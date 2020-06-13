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
    public PlayerTeam team;
    Market market;

    public int dieAmount { get; private set; } = -1;

    public int foodResources = 20;
    public int stoneResources = 20;
    public int sulphurResources = 10;
    public int currency = 10;
    public int canonResources = 0;
    public int wallsResources = 0;

    int foodMine = 0;
    int stoneMine = 0;
    int sulphurMine = 0;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        networkIdentity = GetComponent<NetworkIdentity>();
        market = GameObject.FindObjectOfType<Market>();
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

        gameManager.FoodAmountPlayer.text = foodResources.ToString();
        gameManager.StoneAmountPlayer.text = stoneResources.ToString();
        gameManager.GunPowderAmountPlayer.text = sulphurResources.ToString();
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
        gameManager.ActionPoints.text = dieAmount.ToString();
    }

    public void DecreaseDie()
    {
        dieAmount -= 1;
        if (dieAmount == 0)
        {
            gameManager.CmdTurnFinished(networkIdentity.netId.Value);
        }
    }

    public void PlaceWall()
    {
        if (foodResources < 0)
        {
            DecreaseDie();
            this.foodResources -= 1;
            gameManager.FoodAmountPlayer.text = foodResources.ToString();
            gameManager.ActionPoints.text = dieAmount.ToString();
        }
        else
        {

        }
    }

    public bool IsFoodResourceGreaterThan0()
    {
        return (this.foodResources > 0);
    }

    [ClientRpc]
    public void RpcAssignColor(int colorIndex)
    {
        playerColor = playerMaterials[colorIndex];
    }

    [ClientRpc]
    public void RpcPlayerAssignTeam(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                team = PlayerTeam.Blue;
                break;
            case 1:
                team = PlayerTeam.Green;
                break;
            case 2:
                team = PlayerTeam.Red;
                break;
            default:
                break;
        }
    }

    public Material GetColor()
    {
        return playerColor;
    }

    public void BuyResource(MarketResourceType resource)
    {
        if (currency > 0)
        {
            switch (resource)
            {
                case MarketResourceType.Food:
                    currency -= 1;
                    foodResources += 1;
                    gameManager.FoodAmountPlayer.text = foodResources.ToString();
                    gameManager.Currency.text = currency.ToString();
                    market.CmdDecreaseMarket(MarketResourceType.Food);
                    break;
                case MarketResourceType.Stone:
                    currency -= 1;
                    stoneResources += 1;
                    gameManager.StoneAmountPlayer.text = stoneResources.ToString();
                    gameManager.Currency.text = currency.ToString();
                    market.CmdDecreaseMarket(MarketResourceType.Stone);
                    break;
                case MarketResourceType.Sulphur:
                    currency -= 1;
                    sulphurResources += 1;
                    gameManager.GunPowderAmountPlayer.text = sulphurResources.ToString();
                    gameManager.Currency.text = currency.ToString();
                    market.CmdDecreaseMarket(MarketResourceType.Sulphur);
                    break;
                default:
                    break;
            }
        }
    }

    public void SellResource(MarketResourceType resource)
    {
        switch (resource)
        {
            case MarketResourceType.Food:
                if (foodResources > 0)
                {
                    currency += 1;
                    foodResources -= 1;
                    gameManager.FoodAmountPlayer.text = foodResources.ToString();
                    gameManager.Currency.text = currency.ToString();
                    market.CmdIncreaseMarket(MarketResourceType.Food);
                    gameManager.FoodInMarket.text = market.food.ToString();
                }
                break;
            case MarketResourceType.Stone:
                if (stoneResources > 0)
                {
                    currency += 1;
                    stoneResources -= 1;
                    gameManager.StoneAmountPlayer.text = stoneResources.ToString();
                    gameManager.Currency.text = currency.ToString();
                    market.CmdIncreaseMarket(MarketResourceType.Stone);
                    gameManager.StoneInMarket.text = market.stone.ToString();
                }
                break;
            case MarketResourceType.Sulphur:
                if (sulphurResources > 0)
                {
                    currency += 1;
                    sulphurResources -= 1;
                    gameManager.GunPowderAmountPlayer.text = sulphurResources.ToString();
                    gameManager.Currency.text = currency.ToString();
                    market.CmdIncreaseMarket(MarketResourceType.Sulphur);
                    gameManager.GunpowderInMarket.text = market.sulphur.ToString();
                }
                break;
            default:
                break;
        }
    }
}

public enum MarketResourceType
{
    Food, Stone, Sulphur
}
