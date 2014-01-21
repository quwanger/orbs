using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TTSWaypointEditor : EditorWindow
{
    private List<GameObject> _selectedGameObjects = new List<GameObject>();
    private Vector2 _scrollPosition;

    [MenuItem("Waypoints/Waypoint Editor...")]
    private static void showEditor(){
        EditorWindow.GetWindow<TTSWaypointEditor>(false, "Waypoint Editor");
    }

    [MenuItem("Waypoints/Waypoint Editor...", true)]
    private static bool showEditorValidator(){ return true; }

    void OnSelectionChange(){
        _selectedGameObjects = Selection.gameObjects.Where(go => go.GetComponent<TTSWaypoint>() != null).ToList();
        Repaint();
    }

    void OnGUI(){
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        {
            foreach(GameObject go in _selectedGameObjects){

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel(go.name);
                    // GUILayout.Button("asdfasdf");
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
