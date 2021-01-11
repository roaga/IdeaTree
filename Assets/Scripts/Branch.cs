using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Branch {
    string text;
    Branch parent;
    List<Branch> children;
    int levelNum;
    Vector3 top;
    Vector3 bottom;
    Vector3 rotation;
    GameObject buttonArray;
    public string id;

    public Branch(string text, Branch parent, List<Branch> children, int levelNum) {
        this.text = text;
        this.parent = parent;
        this.children = children == null ? new List<Branch>() : children;
        this.levelNum = levelNum;
        id = Guid.NewGuid().ToString("N");

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
}