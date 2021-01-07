using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notepad : MonoBehaviour
{
    GameObject canvas;
    static Branch branch;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.transform.Find("Container").gameObject;
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (branch != null) {
            if (Manager.notepadOpen && !canvas.activeSelf) {
                canvas.SetActive(true);
                canvas.transform.Find("InputField (TMP)").GetComponent<TMP_InputField>().text = branch.GetText();
            } else if (!Manager.notepadOpen && canvas.activeSelf) {
                branch.SetText(canvas.transform.Find("InputField (TMP)").GetComponent<TMP_InputField>().text);
                canvas.SetActive(false);
            }
        }

        if (!Manager.editorOpen) {
            Manager.notepadOpen = false;
        }
    }

    public static void SetBranch(Branch newBranch) {
        branch = newBranch;
    }
}
