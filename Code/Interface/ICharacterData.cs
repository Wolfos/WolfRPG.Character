using WolfRPG.Core.Statistics;

namespace WolfRPG.Character
{
	public interface ICharacterData
	{
		void ApplyStatusEffect(StatusEffect statusEffect);
		void RemoveStatusEffect(int id);
		void RemoveAllStatusEffects();
		bool HasStatusEffect(int id);
		void Tick(float deltaTime);
		int GetSkillValue(Skill skill);
		int GetAttributeValue(Attribute attribute);
	}
}