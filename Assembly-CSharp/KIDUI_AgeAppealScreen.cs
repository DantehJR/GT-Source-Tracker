using UnityEngine;

public class KIDUI_AgeAppealScreen : MonoBehaviour
{
	public void OnDisable()
	{
		KIDAudioManager.Instance?.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	public void ShowRestrictedAccessScreen()
	{
		gameObject.SetActive(value: true);
	}

	public void OnChangeAgePressed()
	{
		gameObject.SetActive(value: false);
	}
}
