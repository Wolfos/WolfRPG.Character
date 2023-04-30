using WolfRPG.Core.Statistics;

namespace WolfRPG.Character
{
	public interface ICharacterData
	{
		void ApplyStatusEffect(AttributeStatusEffect statusEffect);
		void ApplyStatusEffect(SkillStatusEffect statusEffect);
		void RemoveStatusEffect(string statusEffectName);
		void RemoveAllStatusEffects();
		void RemoveAllStatusEffects(Attribute attribute);
		void RemoveAllStatusEffects(Skill skill);
		bool HasStatusEffect(string statusEffectName);
		void Tick(float deltaTime);
		int GetSkillValue(Skill skill);
		int GetAttributeValue(Attribute attribute);
	}
}