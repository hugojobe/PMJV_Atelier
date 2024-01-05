using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class Rythm : MiniGame
{
    private NoteSpawner noteSpawner => GetComponentInChildren<NoteSpawner>();
    private NoteDetector noteDetector => GetComponentInChildren<NoteDetector>();

    public List<float>[] patterns = new List<float>[]{
        new List<float>(){6, 0.5f, 1f},
        new List<float>(){2, 0.2f, 0.25f, 0.5f, 1f},
        new List<float>(){2, 1f, 1f, 1f, 1f},
        new List<float>(){2, 0.5f, 1f, 0.25f, 1f}
    };

    public List<float> selectedPattern;

    public TextMeshProUGUI loseNoteText;
    public bool canWin = true;

    private void Start() {
        base.Start();

        loseNoteText.gameObject.SetActive(false);

        selectedPattern = patterns[Random.Range(0, patterns.Length)];
        noteSpawner.StartSequence();
    }

    private void Update() {
        if(noteDetector.noteRemaining == false && noteSpawner.spawnFinished == true)
            Win();
    }

    public void Win() {
        canWin = false;
        noteDetector.noteButton.interactable = false;
        noteSpawner.canSpawn = false;

        StartCoroutine(WaitBeforeClose(SolveProblem));
    }

    public void Lose(int loseCode) {
        loseNoteText.gameObject.SetActive(true);
        noteDetector.noteButton.interactable = false;
        noteSpawner.canSpawn = false;
        FindObjectsOfType<Note>().ToList().ForEach(n => n.speed = 0);

        switch(loseCode) {
            case 0:
                loseNoteText.text = "TROP RAPIDE";
                break;
            case 1:
                loseNoteText.text = "TROP LENT";
                break;
        }

        StartCoroutine(WaitBeforeClose(VesselProblem.ProblemNotSolved));
    }

    private IEnumerator WaitBeforeClose(Action onReturn) {
        yield return new WaitForSeconds(1);

        onReturn.Invoke();
    }
}
