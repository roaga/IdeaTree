using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notepad : MonoBehaviour
{
    GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.transform.Find("Container").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.notepadOpen) {
            canvas.SetActive(true);
        } else {
            canvas.SetActive(false);
        }

        if (!Manager.editorOpen) {
            Manager.notepadOpen = false;
        }
        
    }
}
