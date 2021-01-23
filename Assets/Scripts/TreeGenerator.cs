using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class TreeGenerator : MonoBehaviour {

    public GameObject branchButtons;
    public GameObject leafPrefab;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    const int NUM_VERTICES_IN_SHAPE = Manager.NUM_VERTICES_IN_SHAPE;
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

        rootBranch = new Branch("", null, null, 1, null, basePosition, leafPrefab, null);
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

        ReloadMesh();
    }

    void NewBranch(Branch parent) {
        Branch newBranch = new Branch("", parent, null, parent.GetLevelNum() + 1, null, basePosition, leafPrefab, null);
        parent.AddChild(newBranch);

        Vector3 basePos = parent.GetTop();
        Vector3 parentBasePos = parent.GetBase();
        Vector3 rotation = parent.GetRotation();

        // offset rotation to avoid siblings and look natural
        List<Branch> siblings = parent.GetChildren();
        System.Random random = new System.Random();
        float verticalOffset = random.Next(-5, 45);
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
        ReloadMesh();
    }

    void CalculateBranch(Vector3 rotation, Vector3 rootPos, int levelNum, Branch branch) {
        // calculate center of top face using rotation and length factor
        Vector3 topCenter = new Vector3(0, 0.7f * heightFactor / (levelNum * 0.5f), 0);
        Vector3 dir = topCenter - rootPos;
        dir = Quaternion.Euler(rotation) * dir;
        topCenter = dir + rootPos;

        branch.SetTop(topCenter);
        branch.SetBase(rootPos);
        branch.SetRotation(rotation);

        List<Vector3> branchVert = new List<Vector3>();
        // calculate top vertices
        int thickness = (int) (thicknessFactor * numLevels * 0.5f) / levelNum;
        for (double angle = 0.0; angle < 360.0; angle += (360.0 / NUM_VERTICES_IN_SHAPE)) {
            double radAngle = angle * Math.PI / 180;
            branchVert.Add(new Vector3(topCenter.x + thickness * (float) Math.Cos(radAngle), topCenter.y, topCenter.z + thickness * (float) Math.Sin(radAngle)));
        }
        branch.SetVertices(branchVert);

        // instantiate branch buttons
        Destroy(branch.GetButtons());
        Vector3 buttonPos = new Vector3(0, 0.7f * heightFactor / (levelNum * 0.5f), 0);
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
        branch.SetButtons(buttons);
    }

    void UpdateThicknessAndHeight() {
        if (thicknessFactor < 2) {
            thicknessFactor = 0.1f + 0.1f * (float) (DateTime.Now - dateCreated).TotalDays;
        } else {
            thicknessFactor = 0.1f;
        }
        heightFactor = 0.5f + 0.5f * numBranches;
    }

    void ButtonActions(string button, Branch parent) {
        if (button == "newButton") {
            if (parent.GetChildren().Count < 10 && numLevels < 10) {
                NewBranch(parent);
            } else {
                // TODO: show message saying can't add more branches
                Debug.Log("Can't add more branches here");
            }
        } else if (button == "editButton") {
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
                Manager.editorOpen = false;
            } else {
                Branch grandparent = parent.GetParent();
                Queue<Branch> branches = new Queue<Branch>(); // level order traversal
                branches.Enqueue(parent);
                while (branches.Count > 0) {
                    Branch curr = branches.Dequeue();
                    if (curr != null) {
                        List<Branch> children = curr.GetChildren();
                        foreach (Branch child in children) {
                            branches.Enqueue(child);
                        }
                        Destroy(curr.GetButtons());
                        foreach (GameObject leaf in curr.GetLeaves()) {
                            Destroy(leaf);
                        }
                        numBranches--;
                    }
                }
                grandparent.DeleteChild(parent.id);
                ReloadMesh();
            }
        }
    }

    void UpdateMesh() {
        UpdateThicknessAndHeight();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void ReloadMesh() {
        // recalculate everything based on each branch; combine each branch's vertices and triangles
        int block = 0;
        vertices.Clear();
        triangles.Clear();

        // redo base
        for (double angle = 0.0; angle < 360.0; angle += (360.0 / NUM_VERTICES_IN_SHAPE)) {
            double radAngle = angle * Math.PI / 180;
            vertices.Add(new Vector3(thicknessFactor * (float) Math.Cos(radAngle), 0, thicknessFactor * (float) Math.Sin(radAngle)));
        }

        Queue<Branch> branches = new Queue<Branch>(); // level order traversal
        branches.Enqueue(rootBranch);
        while (branches.Count > 0) {
            Branch curr = branches.Dequeue();
            if (curr != null) {
                List<Branch> children = curr.GetChildren();
                foreach (Branch child in children) {
                    branches.Enqueue(child);
                }
                CalculateBranch(curr.GetRotation(), curr.GetBase(), curr.GetLevelNum(), curr);
                List<Vector3> newVert = curr.GetVertices();
                List<int> newTri = curr.GetTriangles();

                for (int i = 0; i < NUM_VERTICES_IN_SHAPE; i++) {
                    vertices.Add(newVert[i]);
                }
                for (int i = 0; i < NUM_VERTICES_IN_SHAPE * NUM_VERTICES_IN_SHAPE; i += 3) {
                    triangles.Add(newTri[i] + block);
                    triangles.Add(newTri[i + 1] + block);
                    triangles.Add(newTri[i + 2] + block);
                }
                block += NUM_VERTICES_IN_SHAPE;
            }
        }
        Debug.Log("Total vertices: " + vertices.Count);
        Debug.Log("Total triangles: " + triangles.Count);

        UpdateMesh();
    }

    void OnMouseDown() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject.name.Contains("TreePrefab") && !Manager.editorOpen) {
            Manager.editorOpen = true;
        }   
    }
}