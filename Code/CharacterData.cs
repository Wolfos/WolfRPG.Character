using System;
using System.Collections.Generic;
using WolfRPG.Core.Statistics;
using Attribute = WolfRPG.Core.Statistics.Attribute;

namespace WolfRPG.Character
{
	public class CharacterData: ICharacterData
	{
		private readonly CharacterAttributes _attributes;
		private readonly CharacterSkills _skills;
		
		// Status effects sorted by Attribute
		private readonly Dictionary<Attribute, List<AttributeStatusEffect>> _attributeStatusEffects;
		// Status effects sorted by skill
		private readonly Dictionary<Skill, List<SkillStatusEffect>> _skillStatusEffects;

		// Status effects sorted by name
		private readonly Dictionary<string, AttributeStatusEffect> _attributeStatusEffectNameMap;
		// Status effects sorted by name
		private readonly Dictionary<string, SkillStatusEffect> _skillStatusEffectNameMap;

		// All status effects that aren't permanent
		private readonly List<AttributeStatusEffect> _expiringAttributeStatusEffects;
		// All status effects that aren't permanent
		private readonly List<SkillStatusEffect> _expiringSkillStatusEffects;

		private float _currentTimeStamp;


		/// <summary>
		/// Initializes with default values
		/// </summary>
		public CharacterData()
		{
			_attributes = new();
			_skills = new();
			
			_attributeStatusEffects = new();
			_skillStatusEffects = new();
			CreateMappingDictionaries();

			_attributeStatusEffectNameMap = new();
			_skillStatusEffectNameMap = new();

			_expiringAttributeStatusEffects = new();
			_expiringSkillStatusEffects = new();
		}

		/// <summary>
		/// Initializes with given values
		/// </summary>
		public CharacterData(CharacterAttributes attributes, CharacterSkills skills)
		{
			_attributes = attributes;
			_skills = skills;

			_attributeStatusEffects = new();
			_skillStatusEffects = new();
			CreateMappingDictionaries();
			
			_attributeStatusEffectNameMap = new();
			_skillStatusEffectNameMap = new();
			
			_expiringAttributeStatusEffects = new();
			_expiringSkillStatusEffects = new();
		}

		private void CreateMappingDictionaries()
		{
			for (var i = 1; i < (int) Attribute.MAX; i++)
			{
				_attributeStatusEffects.Add((Attribute)i, new());
			}

			for (var i = 1; i < (int) Skill.MAX; i++)
			{
				_skillStatusEffects.Add((Skill)i, new());
			}
		}

		public void ApplyStatusEffect(AttributeStatusEffect statusEffect)
		{
			_attributeStatusEffects[statusEffect.Attribute].Add(statusEffect);
			_attributeStatusEffectNameMap.Add(statusEffect.StatusEffectName, statusEffect);

			if (statusEffect.Permanent == false)
			{
				_expiringAttributeStatusEffects.Add(statusEffect);
			}

			statusEffect.AddedTimeStamp = _currentTimeStamp;
		}

		public void ApplyStatusEffect(SkillStatusEffect statusEffect)
		{
			_skillStatusEffects[statusEffect.Skill].Add(statusEffect);
			_skillStatusEffectNameMap.Add(statusEffect.StatusEffectName, statusEffect);

			if (statusEffect.Permanent == false)
			{
				_expiringSkillStatusEffects.Add(statusEffect);
			}
			
			statusEffect.AddedTimeStamp = _currentTimeStamp;
		}

		public void RemoveStatusEffect(string statusEffectName)
		{
			if (_attributeStatusEffectNameMap.ContainsKey(statusEffectName))
			{
				var effect = _attributeStatusEffectNameMap[statusEffectName];
				
				_attributeStatusEffects[effect.Attribute].Remove(effect);
				_attributeStatusEffectNameMap.Remove(statusEffectName);
				_expiringAttributeStatusEffects.Remove(effect);
			}
			
			if (_skillStatusEffectNameMap.ContainsKey(statusEffectName))
			{
				var effect = _skillStatusEffectNameMap[statusEffectName];
				
				_skillStatusEffects[effect.Skill].Remove(effect);
				_skillStatusEffectNameMap.Remove(statusEffectName);
				_expiringSkillStatusEffects.Remove(effect);
			}
		}

		public void RemoveAllStatusEffects()
		{
			for (var i = 1; i < (int) Attribute.MAX; i++)
			{
				_attributeStatusEffects[(Attribute)i].Clear();
			}
			_attributeStatusEffectNameMap.Clear();
			
			for (var i = 1; i < (int) Skill.MAX; i++)
			{
				_skillStatusEffects[(Skill)i].Clear();
			}
			_skillStatusEffectNameMap.Clear();
		}

		public void RemoveAllStatusEffects(Attribute attribute)
		{
			foreach (var effect in _attributeStatusEffects[attribute])
			{
				_attributeStatusEffectNameMap.Remove(effect.StatusEffectName);
			}
			_attributeStatusEffects[attribute].Clear();
		}

		public void RemoveAllStatusEffects(Skill skill)
		{
			foreach (var effect in _skillStatusEffects[skill])
			{
				_skillStatusEffectNameMap.Remove(effect.StatusEffectName);
			}
			_skillStatusEffects[skill].Clear();
		}

		public bool HasStatusEffect(string statusEffectName)
		{
			return _attributeStatusEffectNameMap.ContainsKey(statusEffectName) ||
			       _skillStatusEffectNameMap.ContainsKey(statusEffectName);
		}

		/// <summary>
		/// Handles removal of non-permanent status effects over time
		/// </summary>
		/// <param name="deltaTime"></param>
		public void Tick(float deltaTime)
		{
			_currentTimeStamp += deltaTime;

			List<string> _toRemove = new();

			foreach (var effect in _expiringAttributeStatusEffects)
			{
				if (_currentTimeStamp - effect.AddedTimeStamp > effect.Duration)
				{
					_toRemove.Add(effect.StatusEffectName);
				}
			}
			
			foreach (var effect in _expiringSkillStatusEffects)
			{
				if (_currentTimeStamp - effect.AddedTimeStamp > effect.Duration)
				{
					_toRemove.Add(effect.StatusEffectName);
				}
			}

			foreach (var name in _toRemove)
			{
				RemoveStatusEffect(name);
			}
		}

		public int GetSkillValue(Skill skill)
		{
			var value = _skills.GetSkill(skill);
			
			foreach(var effect in _skillStatusEffects[skill])
			{
				value += effect.Effect;
			}

			// Never go below 0
			return Math.Max(value, 0);
		}

		public int GetAttributeValue(Attribute attribute)
		{
			var value = _attributes.GetAttribute(attribute);

			foreach (var effect in _attributeStatusEffects[attribute])
			{
				value += effect.Effect;
			}

			// Never go below 0
			return Math.Max(value, 0);
		}
	}
}