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
}
