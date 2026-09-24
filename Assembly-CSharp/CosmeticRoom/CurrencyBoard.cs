using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace CosmeticRoom;

public class CurrencyBoard : MonoBehaviour
{
	public TMP_Text dailyRocksTextTMP;

	public TMP_Text currencyBoardTextTMP;

	public void OnEnable()
	{
		CosmeticsController.instance.AddCurrencyBoard(this);
	}

	public void OnDisable()
	{
		CosmeticsController.instance.RemoveCurrencyBoard(this);
	}

	public void UpdateCurrencyBoard(bool checkedDaily, bool gotDaily, int currencyBalance, int secTilTomorrow)
	{
		if (dailyRocksTextTMP.IsNotNull())
		{
			TMP_Text tMP_Text = dailyRocksTextTMP;
			string text;
			if (checkedDaily)
			{
				text = (gotDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS...");
			}
			else
			{
				text = "CHECKING DAILY ROCKS...";
			}
			tMP_Text.text = text;
		}
		if (currencyBoardTextTMP.IsNotNull())
		{
			currencyBoardTextTMP.text = currencyBalance + "\n\n" + secTilTomorrow / 3600 + " HR, " + secTilTomorrow % 3600 / 60 + "MIN";
		}
	}
}
