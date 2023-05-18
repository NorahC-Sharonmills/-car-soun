using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEditor
{

    [MenuItem("Scene/Loading %s1")]
    static void OpenSceneWorldCup()
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/_CAT/Loading.unity");
    }

    [MenuItem("Scene/Game %s2")]
    static void OpenSceneSortBall()
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/_CAT/Game.unity");
    }
}
