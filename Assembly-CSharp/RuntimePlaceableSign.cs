using TMPro;
using UnityEngine;

[SelectionBase]
public class RuntimePlaceableSign : MonoBehaviour
{
	[SerializeField]
	private TMP_Text textField;

	private SignPlacementManager.SignData signData;

	public TMP_Text TextField => textField;

	public SignPlacementManager.SignData SignData
	{
		get
		{
			signData.position = transform.position;
			signData.rotation = transform.rotation;
			if ((bool)textField)
			{
				signData.text = textField.text;
			}
			return signData;
		}
		set
		{
			signData = value;
			Configure();
		}
	}

	public void Init(SignPlacementManager.SignData data)
	{
		signData = data;
		Configure();
	}

	private void Configure()
	{
		transform.SetPositionAndRotation(signData.position, signData.rotation);
		if ((bool)textField)
		{
			textField.text = signData.text;
		}
	}
}
