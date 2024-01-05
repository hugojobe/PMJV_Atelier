using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class VesselProblem : VesselNode {

    public float problemTimer => VesselManager.instance.problemTimer;
    public float currentProblemTimer;

    public override void OnStateEnter() {
        ChooseRoom().RequestProblem().SpawnProblem();
        VesselManager.instance.OnProblemSolved += OnProblemSolved;

        currentProblemTimer = 0;
    }

    public override void OnStateExit() {
        VesselManager.instance.OnProblemSolved -= OnProblemSolved;
    }

    public override void OnStateUpdate() {
        if(VesselManager.instance.canLoseGame)
        currentProblemTimer += Time.deltaTime;

        if(currentProblemTimer >= problemTimer && VesselManager.instance.canLoseGame) {
            currentProblemTimer = 0;
            ProblemNotSolved();
        }
    }

    public RoomManager ChooseRoom() {
        return VesselManager.instance.roomManagers[Random.Range(0, VesselManager.instance.roomManagers.Count)];
    }

    public void OnProblemSolved() {
        VesselManager.instance.ChangeState(VesselState.IDLE);
    }

    public static void ProblemNotSolved() {
        VesselManager.instance.damageVolume.SetTrigger("Damage");
        VesselManager.instance.vesselOxText.GetComponent<Animator>().SetTrigger("Hit");

        VesselManager.instance.StartCoroutine(ScreenShake());

        //Debug.Log($"{VNGameSave.activeFile.playerOx} - 1 = {VNGameSave.activeFile.playerOx-1}");
        VNGameSave.activeFile.playerOx -= 1;
        var activeUI = VesselManager.instance.activeProblem.openMinigame;
        if(activeUI != null)
            Object.Destroy(activeUI.gameObject);

        VesselManager.instance.OnProblemSolved.Invoke();
    }

    public static IEnumerator ScreenShake() {
        Vector2 originalPos = VesselManager.instance.transform.localPosition;
        float time = 0;
        float shakeDuration = 0.15f;

        while(time < shakeDuration) {
            time += Time.deltaTime;

            Vector2 randomOffset = Random.insideUnitSphere * 0.1f;
            VesselManager.instance.transform.localPosition = originalPos + randomOffset;

            yield return null;
        }

        VesselManager.instance.transform.localPosition = originalPos;
    }
}
