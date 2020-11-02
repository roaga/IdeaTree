using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Branch {
    string text;
    Branch parent;
    List<Branch> children;
    bool isLeaf;

    public Branch(string text, Branch parent, List<Branch> children, bool isLeaf) {
        this.text = text;
        this.parent = parent;
        this.children = children;
        this.isLeaf = isLeaf;
    }

    public void addChild(Branch child) {
        children.Add(child);
    }

    public void setText(string text) {
        this.text = text;
    }

    public string getText() {
        return text;
    }

    public Branch getParent() {
        return parent;
    }

    public List<Branch> getChildren() {
        return children;
    }

    public bool getIsLeaf() {
        return isLeaf;
    }
}