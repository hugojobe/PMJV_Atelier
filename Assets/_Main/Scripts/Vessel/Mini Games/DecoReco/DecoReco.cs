using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecoReco : MiniGame
{
    public bool readyToConnect;
    public bool finished;

    public float timeToWait;
    public float currrentTime;

    public MovingConnectorUnspeed toRemove => GetComponentsInChildren<MovingConnectorUnspeed>().FirstOrDefault(c => c.type == MovingConnectorUnspeed.ConnectorType.starting);
    public MovingConnectorUnspeed toReplace => GetComponentsInChildren<MovingConnectorUnspeed>().FirstOrDefault(c => c.type == MovingConnectorUnspeed.ConnectorType.replace);

    private void Start() {
        base.Start();
    }

    private void Update() {
        if(finished)
            currrentTime += Time.deltaTime;
        else
            currrentTime = 0;

        if(currrentTime >= timeToWait)
            SolveProblem();
    }
}
