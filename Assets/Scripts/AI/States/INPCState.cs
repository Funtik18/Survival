using System.Collections;

public interface INPCState
{
    IEnumerator DoState();
    void StopState();
    void DrawGizmos();
}