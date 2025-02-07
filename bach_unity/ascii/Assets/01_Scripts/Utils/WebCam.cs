﻿using UnityEngine;
using System.Collections;
using System.IO;

public class WebCam : MonoBehaviour {
    public int Width = 1280;
    public int Height = 720;
    public int FPS = 15;

    public Material material;
    private int photoIndex;

    WebCamTexture webcamTexture;

    void Start() {

        DirectoryInfo target = new DirectoryInfo(Application.dataPath + "/../outputs/");
        foreach (FileInfo file in target.GetFiles()) {
            file.Delete();
        }


        target = new DirectoryInfo(Application.dataPath + "/../images/");
        foreach (FileInfo file in target.GetFiles()) {
            file.Delete();
        }

        target = new DirectoryInfo(Application.dataPath + "/../images/reading/");
        foreach (FileInfo file in target.GetFiles()) {
            file.Delete();
        }
        target = new DirectoryInfo(Application.dataPath + "/../images/reading/old/");
        foreach (FileInfo file in target.GetFiles()) {
            file.Delete();
        }


        WebCamDevice[] devices = WebCamTexture.devices;

        // display all cameras
        for (var i = 0; i < devices.Length; i++) {
            // get camera name
            string camname = devices[i].name;
            print(i + ":" + camname);

            webcamTexture = new WebCamTexture(camname, Width, Height, FPS);
            print(webcamTexture);
            material.mainTexture = webcamTexture;
            webcamTexture.Play();
            break;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
        }
    }

    public void SaveImage() {
        if (webcamTexture != null) {
            SaveToPNGFile(webcamTexture.GetPixels(), Application.dataPath + "/../images/" + photoIndex.ToString("00000000") + ".png");
            photoIndex++;
        }
    }

    void SaveToPNGFile(Color[] texData, string filename) {
        Texture2D takenPhoto = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

        takenPhoto.SetPixels(texData);
        takenPhoto.Apply();

        byte[] png = takenPhoto.EncodeToPNG();
        Destroy(takenPhoto);

        File.WriteAllBytes(filename, png);
    }
}