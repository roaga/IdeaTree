using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager {
    public static Dictionary<string, GameObject> trees = new Dictionary<string, GameObject>();
    public static bool editorOpen = false;
    public static bool notepadOpen = false;
    public static const int NUM_VERTICES_IN_SHAPE = 6;
}