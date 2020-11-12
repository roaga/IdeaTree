using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Branch {
    string text;
    Branch parent;
    List<Branch> children;
    int levelNum;
    Vector3 top;
    Vector3 bottom;
    Vector3 rotation;

    public Branch(string text, Branch parent, List<Branch> children, int levelNum) {
        this.text = text;
        this.parent = parent;
        this.children = children;
        this.levelNum = levelNum;

    }

    public void AddChild(Branch child) {
        children.Add(child);
    }

    public void SetText(string text) {
        this.text = text;
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
}