using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelSelectContoller : MonoBehaviour {

    GameObject loading;

    public void Start()
    {

        foreach (var level in transform.GetComponentsInChildren<Button>())
            level.onClick.AddListener(() => StartLevel(level.gameObject.name));

        loading = transform.parent.FindChild("Loading").gameObject;
        loading.SetActive(false);
    }

    public void StartLevel(string level)
    {
        loading.SetActive(true);
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        SceneManager.LoadSceneAsync(level);
    }
}
