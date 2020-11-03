using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreeGenerator : MonoBehaviour {

    public GameObject leaves;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    const int NUM_VERTICES_IN_SHAPE = 6; // other code will have to be changed too if this changes

    Vector3 basePosition;
    int numLevels = 0;
    int heightFactor = 1;
    int thicknessFactor = 1;
    List<Branch> branches;

    public void SpawnTree(Vector3 basePos) {
        basePosition = basePos;

        Branch rootBranch = new Branch("", null, null, 1);
        branches = new List<Branch>();
        branches.Add(rootBranch);
        numLevels = 1;

        // calculate base vertices
        int i = 0;
        for (double angle = 0.0; angle < 360.0; angle +=  360.0 / NUM_VERTICES_IN_SHAPE) {
            vertices[i] = new Vector3(basePosition.x + thicknessFactor * Math.Cos(angle), basePosition.y, basePosition.z + thicknessFactor * Math.Sin(angle));
            i++;
        }

        float[] verticesToAdd = CalculateBranch(new Vector3(0, 0, 0), basePos, heightFactor, rootBranch.GetLevelNum());
        for (int i = 0; i < verticesToAdd.Length; i++) {
            vertices[i + 6] = verticesToAdd[i];
        }

        CreateShape();
    }

    void NewBranch() {

        

    }

    float[] CalculateBranchVertices(Vector3 rotation, Vector3 rootPos, float length, int levelNum) {
        float[] vertices = new float[NUM_VERTICES_IN_SHAPE * 2];

        // TODO: calculate center using rotation and length factor
        Vector3 center = basePosition;

        // calculate top vertices
        int i = 0;
        for (double angle = 0.0; angle < 360.0; angle +=  360.0 / NUM_VERTICES_IN_SHAPE) {
            
            i++;
        }

        // calculate triangles using previous vertices as base


        return vertices;
    }

    void CalculateLengths() { 

    }

    void CalculateThicknesses() {
        
    }

    void CreateShape() {
        Debug.Log("TODO: generate tree mesh");


        UpdateMesh();
    }
    
    void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        // GetComponent<MeshCollider>().sharedMesh = null;
        // GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}