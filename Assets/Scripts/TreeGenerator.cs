using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreeGenerator : MonoBehaviour {

    public GameObject leaves;

    Mesh mesh;
    List<Vector3> vertices;
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
        vertices = new List<Vector3>();
        int i = 0;
        for (double angle = 0.0; angle < 360.0; angle +=  360.0 / NUM_VERTICES_IN_SHAPE) {
            vertices.Add(new Vector3(basePosition.x + thicknessFactor * Math.Cos(angle), basePosition.y, basePosition.z + thicknessFactor * Math.Sin(angle)));
            i++;
        }

        float[] verticesToAdd = CalculateBranch(new Vector3(0, 0, 0), basePos, heightFactor, rootBranch.GetLevelNum());
        for (int i = 0; i < verticesToAdd.Length; i++) {
            vertices.Add(verticesToAdd[i]);
        }

        CreateShape();
    }

    void NewBranch() {

        

    }

    float[] CalculateBranch(Vector3 rotation, Vector3 rootPos, float length, int levelNum) {
        float[] vertices = new float[NUM_VERTICES_IN_SHAPE * 2];

        // calculate center of top face using rotation and length factor
        Vector3 topCenter = new Vector3(basePosition.x, basePosition.y + heightFactor / (levelNum * 0.5), basePosition.z);
        Vector3 dir = topCenter - rootPos;
        dir = Quaternion.Euler(rotatation) * dir;
        topCenter = dir + rootPos;

        // calculate top vertices
        float thickness = thicknessFactor * numLevels * 0.5f / levelNum;
        int i = 0;
        for (double angle = 0.0; angle < 360.0; angle +=  360.0 / NUM_VERTICES_IN_SHAPE) {
            vertices.Add(new Vector3(topCenter.x + thickness * Math.Cos(angle), topCenter.y, topCenter.z + thickness * Math.Sin(angle)));
            i++;
        }

        // TODO: calculate triangles using previous vertices as base


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
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        // GetComponent<MeshCollider>().sharedMesh = null;
        // GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}