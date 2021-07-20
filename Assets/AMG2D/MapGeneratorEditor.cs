using System;
using AMG2D.Configuration;
using UnityEditor;
using UnityEngine;

namespace AMG2D
{
    /*
    [CustomEditor(typeof(MapGenerator), true)]
    [CanEditMultipleObjects]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mapGenerator = (MapGenerator)target;
            if (mapGenerator.Config == null) mapGenerator.Config = new GeneralMapConfig();

            mapGenerator.Config.Height = EditorGUILayout.IntField("Map Height", mapGenerator.Config.Height);
            mapGenerator.Config.Width = EditorGUILayout.IntField("Map Width", mapGenerator.Config.Width);

            if (GUILayout.Button("Generate")) mapGenerator.Generate();
        }
    }*/
}
