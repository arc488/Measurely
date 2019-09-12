using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotHandler : MonoBehaviour
{
    [SerializeField] Image imageDisplayer;
    [SerializeField] Canvas appCanvas = null;
    [SerializeField] Canvas galleryCanvas = null;
    public int currentDisplayerImage = 0;

    List<Texture2D> images;
    string[] imagePaths;
    string path;
    public Sprite sprite;


    // Start is called before the first frame update
    void Start()
    {
        images = new List<Texture2D>();

        path = Application.persistentDataPath;

    }

    // Update is called once per frame
    void Update()
    {
        HandleEscapeKey();

        if (images.Count < 1) return;

        var tex = images[images.Count - 1];
        sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        imageDisplayer.sprite = sprite;
    }

    public void TakeScreenshot()
    {
        var files = Directory.GetFiles(path, "MES*");

        var filename = "MES" + files.Length + ".png";
        Debug.Log(filename);

        appCanvas.enabled = false;
        ScreenCapture.CaptureScreenshot(filename);
        StartCoroutine(WaitForSave(filename));

    }

    public IEnumerator WaitForSave(string filename)
    {
        yield return new WaitUntil(() => IsScreencapSaved(filename));
        appCanvas.enabled = true;
    }


    public bool IsScreencapSaved(string filename)
    {
        imagePaths = Directory.GetFiles(path, "MES*");
        foreach (var path in imagePaths)
        {
            if (path.Contains(filename)) return true;
        }
        return false;
    }

    public void ShowGallery()
    {
        appCanvas.enabled = false;
        galleryCanvas.enabled = true;
        
        imagePaths = Directory.GetFiles(path, "MES*");

        if (imagePaths.Length < 1) return;

        Texture2D tex;

        foreach (var imagePath in imagePaths)
        {
            Debug.Log("Image path: " + imagePath);
            var fileData = File.ReadAllBytes(imagePath);
            tex = new Texture2D(1, 1);
            tex.LoadImage(fileData);
            images.Add(tex);
        }
        Debug.Log("Images count " + images.Count);

    }


    private void HandleEscapeKey()
    {
        if (Input.GetKey(KeyCode.Escape) && galleryCanvas.enabled == true)
        {
            appCanvas.enabled = true;
            galleryCanvas.enabled = false;
        }
    }

}
