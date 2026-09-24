using UnityEngine;

public class KIDUI_TooYoungToPlay : MonoBehaviour
{
	public void ShowTooYoungToPlayScreen()
	{
		gameObject.SetActive(value: true);
	}

	public void OnQuitPressed()
	{
		Application.Quit();
	}
}
