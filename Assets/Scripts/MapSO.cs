using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[CreateAssetMenu(fileName ="Map",menuName ="Map")]
public class MapSO : ScriptableObject
{
    public CoordinateInformation[] coordinateInformations;

    public Tile GetTileTypeFromCoords(int index)
    {
        return coordinateInformations[index].tile;
    }
}

[System.Serializable]
public class CoordinateInformation
{
    public int row;
    public int column;
    public Tile tile;
}

public enum Tile
{
    Grass,Rock,Sand
}
