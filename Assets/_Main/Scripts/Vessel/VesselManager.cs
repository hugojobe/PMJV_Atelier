using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VesselManager : MonoBehaviour
{
    public VesselState currentStateDisplay;
    [HideInInspector] public VesselNode currentState;

    [HideInInspector] public static VesselManager instance;

    public List<RoomManager> roomManagers;

    public Animator animator;
    public RoomManager zoomedRoomManager;

    public UnityAction OnProblemSolved;

    public bool isInMinigame;

    public ProblemSpawner activeProblem;

    private Vector2 originalPos;
    public float shakeAmount;
    public bool hasAnyRoomZoomed;

    public bool canGenerateProblem;

    public List<GameObject> objectsToDisableInVesselMode;

    public Animator damageVolume;
    public float problemTimer;

    public TextMeshProUGUI vesselOxText;

    public bool canLoseGame;

    public Coroutine vesselModeCoroutine;
    public Slider timerSlider;

    private void Awake() {
        if(instance == null) instance = this;
        ChangeState(VesselState.VN);

        originalPos = transform.localPosition;

        timerSlider.gameObject.SetActive(false);
    }

    private void Update() {
        currentState?.OnStateUpdate();

        vesselOxText.text = $"<color=#2BA7EE>OX</color> {VNGameSave.activeFile.playerOx}/10";

        Shake();
        hasAnyRoomZoomed = roomManagers.Select(rm => rm.isZoomed).Any(value => value);
    }

    public void ChangeState(VesselState newState) {
        currentStateDisplay = newState;
        VesselNode newNode = null;
        switch(newState) {
            case VesselState.VN:
                newNode = new VesselVN();
                break;
            case VesselState.IDLE:
                newNode = new VesselIdle();
                break;
            case VesselState.PROBLEM:
                newNode = new VesselProblem();
                break;
        }

        SwitchState(newNode);
    }

    public void SwitchState(VesselNode newState) {
        currentState?.OnStateExit();
        currentState = newState;
        currentState?.OnStateEnter();
    }

    public void SetRoomIsZomed(int isZoomed) {
        int roomIndex = int.Parse(Regex.Match(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, @"\d+").Value) - 1;
        roomManagers[roomIndex].isZoomed = (isZoomed == 1)? true : false;
    }

    public void Shake() {
        Vector2 randomOffset = Random.insideUnitSphere * shakeAmount;
        transform.localPosition = originalPos + randomOffset;
    }

    public IEnumerator EnterVesselMode(string exitFilePath,float phaseDuration) {
        Debug.Log($"Entered vessel mode for {phaseDuration}");
        animator.SetTrigger("VN");
        canGenerateProblem = true;

        ChangeState(VesselState.IDLE);

        yield return new WaitForSeconds(phaseDuration);

        while(!CanReturnToVNState()) {
            yield return null;
        }

        canGenerateProblem =  false;

        yield return new WaitForSeconds(4f);

        ChangeState(VesselState.VN);
        animator.SetTrigger("VN");

        AudioManager.instance.StopTrack(0);

        //AudioManager.instance.PlaySoundEffect("VesselDown1", 2f);
        AudioManager.instance.PlaySoundEffect("VesselDown2", 2f);

        foreach(var obj in objectsToDisableInVesselMode) {
            obj.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(2f);

        ExtensionGeneral.LoadNewDialogueFile(new string[]{exitFilePath});
    }

    private bool CanReturnToVNState() {
        if(currentState is VesselProblem) return false;

        return true;
    }

    public void Reset() {
        canGenerateProblem = false;

        activeProblem?.openMinigame?.SolveProblem();

        ChangeState(VesselState.VN);

        foreach(var obj in objectsToDisableInVesselMode) {
            obj.SetActive(true);
        }
    }
}

[System.Serializable]
public class RoomZoomEvent {
    public int roomNumber;
}

public enum VesselState {
    VN,
    IDLE,
    PROBLEM,
}
