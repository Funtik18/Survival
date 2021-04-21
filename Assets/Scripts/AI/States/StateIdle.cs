using System.Collections;

using UnityEngine.Events;

public class StateIdle : INPCState
{
    public UnityAction onIdle;

    public IEnumerator DoState()
    {
        yield return null;

        onIdle?.Invoke();
    }

    public void StopState() { }

    public void DrawGizmos() { }
}
