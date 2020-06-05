using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour,IPlaceable
{
    public MeshRenderer meshRenderer;
    public Light[] lights;
    private void Awake()
    {
    }

    private void Start()
    {
        
    }
}
