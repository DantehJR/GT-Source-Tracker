using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class MaterialUtils
{
	public static string GetTrimmedMaterialName(Material material)
	{
		return material.name.Replace(" (Instance)", "").Trim();
	}

	public static void SwapMaterial(MeshAndMaterials meshAndMaterial, bool isOnToOff)
	{
		List<Material> value;
		using (ListPool<Material>.Get(out value))
		{
			meshAndMaterial.meshRenderer.GetSharedMaterials(value);
			for (int i = 0; i < value.Count; i++)
			{
				string trimmedMaterialName = GetTrimmedMaterialName(value[i]);
				string text;
				if (isOnToOff)
				{
					text = ((meshAndMaterial.onMaterial != null) ? GetTrimmedMaterialName(meshAndMaterial.onMaterial) : null);
				}
				else
				{
					text = ((meshAndMaterial.offMaterial != null) ? GetTrimmedMaterialName(meshAndMaterial.offMaterial) : null);
				}
				if (text != null && trimmedMaterialName == text)
				{
					value[i] = (isOnToOff ? meshAndMaterial.offMaterial : meshAndMaterial.onMaterial);
				}
			}
			meshAndMaterial.meshRenderer.SetSharedMaterials(value);
		}
	}
}
