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
            return UnityEditor.EditorApplication.isRemoteConnected || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }
}