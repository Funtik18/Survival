using System.IO;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScreenshotHandler : MonoBehaviour
{
	[SerializeField] private Vector2Int size = new Vector2Int(512, 512); 
    [SerializeField] private Camera cam;

	private bool takeScreenshot = false;

	private void OnPostRender()
	{
		if(takeScreenshot)
		{
			RenderTexture texture = cam.targetTexture;

			Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
			Rect rect = new Rect(0, 0, texture2D.width, texture2D.height);

			texture2D.ReadPixels(rect, 0, 0);

			byte[] byteArray = texture2D.EncodeToPNG();

			string dir = Application.dataPath + "/Screenshots";

			if(!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}


			string fileName = "/Item_" + size + ".png";

			File.WriteAllBytes(dir + fileName, byteArray);

			Debug.LogError(Application.dataPath + "/Screenshots  Saved");

			RenderTexture.ReleaseTemporary(texture);
			cam.targetTexture = null;

#if UNITY_EDITOR
			AssetDatabase.Refresh();
#endif
			takeScreenshot = false;
		}
	}

	private void TakeScreenshot(int width, int height)
	{
		cam.targetTexture = RenderTexture.GetTemporary(width, height, 16);

		takeScreenshot = true;
	}

	[Button]
	private void TakeScreenshot()
	{
		TakeScreenshot(size.x, size.y);
	}
}
