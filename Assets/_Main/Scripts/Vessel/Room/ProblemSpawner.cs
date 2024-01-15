using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProblemSpawner : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string problemNameDebug;

    public GameObject problemMark;
    public GameObject problemMinigameUI;

    public bool hasProblemMark;

    private Material mat;
    public float outlineWidth = 1f;

    public MiniGame openMinigame;

    private void OnEnable() {
        problemMark.SetActive(hasProblemMark);
        VesselManager.instance.OnProblemSolved += OnProblemSolved;
    }

    private void OnDisable() {
        VesselManager.instance.OnProblemSolved -= OnProblemSolved;
    }

    private void Start() {
        SpriteRenderer uiImage = GetComponent<SpriteRenderer>();
        uiImage.material = new Material(uiImage.material);
        mat = uiImage.material;
        
        mat.SetFloat("_OutlinePixelWidth", 0);
        mat.SetFloat("_GlitchAmount", 0);

    }

    public void SpawnProblem() {
        //Debug.Log($"Problem {problemNameDebug} Spawned");
        //mat.SetFloat("_GlitchAmount", 1);

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

        mat.SetFloat("_GlitchAmount", 0);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(!hasProblemMark) return;

        openMinigame = Instantiate(problemMinigameUI).GetComponent<MiniGame>();
        VesselManager.instance.isInMinigame = true;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(hasProblemMark)
            mat.SetFloat("_OutlinePixelWidth", outlineWidth);
    }

    public void OnPointerExit(PointerEventData eventData) {
        mat.SetFloat("_OutlinePixelWidth", 0);
    }
}
