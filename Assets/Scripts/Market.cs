using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Market : NetworkBehaviour
{
    [SyncVar] public int sulphur = 10;
    [SyncVar] public int stone = 10;
    [SyncVar] public int food = 10;


    [Command]
    public void CmdIncreaseMarket(MarketResourceType type)
    {
        switch (type)
        {
            case MarketResourceType.Food:
                food += 1;
                break;
            case MarketResourceType.Stone:
                stone += 1;
                break;
            case MarketResourceType.Sulphur:
                sulphur += 1;
                break;
            default:
                break;
        }
    }

    [Command]
    public void CmdDecreaseMarket(MarketResourceType type)
    {
        switch (type)
        {
            case MarketResourceType.Food:
                food -= 1;
                break;
            case MarketResourceType.Stone:
                stone -= 1;
                break;
            case MarketResourceType.Sulphur:
                sulphur -= 1;
                break;
            default:
                break;
        }
    }
}
