using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using System.IO;
using System;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject marker = null;
    [SerializeField] GameObject anchorMarker = null;
    [SerializeField] GameObject meterText = null;
    [SerializeField] Camera firstPersonCamera = null;

    [SerializeField] GameObject editButton;
    [SerializeField] GameObject drawButton;
    [SerializeField] GameObject addButton;

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
        draw
    }
    #endregion

    public class DistanceMarker
    {
        public Anchor anchor;
        public GameObject anchorMarker;
        public GameObject distanceDisplayInstance;
    }

    #region Toggle app mode methods
    public void EditMode()
    {
        appMode = AppMode.edit;
        drawButton.SetActive(true);
        editButton.SetActive(false);
        addButton.SetActive(false);
        marker.SetActive(false);
    }

    public void DrawMode()
    {
        appMode = AppMode.draw;
        drawButton.SetActive(false);
        editButton.SetActive(true);
        addButton.SetActive(true);
        marker.SetActive(true);

    }
    #endregion

    void Start()
    {
        distanceMarkers = new List<DistanceMarker>();
        appMode = AppMode.draw;
        CreateLineRenderer();
    }


    void Update()
    {
        if (Session.Status != SessionStatus.Tracking) return;
        if (distanceMarkers == null) distanceMarkers = new List<DistanceMarker>();

        ControlMarker();
        RotateDistances();
        DisplayGizmos();
        UpdateAnchorPosition();

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

        line.positionCount = appMode == AppMode.draw ? distanceMarkers.Count + 1 : distanceMarkers.Count;

        for (int i = 0; i < distanceMarkers.Count; i++)
        {
            var position = distanceMarkers[i].anchor.transform.position;
            line.SetPosition(i, position);
        }

        if (appMode == AppMode.edit) return;
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

    public void UpdateAnchorPosition()
    {
        if (appMode == AppMode.draw) return;
        foreach (var distanceMarker in distanceMarkers)
        {
            distanceMarker.anchor.transform.position = distanceMarker.anchorMarker.transform.position;
        }

    }
    #region ScreenshotMethods


    #endregion


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


    public void DisplayGizmos()
    {
        if (distanceMarkers.Count < 1) return;
        bool gizmosEnabled = false;
        gizmosEnabled = appMode == AppMode.draw ? false : true;
        foreach (var distanceMarker in distanceMarkers)
        {
            distanceMarker.anchorMarker.transform.GetChild(0).gameObject.SetActive(gizmosEnabled);
        }
    }

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
