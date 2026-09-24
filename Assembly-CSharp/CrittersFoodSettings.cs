using UnityEngine;

public class CrittersFoodSettings : CrittersActorSettings
{
	public float _maxFood;

	public float _currentFood;

	public float _startingSize;

	public float _currentSize;

	public Transform _food;

	public bool _disableWhenEmpty;

	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersFood crittersFood = (CrittersFood)parentActor;
		crittersFood.maxFood = _maxFood;
		crittersFood.currentFood = _currentFood;
		crittersFood.startingSize = _startingSize;
		crittersFood.currentSize = _currentSize;
		crittersFood.food = _food;
		crittersFood.disableWhenEmpty = _disableWhenEmpty;
		crittersFood.SpawnData(_maxFood, _currentFood, _startingSize);
	}
}
