﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour {

    [SerializeField] private NoteManager noteManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioClip se;
    [SerializeField] private DataDrawer dataDrawer;

    public float Time { get { return bgm.time; } }

    private bool isLoading;

    void Update() {
        if (!bgm.isPlaying && !isLoading) {
            StartCoroutine(loadResult());
        }
    }

    private IEnumerator loadResult() {
        isLoading = true;
        var loader = SceneManager.LoadSceneAsync("Result", LoadSceneMode.Additive);
        yield return loader;

        FindObjectOfType<ResultManager>().SetData(
            scoreManager.scoreDataDic[1],
            scoreManager.scoreDataDic[2]
        );
        SceneManager.UnloadScene("Sub");
    }

    public void PlaySE() {
        bgm.PlayOneShot(se);
    }
}
