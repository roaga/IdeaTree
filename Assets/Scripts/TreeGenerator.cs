using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeGenerator : MonoBehaviour {

    public GameObject leaves;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    Vector3 basePosition;
    int numLevels = 0;
    int heightFactor = 1;
    int thicknessFactor = 1;
    List<Branch> branches;

    public void SpawnTree(Vector3 basePos) {
        basePosition = basePos;

        Branch rootBranch = new Branch("", null, null, true);
        branches = new List<Branch>();
        branches.Add(rootBranch);

        Debug.Log("TODO: generate hexagonal and tapering tree mesh with correct but random rotation");
    }

    void NewBranch() {

        

    }

    void NewLevel() {

    }

    void CalculateLengths() { 

    }

    void CalculateThicknesses() {
        
    }
    
}