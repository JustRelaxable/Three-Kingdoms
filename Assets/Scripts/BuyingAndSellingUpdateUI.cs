using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BuyingAndSellingUpdateUI : NetworkBehaviour
{
    public Text FoodAmountPlayer;
    public Text StoneAmountPlayer;
    public Text GunPowderAmountPlayer;
    public Text Canon;
    public Text Walls;
    public Text Currency;
    public Text FoodInMarket;
    public Text StoneInMarket;
    public Text GunpowderInMarket;
    public Text ActionPoints;

    public GameObject PanelLoading;
    public GameObject InGameMarket;


    GameManager gameManager;
    Player player;

    public void OnGameStart()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void BuyResource(string resourceType)
    {
        switch (resourceType)
        {
            case "Food":
                player.BuyResource(MarketResourceType.Food);
                break;
            case "Stone":
                player.BuyResource(MarketResourceType.Stone);
                break;
            case "Sulphur":
                player.BuyResource(MarketResourceType.Sulphur);
                break;
            default:
                break;
        }
        
    }

    public void SellResources(string resourceType)
    {

        switch (resourceType)
        {
            case "Food":
                player.SellResource(MarketResourceType.Food);
                break;
            case "Stone":
                player.SellResource(MarketResourceType.Stone);
                break;
            case "Sulphur":
                player.SellResource(MarketResourceType.Sulphur);
                break;
            default:
                break;
        }
    }

    public void AllocateResourcesToPlayer()
    {
        FoodAmountPlayer.text = player.foodResources.ToString();
        StoneAmountPlayer.text = player.stoneResources.ToString();
        GunPowderAmountPlayer.text = player.sulphurResources.ToString();
        Canon.text = player.canonResources.ToString();
        Walls.text = player.wallsResources.ToString();
        ActionPoints.text = player.dieAmount.ToString();
    }

    public void AssignPlayer(Player player)
    {
        this.player = player;
    }

    public void GameLoaded()
    {
        PanelLoading.SetActive(false);
        InGameMarket.SetActive(true);
    }
}
