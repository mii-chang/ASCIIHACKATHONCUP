﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System;
using UniRx;
using UniRx.Triggers;

public class NoteManager : SingletonMonoBehaviour<NoteManager> {
    public const int BPM = 170;
    public const float BeatTime = 60f / BPM / 4f;
    public const float DisplayTime = 1.5f;
    public const float StartTime = 0.485f;
    public const float MissTime = 0.2f;

    public const int FireWorkMaxType = 3;

    [SerializeField] private SoundManager sound;
    [SerializeField] private TextAsset data;
    [SerializeField] private Note team1NoteBase;
    [SerializeField] private Note team2NoteBase;
    [SerializeField] private GameObject[] fireWorkObj;
    [SerializeField] private OscController oscController;

    private Queue<NoteData> team1NoteDatas = new Queue<NoteData>();
    private Queue<NoteData> team2NoteDatas = new Queue<NoteData>();

    private List<Note> notes = new List<Note>();

    public IObservable<DecesionResultData> onDecesionResultObservable {
        get { return decesionResuleSubject.AsObservable(); }
    }
    private Subject<DecesionResultData> decesionResuleSubject = new Subject<DecesionResultData>();


    private void Start() {
        CreateScore();
        CreateNote();
        JudgeNote();

        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Space)).Subscribe(_ => { oscController.SendScreenShotOSC(); });
    }


    public void JudgeNote() {
        oscController.onDeviceDataObservable
                     .Subscribe(data =>
                     {
                         for (int i = 0; i < FireWorkMaxType; i++) {
                             if (i == 0 && !data.isJump) continue;
                             if (i == 2 && !data.isLoudVoice) continue;
                             if (i == 1 && !(data.isJump && data.isLoudVoice)) continue;

                             var note = notes.FirstOrDefault(n => n.Data.Type == i && n.Data.Team == data.team);
                             if (!note) continue;
                             if (Mathf.Abs(note.Data.Time - sound.Time) < MissTime) {
                                 Evaluate(note, Const.DecisionResult.Perfect);
                             }
                         }
                     });
    }

    private void CreateNote() {
        this.UpdateAsObservable().Subscribe(_ =>
        {
            if (team1NoteDatas.Count != 0 && team1NoteDatas.Peek().Time - DisplayTime < sound.Time) {
                var note = Instantiate(team1NoteBase);
                var noteData = team1NoteDatas.Dequeue();
                note.transform.SetParent(transform);
                note.SetData(noteData);
                notes.Add(note);
            }

            if (team2NoteDatas.Count != 0 && team2NoteDatas.Peek().Time - DisplayTime < sound.Time) {
                var note = Instantiate(team2NoteBase);
                var noteData = team2NoteDatas.Dequeue();
                note.transform.SetParent(transform);
                note.SetData(noteData);
                notes.Add(note);
            }

        });
    }

    private void CreateScore() {
        float time = StartTime;
        foreach (var s in data.text.Split(',')) {
            foreach (var c in s) {
                var type = c - '0';
                if (type >= 0 && type < FireWorkMaxType) {
                    team1NoteDatas.Enqueue(new NoteData(Const.Team.team1, type, time));
                    team2NoteDatas.Enqueue(new NoteData(Const.Team.team2, type, time));
                }
            }
            time += BeatTime;
        }
    }

    public void Evaluate(Note note, Const.DecisionResult result) {
        switch (result) {
            case Const.DecisionResult.Perfect:
                note.Fired(fireWorkObj[note.Data.Type]);
                sound.PlaySE();
                //webCam.SaveImage();
                break;
            case Const.DecisionResult.Miss:
                note.Falled();
                break;
        }

        decesionResuleSubject.OnNext(new DecesionResultData(note.Data.Team, result));
        notes.Remove(note);
        Destroy(note.gameObject);
    }
}


public class NoteData {
    public Const.Team Team { get; private set; }
    public int Type { get; private set; }
    public float Time { get; private set; }

    public NoteData(Const.Team team, int type, float time) {
        Team = team;
        Type = type;
        Time = time;
    }
}

public class DecesionResultData {
    public Const.Team team;
    public Const.DecisionResult result;
    public DecesionResultData(Const.Team team, Const.DecisionResult result) {
        this.team = team;
        this.result = result;
    }
}