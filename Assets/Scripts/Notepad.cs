using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notepad : MonoBehaviour
{
    GameObject canvas;
    Branch branch;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.transform.Find("Container").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.notepadOpen) {
            canvas.Find("InputField").GetComponent<TMP_InputField>().text = branch.GetText();
            canvas.SetActive(true);
        } else {
            branch.SetText(canvas.Find("InputField").GetComponent<TMP_InputField>().text);
            canvas.SetActive(false);
        }

        if (!Manager.editorOpen) {
            Manager.notepadOpen = false;
        }
        
        // TODO: save text to branch
    }

    public static void SetBranch(Branch branch) {
        this.branch = branch;
    }
}
