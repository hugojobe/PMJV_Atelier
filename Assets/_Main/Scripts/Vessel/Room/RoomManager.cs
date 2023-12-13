using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomManager : MonoBehaviour, IPointerClickHandler
{
    VesselManager vesselManager => VesselManager.instance;

    public int roomIndex;
    public bool isZoomed;

    public SpriteRenderer problemMark;
    public bool hasProbemInside;

    public List<ProblemSpawner> problems;
    public ProblemSpawner lastProblem;

    public ProblemSpawner RequestProblem() {
        var availableProblems = problems.Where(p => p != lastProblem).ToList();

        var newProblem = problems[Random.Range(0, availableProblems.Count)];
        lastProblem = newProblem;

        vesselManager.OnProblemSolved += OnProblemSolved;
        hasProbemInside = true;

        return newProblem;
    }

    private void Update() {
        if(isZoomed && !VesselManager.instance.isInMinigame && Input.GetKeyDown(KeyCode.Escape)){
            ZoomOutRoom();
        }

        problemMark.gameObject.SetActive(hasProbemInside && !vesselManager.hasAnyRoomZoomed);
    }

    public void ZoomOutRoom() {
        VesselManager.instance.zoomedRoomManager = null;
        VesselManager.instance.animator.SetTrigger($"Room{roomIndex}");
    }

    public void OnProblemSolved() {
        vesselManager.OnProblemSolved -= OnProblemSolved;
        hasProbemInside = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(isZoomed || VesselManager.instance.hasAnyRoomZoomed) return;
        VesselManager.instance.animator.SetTrigger($"Room{roomIndex}");
        VesselManager.instance.zoomedRoomManager = this;
    }
}
