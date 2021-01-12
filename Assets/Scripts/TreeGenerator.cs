using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class TreeGenerator : MonoBehaviour {

    public GameObject leaves;
    public GameObject branchButtons;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    const int NUM_VERTICES_IN_SHAPE = 6;
    string id;

    public Vector3 basePosition;
    public DateTime dateCreated;
    int numLevels = 0;
    int numBranches = 0;
    float heightFactor;
    float thicknessFactor;
    Branch rootBranch;

    public void SpawnTree(Vector3 basePos, DateTime date, string id) {
        basePosition = basePos;
        dateCreated = date;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        this.id = id;

        rootBranch = new Branch("", null, null, 1);
        numLevels = 1;
        numBranches = 1;
        UpdateThicknessAndHeight();

        // calculate base vertices
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for (double angle = 0.0; angle < 360.0; angle += (360.0 / NUM_VERTICES_IN_SHAPE)) {
            double radAngle = angle * Math.PI / 180;
            vertices.Add(new Vector3(thicknessFactor * (float) Math.Cos(radAngle), 0, thicknessFactor * (float) Math.Sin(radAngle)));
        }

        CalculateBranch(new Vector3(0, 0, 0), basePos, rootBranch.GetLevelNum(), rootBranch);

        UpdateMesh();
    }

    void NewBranch(Branch parent) {
        Branch newBranch = new Branch("", parent, null, parent.GetLevelNum() + 1);
        parent.AddChild(newBranch);

        Vector3 basePos = parent.GetTop();
        Vector3 parentBasePos = parent.GetBase();
        Vector3 rotation = parent.GetRotation();

        // offset rotation to avoid siblings and look natural
        List<Branch> siblings = parent.GetChildren();
        System.Random random = new System.Random();
        float verticalOffset = random.Next(-45, -5);
        if (verticalOffset == 0) { verticalOffset = -5; }
        float horizontalOffset = -80 + (siblings.Count - 1) * 20f; 
        
        // apply vertical rotation
        rotation.x += verticalOffset + (float) Math.Sin(rotation.x);
        rotation.z += verticalOffset + (float) Math.Cos(rotation.z);

        // apply horizontal rotation
        rotation.y += horizontalOffset;

        // calculate branch
        CalculateBranch(rotation, basePos, newBranch.GetLevelNum(), newBranch);
        numBranches++;
        UpdateMesh();
    }

    void CalculateBranch(Vector3 rotation, Vector3 rootPos, int levelNum, Branch branch) {
        // calculate center of top face using rotation and length factor
        Vector3 topCenter = new Vector3(0, 0.5f * heightFactor / (levelNum * 0.5f), 0);
        Vector3 dir = topCenter - rootPos;
        dir = Quaternion.Euler(rotation) * dir;
        topCenter = dir + rootPos;

        branch.SetTop(topCenter);
        branch.SetBase(rootPos);
        branch.SetRotation(rotation);

        // calculate top vertices
        int thickness = (int) (thicknessFactor * numLevels * 0.5f) / levelNum;
        for (double angle = 0.0; angle < 360.0; angle += (360.0 / NUM_VERTICES_IN_SHAPE)) {
            double radAngle = angle * Math.PI / 180;
            vertices.Add(new Vector3(topCenter.x + thickness * (float) Math.Cos(radAngle), topCenter.y, topCenter.z + thickness * (float) Math.Sin(radAngle)));
        }

        // Calculate triangles using previous vertices as base; there should be 2 * NUM_VERTICES_IN_SHAPE = 12 triangles
        for (int vertex = (numBranches - 1) * 2 * NUM_VERTICES_IN_SHAPE; vertex < (numBranches - 1) * 2 * NUM_VERTICES_IN_SHAPE + NUM_VERTICES_IN_SHAPE; vertex++) {
            triangles.Add(vertex + NUM_VERTICES_IN_SHAPE);
            if ((vertex + 1) % NUM_VERTICES_IN_SHAPE == 0) {
                triangles.Add(vertex + 1 - NUM_VERTICES_IN_SHAPE);
            } else {
                triangles.Add(vertex + 1);
            }
            triangles.Add(vertex);

            triangles.Add(vertex + NUM_VERTICES_IN_SHAPE);
            if ((vertex + 1 + NUM_VERTICES_IN_SHAPE) % NUM_VERTICES_IN_SHAPE == 0) {
                triangles.Add(vertex + 1);
            } else {
                triangles.Add(vertex + 1 + NUM_VERTICES_IN_SHAPE);
            }
            triangles.Add(vertex);
        }

        // instantiate buttons
        Vector3 buttonPos = new Vector3(0, 0.4f * heightFactor / (levelNum * 0.5f), 0);
        dir = buttonPos - rootPos;
        dir = Quaternion.Euler(rotation) * dir;
        buttonPos = dir + rootPos;
    
        GameObject buttons = GameObject.Instantiate(branchButtons, basePosition + buttonPos, Quaternion.identity, gameObject.transform) as GameObject;
        Button newButton = buttons.transform.Find("Canvas").Find("NewBranchButton").gameObject.GetComponent<Button>();
        Button editButton = buttons.transform.Find("Canvas").Find("EditBranchButton").gameObject.GetComponent<Button>();
        Button deleteButton = buttons.transform.Find("Canvas").Find("DeleteBranchButton").gameObject.GetComponent<Button>();
        newButton.onClick.AddListener(() => ButtonActions("newButton", branch));
        editButton.onClick.AddListener(() => ButtonActions("editButton", branch));
        deleteButton.onClick.AddListener(() => ButtonActions("deleteButton", branch));
    }

    void UpdateThicknessAndHeight() {
        if (thicknessFactor < 2) {
            thicknessFactor = 0.1f + 0.1f * (float) (DateTime.Now - dateCreated).TotalDays;
        } else {
            thicknessFactor = 0.1f;
        }
        heightFactor = 0.5f + 0.5f * numBranches;

        if (vertices != null) {
            for (int i = 0; i < vertices.Count - 6; i++) {
                Vector3 vertex = vertices[i];
                vertex = new Vector3(vertex.x, vertex.y * heightFactor, vertex.z);
                vertices[i] = vertex;
            }
        }
    }

    void ButtonActions(string button, Branch parent) {
        if (button == "newButton") {
            if (parent.GetChildren().Count < 10 && numLevels < 10) {
                NewBranch(parent);
                Debug.Log("Creating new branch...");
            } else {
                // TODO: show message saying can't add more branches
                Debug.Log("Can't add more branches here");
            }
        } else if (button == "editButton") {
            Debug.Log("Editing this branch...");
            if (!Manager.notepadOpen) {
                Notepad.SetBranch(parent);
                Manager.notepadOpen = true;
            } else {
                Manager.editorOpen = false;
            }
        } else if (button == "deleteButton") {
            if (parent.id == rootBranch.id) {
                Manager.trees.Remove(id);
                Destroy(gameObject);
            } else {
                Branch grandparent = parent.GetParent();
                grandparent.DeleteChild(parent.id);
                numBranches--;
                UpdateMesh();
                //TODO: Delete vertices and triangles, leaves
            }
        }
    }

    void GenerateLeaves() { // TODO: tie leaf generation to each branch and presence of note
        for (int i = vertices.Count - NUM_VERTICES_IN_SHAPE; i < vertices.Count; i += NUM_VERTICES_IN_SHAPE) {
            for (int j = 0; j < NUM_VERTICES_IN_SHAPE; j += (NUM_VERTICES_IN_SHAPE / numBranches / 2)) {
                Instantiate(leaves, (vertices[i + j] + vertices[i + j - 6]) / 1.1f + basePosition, UnityEngine.Random.rotation);
                Instantiate(leaves, (vertices[i + j] + vertices[i + j - 6]) / 1.3f + basePosition, UnityEngine.Random.rotation);
                Instantiate(leaves, (vertices[i + j] + vertices[i + j - 6]) / 1.7f + basePosition, UnityEngine.Random.rotation);
            }
        }
    }

    void OnMouseDown() {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject.name.Contains("TreePrefab") && !Manager.editorOpen) {
            Manager.editorOpen = true;
        }   
    }

    void UpdateMesh() {
        UpdateThicknessAndHeight();
        GenerateLeaves();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void ReloadMesh() {
        // load save data and set mesh and triangles, branches

    }

    void SaveData() { 
        // save mesh and triangles, branches data
    }
}