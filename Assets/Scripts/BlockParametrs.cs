using System;

[Serializable]
public class BlockParametrs
{
	public EntityType dropEntity;

	public int life_index;

	public bool is_glass;

	public SoundManager.Sound FootEffect;

	public SoundManager.Sound HitEffect;

	public SoundManager.Sound DestroyEffect;
}
