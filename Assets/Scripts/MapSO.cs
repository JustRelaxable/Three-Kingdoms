using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[CreateAssetMenu(fileName ="Map",menuName ="Map")]
public class MapSO : ScriptableObject
{
    public CoordinateInformation[] coordinateInformations;

    public Tile GetTileTypeFromCoords(int x,int z)
    {
        for (int i = 0; i < coordinateInformations.Length; i++)
        {
            if(x == coordinateInformations[i].column && z == coordinateInformations[i].row)
            {
                return coordinateInformations[i].tile;
            }
        }
        return Tile.Grass;

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
