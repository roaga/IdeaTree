using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreeGenerator : MonoBehaviour {

    public GameObject leaves;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    const int NUM_VERTICES_IN_SHAPE = 6; // other code will have to be changed too if this changes

    Vector3 basePosition;
    int numLevels = 0;
    int heightFactor = 1;
    int thicknessFactor = 1;
    List<Branch> branches;

    public void SpawnTree(Vector3 basePos) {
        basePosition = basePos;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Branch rootBranch = new Branch("", null, null, 1);
        branches = new List<Branch>();
        branches.Add(rootBranch);
        numLevels = 1;

        // calculate base vertices
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for (double angle = 0.0; angle < 360.0; angle +=  360.0 / NUM_VERTICES_IN_SHAPE) {
            vertices.Add(new Vector3(thicknessFactor * (float) Math.Cos(angle), 0, thicknessFactor * (float) Math.Sin(angle)));
        }

        CalculateBranch(new Vector3(0, 0, 0), basePos, heightFactor, rootBranch.GetLevelNum());

        UpdateMesh();
    }

    void NewBranch() {

        

    }

    void CalculateBranch(Vector3 rotation, Vector3 rootPos, float length, int levelNum) {
        // calculate center of top face using rotation and length factor
        Vector3 topCenter = new Vector3(0, heightFactor / (levelNum * 0.5f), 0);
        Vector3 dir = topCenter - rootPos;
        dir = Quaternion.Euler(rotation) * dir;
        topCenter = dir + rootPos;

        // calculate top vertices
        float thickness = thicknessFactor * numLevels * 0.5f / levelNum;
        for (double angle = 0.0; angle < 360.0; angle +=  360.0 / NUM_VERTICES_IN_SHAPE) {
            vertices.Add(new Vector3(topCenter.x + thickness * (float) Math.Cos(angle), topCenter.y, topCenter.z + thickness * (float) Math.Sin(angle)));
        }

        // Calculate triangles using previous vertices as base; there should be 2 * NUM_VERTICES_IN_SHAPE = 12 triangles
        for (int vertex = (branches.Count - 1) * 2; vertex < NUM_VERTICES_IN_SHAPE; vertex++) {
            triangles.Add(vertex);
            if ((vertex + 1) % NUM_VERTICES_IN_SHAPE == 0) {
                triangles.Add(vertex + 1 - NUM_VERTICES_IN_SHAPE);
            } else {
                triangles.Add(vertex + 1);
            }
            triangles.Add(vertex + NUM_VERTICES_IN_SHAPE - 1);

            triangles.Add(vertex);
            triangles.Add(vertex + NUM_VERTICES_IN_SHAPE - 1);
            triangles.Add(vertex + NUM_VERTICES_IN_SHAPE);
        }
    }

    void CalculateLengths() { 

    }

    void CalculateThicknesses() {
        
    }
    
    void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        // GetComponent<MeshCollider>().sharedMesh = null;
        // GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}