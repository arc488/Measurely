using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour
{

    [SerializeField] Image imageDisplayer = null;
    [SerializeField] Canvas appCanvas = null;
    [SerializeField] Canvas galleryCanvas = null;

    public int displayImageIndex;

    List<Texture2D> images;
    string[] imagePaths;
    public Sprite sprite;

    string path;

    void Start()
    {
        path = Application.persistentDataPath;

        images = new List<Texture2D>();
    }

    void Update()
    {
        HandleEscapeKey();

        if (images.Count < 1) return;

        var tex = images[displayImageIndex];
        sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        imageDisplayer.sprite = sprite;
    }

    public void ShowGallery()
    {
        appCanvas.enabled = false;
        galleryCanvas.enabled = true;

        GetImageTextures();

        displayImageIndex = images.Count - 1;
    }

    public void GetImageTextures()
    {
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
