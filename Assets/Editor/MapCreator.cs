using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapCreator : EditorWindow
{
    Object mapSO;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Map Creator")]
    static void Init()
    {
        GetWindow<MapCreator>("Map Creator", true);
    }

    void OnGUI()
    {
        GUILayout.Label("Map", EditorStyles.boldLabel);
        mapSO = EditorGUILayout.ObjectField(mapSO, typeof(MapSO), true);

        if(mapSO != null)
        {
        }
    }
}
