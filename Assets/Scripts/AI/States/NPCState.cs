using UnityEngine;

[System.Serializable]
public class NPCState
{
    public string stateName = "New State";
    public string animationBool = string.Empty;
}
[System.Serializable]
public class NPCIdleState : NPCState
{
    [Min(0)]
    public float idleDuration = 0;
    public int stateWeight = 20;
}
[System.Serializable]
public class NPCMovementState : NPCState
{
    public float moveSpeed = 3f;
    public float turnSpeed = 120f;
}