using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    public static bool IsPlatformPC 
    {
        get
        {
            return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
        }
    }
    public static bool IsPlatformMobile 
    {
        get
        {
#if UNITY_EDITOR
            return UnityEditor.EditorApplication.isRemoteConnected;
#else
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
#endif
        }
    }
}