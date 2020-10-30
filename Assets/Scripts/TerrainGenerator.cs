﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public GameObject grassPatch;
    public GameObject[] accessories = new GameObject[5];

    public int xSize = 20;
    public int zSize = 20;
    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
    }

    void Update() {
        UpdateMesh();
    }

    void CreateShape() {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                y += Mathf.PerlinNoise(x * 0.2f, z * 0.2f) * 3f;
                vertices[i] = new Vector3(x, y, z);

                // grass spawn
                if (y < 2.5) { // avoid hills
                    Instantiate(grassPatch, new Vector3(x, y, z), Quaternion.identity);
                    Instantiate(grassPatch, new Vector3(Math.Abs(x - 0.2f), y, Math.Abs(z - 0.2f)), Quaternion.identity);
                    Instantiate(grassPatch, new Vector3(Math.Abs(x + 0.2f), y, Math.Abs(z + 0.2f)), Quaternion.identity);
                    Instantiate(grassPatch, new Vector3(Math.Abs(x - 0.2f), y, Math.Abs(z + 0.2f)), Quaternion.identity);
                    Instantiate(grassPatch, new Vector3(Math.Abs(x + 0.2f), y, Math.Abs(z - 0.2f)), Quaternion.identity);
                }
                // random terrain accessory spawn
                if (random.Next(0, 10) < 1) {
                    int num = random.Next(0, accessories.Length);
                    Instantiate(accessories[num], new Vector3(x, y, z), Quaternion.Euler(new Vector3(0, random.Next(0, 359), 0)));
                }

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }

    void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
    
}
