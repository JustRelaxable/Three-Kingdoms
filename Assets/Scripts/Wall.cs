using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour,IPlaceable
{
    private GameObject player;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = gameManager.currentPlayer;

    }

    private void Start()
    {
        
    }
}
