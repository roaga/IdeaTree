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

    public Vector3 basePosition;
    public DateTime dateCreated;
    int numLevels = 0;
    float heightFactor;
    float thicknessFactor;
    List<Branch> branches;

    public void SpawnTree(Vector3 basePos, DateTime date) {
        basePosition = basePos;
        dateCreated = date;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Branch rootBranch = new Branch("", null, null, 1);
        branches = new List<Branch>();
        branches.Add(rootBranch);
        numLevels = 1;
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
        float verticalOffset = random.Next(-45, 10);
        if (verticalOffset == 0) { verticalOffset -= 5; }
        float horizontalOffset = -80 + (siblings.Count - 1) * 10f; 
        
        // apply vertical rotation
        rotation.x += verticalOffset * (float) Math.Sin(rotation.x);
        rotation.z += verticalOffset * (float) Math.Cos(rotation.z);

        // apply horizontal rotation
        rotation.y += horizontalOffset;

        // calculate branch
        CalculateBranch(rotation, basePos, newBranch.GetLevelNum(), newBranch);

        // instantiate buttons
        Vector3 buttonPos = new Vector3(0, 0.5f * heightFactor / (newBranch.GetLevelNum() * 0.5f), 0);
        Vector3 dir = buttonPos - basePos;
        dir = Quaternion.Euler(rotation) * dir;
        buttonPos = dir + basePos;
        GameObject buttons = Instantiate(branchButtons, buttonPos, Quaternion.identity);
        Button newButton = buttons.transform.Find("NewBranchButton").gameObject.GetComponent<Button>();
        Button editButton = buttons.transform.Find("EditBranchButton").gameObject.GetComponent<Button>();
        Button deleteButton = buttons.transform.Find("DeleteBranchButton").gameObject.GetComponent<Button>();
        newButton.onClick.AddListener(() => ButtonActions(newButton));
        editButton.onClick.AddListener(() => ButtonActions(editButton));
        deleteButton.onClick.AddListener(() => ButtonActions(deleteButton));
    }

    void CalculateBranch(Vector3 rotation, Vector3 rootPos, int levelNum, Branch branch) {
        // calculate center of top face using rotation and length factor
        Vector3 topCenter = new Vector3(0, heightFactor / (levelNum * 0.5f), 0);
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
        for (int vertex = (branches.Count - 1) * 2 * NUM_VERTICES_IN_SHAPE; vertex < (branches.Count - 1) * 2 * NUM_VERTICES_IN_SHAPE + NUM_VERTICES_IN_SHAPE; vertex++) {
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
    }
    
    void UpdateMesh() {
        UpdateThicknessAndHeight();
        GenerateLeaves();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        // GetComponent<MeshCollider>().sharedMesh = null;
        // GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void ReloadMesh() {
        UpdateThicknessAndHeight();
        GenerateLeaves();

        // load save data and set mesh and triangles, branches

    }

    void SaveData() { 
        // save mesh and triangles, branches data
    }

    void UpdateThicknessAndHeight() {
        if (thicknessFactor < 2) {
            thicknessFactor = 0.1f + 0.1f * (float) (DateTime.Now - dateCreated).TotalDays;
        } else {
            thicknessFactor = 0.1f;
        }
        heightFactor = 0.5f + 0.5f * branches.Count;
    }

    void ButtonActions(Button button) {

    }

    void GenerateLeaves() {
        for (int i = NUM_VERTICES_IN_SHAPE; i < vertices.Count; i += NUM_VERTICES_IN_SHAPE) {
            for (int j = 0; j < NUM_VERTICES_IN_SHAPE; j += (NUM_VERTICES_IN_SHAPE / branches.Count / 2)) {
                Instantiate(leaves, (vertices[i + j] + vertices[i + j - 6]) / 1.1f + basePosition, UnityEngine.Random.rotation);
                Instantiate(leaves, (vertices[i + j] + vertices[i + j - 6]) / 1.3f + basePosition, UnityEngine.Random.rotation);
                Instantiate(leaves, (vertices[i + j] + vertices[i + j - 6]) / 1.7f + basePosition, UnityEngine.Random.rotation);
            }
        }
    }
}