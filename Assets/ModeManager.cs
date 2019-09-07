using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public AppMode appMode;

    public static ModeManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public enum AppMode
    {
        edit,
        add
    }

    void Start()
    {
        appMode = AppMode.add;
    }

    public void EditMode()
    {
        appMode = AppMode.edit;
    }
    
    public void AddMode()
    {
        appMode = AppMode.add;
    }

}
