using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingAndSellingUpdateUI : MonoBehaviour
{
    GameManager gameManager;
    Player player;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = gameManager.currentPlayer.GetComponent<Player>();
    }

    public void BuyResource(string resourceType)
    {
        switch (resourceType)
        {
            case "Food":
                player.BuyResource(global::MarketResourceType.Food);
                break;
            case "Stone":
                player.BuyResource(global::MarketResourceType.Stone);
                break;
            case "Sulphur":
                player.BuyResource(global::MarketResourceType.Sulphur);
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
                player.SellResource(global::MarketResourceType.Food);
                break;
            case "Stone":
                player.SellResource(global::MarketResourceType.Stone);
                break;
            case "Sulphur":
                player.SellResource(global::MarketResourceType.Sulphur);
                break;
            default:
                break;
        }
    }
}
