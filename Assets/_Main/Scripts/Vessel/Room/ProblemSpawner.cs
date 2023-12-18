using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProblemSpawner : MonoBehaviour, IPointerClickHandler
{
    public string problemNameDebug;

    public GameObject problemMark;
    public GameObject problemMinigameUI;

    public bool hasProblemMark;

    public MiniGame openMinigame;

    private void OnEnable() {
        problemMark.SetActive(hasProblemMark);
        VesselManager.instance.OnProblemSolved += OnProblemSolved;
    }

    private void OnDisable() {
        VesselManager.instance.OnProblemSolved -= OnProblemSolved;
    }

    public void SpawnProblem() {
        //Debug.Log($"Problem {problemNameDebug} Spawned");
        hasProblemMark = true;
        problemMark.SetActive(true);
        VesselManager.instance.activeProblem = this;
        AudioManager.instance.PlaySoundEffect(FilePaths.resourcesSFX + "VesselProblem", 1.5f);
    }

    public void OnProblemSolved() {
        VesselNode currentNode = VesselManager.instance.currentState;
        if (currentNode is VesselProblem) {
            (currentNode as VesselProblem).OnProblemSolved();
        }

        VesselManager.instance.activeProblem = null;
        VesselManager.instance.isInMinigame = false;
        problemMark.SetActive(false);
        hasProblemMark = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(!hasProblemMark) return;

        openMinigame = Instantiate(problemMinigameUI).GetComponent<MiniGame>();
        VesselManager.instance.isInMinigame = true;
    }
}
