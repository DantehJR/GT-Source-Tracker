public class CrittersLoudNoiseSettings : CrittersActorSettings
{
	public float _soundVolume;

	public float _soundDuration;

	public bool _soundEnabled;

	public bool _disableWhenSoundDisabled;

	public float _volumeFearAttractionMultiplier = 1f;

	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)parentActor;
		crittersLoudNoise.soundVolume = _soundVolume;
		crittersLoudNoise.soundDuration = _soundDuration;
		crittersLoudNoise.soundEnabled = _soundEnabled;
		crittersLoudNoise.disableWhenSoundDisabled = _disableWhenSoundDisabled;
		crittersLoudNoise.volumeFearAttractionMultiplier = _volumeFearAttractionMultiplier;
	}
}
