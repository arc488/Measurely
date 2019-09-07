using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using System.IO;
using System;
using TMPro;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject marker = null;
    [SerializeField] GameObject anchorMarker = null;
    [SerializeField] GameObject meterText = null;
    [SerializeField] Camera firstPersonCamera = null;

    List<DistanceMarker> distanceMarkers;

    AppMode appMode;
    LineRenderer line;
    Pose currentPose;

    /// <summary>
    /// Contains enums
    /// </summary>
    #region Enums
    public enum AppMode
    {
        edit,
        add
    }

    public class DistanceMarker
    {
        public Anchor anchor;
        public GameObject anchorMarker;
        public GameObject distanceDisplayInstance;
    }
    #endregion

    #region Toggle app mode methods
    public void EditMode()
    {
        appMode = AppMode.edit;
    }

    public void AddMode()
    {
        appMode = AppMode.add;
    }
    #endregion

    void Start()
    {
        distanceMarkers = new List<DistanceMarker>();
        appMode = AppMode.add;
        CreateLineRenderer();
    }


    void Update()
    {
        if (Session.Status != SessionStatus.Tracking) return;
        if (distanceMarkers == null) distanceMarkers = new List<DistanceMarker>();

        ControlMarker();
        RotateDistances();

        if (distanceMarkers.Count < 1)
        {
            line.enabled = false;
            return;
        }

        DrawLines();

        if (distanceMarkers.Count < 2) return;

        UpdateDistances();
    }

    private void ControlMarker()
    {
        TrackableHit hit;
        TrackableHitFlags filter;
        filter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon; //| TrackableHitFlags.FeaturePoint ;

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, filter, out hit))
        {
            marker.transform.position = hit.Pose.position;
            currentPose = hit.Pose;
        }
    }

    private void RotateDistances()
    {
        foreach (DistanceMarker marker in distanceMarkers)
        {
            if (marker.distanceDisplayInstance == null) continue;
            marker.distanceDisplayInstance.transform.LookAt(firstPersonCamera.transform);
        }
    }

    private void DrawLines()
    {
        line.enabled = true;

        line.positionCount = distanceMarkers.Count + 1;

        for (int i = 0; i < distanceMarkers.Count; i++)
        {
            var position = distanceMarkers[i].anchor.transform.position;
            line.SetPosition(i, position);
        }

        line.SetPosition(line.positionCount - 1, marker.transform.position);
    }


    public void UpdateDistances()
    {
        for (int i = 1; i < distanceMarkers.Count; i++)
        {
            var instance = distanceMarkers[i].distanceDisplayInstance;

            var distance = Vector3.Distance(distanceMarkers[i].anchor.transform.position, distanceMarkers[i - 1].anchor.transform.position);

            var text = instance.GetComponentInChildren<TextMeshPro>();

            var displayPosition = (distanceMarkers[i].anchor.transform.position + distanceMarkers[i - 1].anchor.transform.position) / 2;
            displayPosition.y += 0.2f;
            instance.transform.position = displayPosition;

            text.text = string.Format("{0:#.00} m", distance);

        }
    }
 


    #region Removal methods

    public void UndoLast()
    {
        if (distanceMarkers.Count - 1 < 0) return;
        RemoveMarker();
    }

    public void ClearAll()
    {
        if (distanceMarkers.Count - 1 < 0) return;
        while (distanceMarkers.Count > 0)
        {
            RemoveMarker();
        }
    }

    public void RemoveMarker()
    {
        Destroy(distanceMarkers[distanceMarkers.Count - 1].distanceDisplayInstance);
        Destroy(distanceMarkers[distanceMarkers.Count - 1].anchorMarker);
        Destroy(distanceMarkers[distanceMarkers.Count - 1].anchor);
        distanceMarkers.RemoveAt(distanceMarkers.Count - 1);
    }
    #endregion


    public void AddToList()
    {
        var dm = new DistanceMarker();

        dm.anchor = Session.CreateAnchor(currentPose);
        dm.distanceDisplayInstance = Instantiate(meterText);
        dm.anchorMarker = Instantiate(anchorMarker);
        dm.anchorMarker.transform.position = dm.anchor.transform.position;
       
        distanceMarkers.Add(dm);
    }

    private void CreateLineRenderer()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.startColor = Color.green;
        line.endColor = Color.green;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
    }
}
