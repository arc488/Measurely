using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    [SerializeField] Canvas appCanvas = null;

    string path;
    string[] imagePaths;


    void Start()
    {
        path = Application.persistentDataPath;
    }

    #region Take Screenshot 
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
    #endregion







}
