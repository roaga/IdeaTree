using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreeGenerator : MonoBehaviour {

    public GameObject leaves;
    public List<Button[]> branchButtons;

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
        List<Vector3> siblings = parent.GetChildren();
        System.Random random = new System.Random();
        float verticalOffset = random.Next(-45, 10);
        if (verticalOffset == 0) { verticalOffset -= 5; }
        float horizontalOffset = -80 + (siblings.Count - 1) * 10f; 
        
        // apply vertical rotation
        rotation.x += verticalOffset * Math.Sin(rotation.x);
        rotation.z += verticalOffset * Math.Cos(rotation.z);

        // apply horizontal rotation
        rotation.y += horizontalOffset;

        // calculate branch
        CalculateBranch(rotation, basePos, newBranch.GetLevelNum());
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

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        // GetComponent<MeshCollider>().sharedMesh = null;
        // GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void ReloadMesh() {
        UpdateThicknessAndHeight();

        // load save data and set mesh and triangles, branches

    }

    void SaveData() { 
        // save mesh and triangles, branches data
    }

    void UpdateThicknessAndHeight() {
        if (thicknessFactor < 2) {
            thicknessFactor = 0.1f + 0.1f * (DateTime.Now - dateCreated).TotalDays;
        } else {
            thicknessFactor = 0.1f;
        }
        heightFactor = 0.5f + 0.5f * branches.Count;
    }
}