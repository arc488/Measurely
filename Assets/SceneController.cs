using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using System.IO;
using System;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject marker = null;
    [SerializeField] GameObject anchorMarker = null;
    List<Anchor> anchors;
    LineRenderer line;
    GameObject markerInstance;
    Pose currentPose;

    void Start()
    {
        anchors = new List<Anchor>();
        CreateLineRenderer();
    }


    void Update()
    {
        if (Session.Status != SessionStatus.Tracking) return;

        ControlMarker();

        if (anchors.Count < 2)
        {
            line.enabled = false;
            return;
        }

        DrawLines();
    }

    private void ControlMarker()
    {
        TrackableHit hit;
        TrackableHitFlags filter;
        filter = TrackableHitFlags.FeaturePoint | TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, filter, out hit))
        {
            marker.transform.position = hit.Pose.position;
            currentPose = hit.Pose;
        }
    }

    private void DrawLines()
    {
        line.enabled = true;

        line.positionCount = anchors.Count;

        for (int i = 0; i < anchors.Count; i++)
        {
            var position = anchors[i].transform.position;
            Instantiate(anchorMarker, position, Quaternion.identity);
            line.SetPosition(i, position);
        }
    }

    private void CreateLineRenderer()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.startColor = Color.green;
        line.endColor = Color.green;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
    }

    public void AddToList()
    {
        var myAnchor = Session.CreateAnchor(currentPose);
        anchors.Add(myAnchor);
    
        //string path;
        //path = Application.persistentDataPath + "/point" + System.DateTime.UtcNow + ".txt";

        //var stream = new StreamWriter(path);

        //for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        //{
        //    points.Add(Frame.PointCloud.GetPointAsStruct(i));
        //}

        //for (int i = 0; i < points.Count; i++)
        //{
        //    Debug.Log("i = " + i);
        //    Debug.Log("Position: " + points[i].Position);
        //    Debug.Log("Confidence: " + points[i].Confidence);

        //    PointCloudData myPoint = new PointCloudData();
        //    myPoint.position = points[i].Position;
        //    myPoint.confidence = points[i].Confidence;

        //    string jsonPoint = JsonUtility.ToJson(myPoint);

        //    stream.WriteLine(jsonPoint); 
        //}

        //stream.Close();
    }
}
