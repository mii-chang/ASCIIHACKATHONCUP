﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Note : MonoBehaviour {

    [SerializeField] private Transform[] muzzlePositions;
    [SerializeField] private GameObject lineObj;
    [SerializeField] private GameObject puffObj;

    private NoteManager noteManager;
    private SoundManager soundManager;
    private GameObject line;
    private GameObject puff;

    public NoteData Data { get; private set; }

    public void SetData(NoteData data) {
        Data = data;
        gameObject.SetActive(true);
    }

    private void Awake() {
        noteManager = NoteManager.Instance;
        soundManager = SoundManager.Instance;
    }

    private void Start() {
        Init();
        CalcNote();
    }

    private void Init() {
        transform.position = muzzlePositions[Data.Type].position;
        GetComponent<ParticleSystem>().Play();
        line = Instantiate(lineObj);
        line.transform.SetParent(transform);
    }

    private void CalcNote() {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                var t = (Data.Time - soundManager.Time);
                var rate = t / NoteManager.DisplayTime;

                line.transform.localPosition = new Vector3(
                    0,
                    0,
                    Mathf.Lerp(76, 0, rate)
                );

                if (t < -NoteManager.MissTime) {
                    noteManager.Evaluate(this, Const.DecisionResult.Miss);
                }
            });
    }

    public void Fired(GameObject fireWorkObj) {
        var obj = Instantiate(fireWorkObj, transform.position + Vector3.up * 76, Quaternion.identity) as GameObject;
        obj.GetComponent<ParticleSystem>().Play();
    }

    public void Falled() {
        puff = Instantiate(puffObj);
        puff.transform.position = muzzlePositions[Data.Type].position + Vector3.up * 76;
        puff.SetActive(true);
        puff.GetComponent<ParticleSystem>().Play();
    }
}
