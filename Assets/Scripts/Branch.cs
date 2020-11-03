using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Branch {
    string text;
    Branch parent;
    List<Branch> children;
    int levelNum;

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
}