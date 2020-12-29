using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor : MonoBehaviour
{
    GameObject canvas;
    Camera cameraToLookAt;

    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.transform.Find("Canvas").gameObject;
        cameraToLookAt = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.editorOpen) {
            canvas.SetActive(true);
        } else {
            canvas.SetActive(false);
        }

        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt( cameraToLookAt.transform.position - v ); 
        transform.rotation = cameraToLookAt.transform.rotation;
    }
}