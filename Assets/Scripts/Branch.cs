using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Branch {
    const int NUM_VERTICES_IN_SHAPE = Manager.NUM_VERTICES_IN_SHAPE;

    string text;
    Branch parent;
    List<Branch> children;
    int levelNum;
    Vector3 top;
    Vector3 bottom;
    Vector3 rotation;
    GameObject buttonArray;
    public string id;

    List<Vector3> vertices;
    List<int> triangles;

    List<GameObject> leaves;

    public Branch(string text, Branch parent, List<Branch> children, int levelNum, List<Vector3> initVert) {
        this.text = text;
        this.parent = parent;
        this.children = children == null ? new List<Branch>() : children;
        this.levelNum = levelNum;
        id = Guid.NewGuid().ToString("N");
        vertices = initVert;
        leaves = new List<GameObject>();

        GenerateLeaves();
    }

    public void AddChild(Branch child) {
        children.Add(child);
    }

    public void SetText(string text) {
        this.text = text;

        if (text.Length > 25) {
            leaves[0].SetActive(true);
        } else {
            leaves[0].SetActive(false);
        }
        if (text.Length > 60) {
            leaves[1].SetActive(true);
        } else {
            leaves[1].SetActive(false);
        }
        if (text.Length > 150) {
            leaves[2].SetActive(true);
        } else {
            leaves[2].SetActive(false);
        }
    }

    public string GetText() {
        return text;
    }

    public Branch GetParent() {
        return parent;
    }

    public List<Branch> GetChildren() {
        return children;
    }

    public void DeleteChild(string id) {
        children.RemoveAll(child => child.id == id);
    }

    public int GetLevelNum() {
        return levelNum;
    }

    public void SetTop(Vector3 pos) {
        top = pos;
    }

    public Vector3 GetTop() {
        return top;
    }
    public void SetBase(Vector3 pos) {
        bottom = pos;
    }

    public Vector3 GetBase() {
        return bottom;
    }

    public void SetRotation(Vector3 rotation) {
        this.rotation = rotation;
    }

    public Vector3 GetRotation() {
        return rotation;
    }

    public void SetButtonArray(GameObject buttonArray) { 
        this.buttonArray = buttonArray;
    }

    public List<Vector3> GetVertices() {
        return vertices;
    }

    public List<int> GetTriangles() {
        return triangles;
    }

    public void SetVertices(List<Vector3> newVert) {
        vertices = newVert;
        CalculateTriangles();
    }

    private void CalculateTriangles() {
        triangles.Clear();
        for (int vertex = 0; vertex < NUM_VERTICES_IN_SHAPE; vertex++) {
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

    void GenerateLeaves() { 
        for (int j = 0; j < NUM_VERTICES_IN_SHAPE; j++) {
            GameObject leavesHigh = GameObject.Instantiate(leaves, (vertices[j] + vertices[j + NUM_VERTICES_IN_SHAPE]) / 1.1f + basePosition, UnityEngine.Random.rotation);
            GameObject leavesMed = GameObject.Instantiate(leaves, (vertices[j] + vertices[j + NUM_VERTICES_IN_SHAPE]) / 1.3f + basePosition, UnityEngine.Random.rotation);
            GameObject leavesLow = GameObject.Instantiate(leaves, (vertices[j] + vertices[j + NUM_VERTICES_IN_SHAPE]) / 1.7f + basePosition, UnityEngine.Random.rotation);
            
            leavesHigh.SetActive(false);
            leavesMed.SetActive(false);
            leavesLow.SetActive(false);

            leaves.Add(leavesHigh);
            leaves.Add(leavesMed);
            leaves.Add(leavesLow);
        }
    }
}