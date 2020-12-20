using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor : MonoBehaviour
{
    GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.transform.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.editorOpen) {
            canvas.SetActive(true);
        } else {
            canvas.SetActive(false);
        }
    }
}
