using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlePlace : TeamPlaceable
{

    private void Start()
    {
        StartCoroutine(PlaceCaastlePlace());
    }

    IEnumerator PlaceCaastlePlace()
    {
        yield return new WaitForSeconds(3);
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.gameObject.CompareTag("Cell"))
            {
                CellManager cellManager = hitInfo.collider.gameObject.GetComponent<CellManager>();
                int index = cellManager.FindNearestCenterEdgeForCastle(hitInfo.point);
                cellManager.edges[index].gameObject = this.gameObject;
            }
        }
    }
}

public enum PlayerTeam
{
    Red,Blue,Green
}
