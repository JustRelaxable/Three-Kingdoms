using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    
    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
            
        }
        if (isServer)
        {
            RegisterPlayer();
        }
    }

    
    void Update()
    {
        
    }

    public override void OnStartClient()
    {
        //RegisterPlayer();
        SetPlayerSpawnPoint();
    }

    private void SetPlayerSpawnPoint()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn Point");
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
    }

    private void RegisterPlayer()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.RegisterPlayer(gameObject);
    }
}
