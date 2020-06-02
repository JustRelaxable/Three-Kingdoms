using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ActionPointsAndDiceUI : MonoBehaviour
{
    int playerActionPoints = 0;
    public Text actionPointsText;
    public string actionPointsString;
    public static UnityAction decreaseActionPoints;

    void Start()
    {
        decreaseActionPoints += DecreaseActionPoints;
        UpdateActionPointsText();
    }

    
    void Update()
    {
        
    }

    public void RollTheDice()
    {
        int dice1 = Random.Range(1, 4);
        int dice2 = Random.Range(1, 4);
        playerActionPoints = dice1 + dice2;
        UpdateActionPointsText();
    }

    public void UpdateActionPointsText()
    {
        actionPointsText.text = actionPointsString + playerActionPoints;
    }

    public void DecreaseActionPoints()
    {
        playerActionPoints--;
        UpdateActionPointsText();
    }
}
