using System;
using System.Collections.Generic;
using UnityEngine;
using WolfRPG.Core;
using WolfRPG.Core.Statistics;
using Attribute = WolfRPG.Core.Statistics.Attribute;

namespace WolfRPG.Character
{
	public class CharacterData: ICharacterData
	{
		// Setters are used by JSON
		public CharacterComponent CharacterComponent { get; set; }
		public NpcComponent NpcComponent { get; set; }
		public CharacterAttributes Attributes { get; set; }
		public CharacterSkills Skills { get; set; }


		private readonly List<StatusEffect> _statusEffects;

		private float _currentTimeStamp;


		/// <summary>
		/// Initializes with default values
		/// </summary>
		public CharacterData()
		{
			Attributes = new();
			Skills = new();
			
			_statusEffects = new();
		}

		/// <summary>
		/// Initializes with given values
		/// </summary>
		public CharacterData(CharacterAttributes attributes, CharacterSkills skills, CharacterComponent characterComponent = null, NpcComponent npcComponent = null)
		{
			Attributes = attributes;
			Skills = skills;
			_statusEffects = new();

			CharacterComponent = characterComponent;
			NpcComponent = npcComponent;
		}

		public void ApplyStatusEffect(RPGObjectReference objectReference)
		{
			var effect = objectReference.GetComponent<StatusEffect>();
			if (effect == null)
			{
				Debug.LogError($"Object {objectReference.Guid} is not a valid status effect");
				return;
			}
			ApplyStatusEffect(effect);
		}

		public void ApplyStatusEffect(StatusEffect statusEffect)
		{
			if (statusEffect.Type != StatusEffectType.ApplyOnce)
			{
				_statusEffects.Add(statusEffect);
				statusEffect.AddedTimeStamp = _currentTimeStamp;
			}

			if (statusEffect.Type is StatusEffectType.ApplyForDuration or 
			    StatusEffectType.ApplyOnce)
			{
				ModifyAttributeLogic(statusEffect, true);
				statusEffect.ApplyTimeStamp = _currentTimeStamp;
			}
		}

		public void RemoveStatusEffect(int id)
		{
			// Iterate in reverse for safe and fast removal
			for (int i = _statusEffects.Count - 1; i >= 0; i--)
			{
				if (_statusEffects[i].Id == id)
				{
					_statusEffects.RemoveAt(i);
				}
			}
		}

		public void RemoveAllStatusEffects()
		{
			_statusEffects.Clear();
		}

		public bool HasStatusEffect(int id)
		{
			foreach (var effect in _statusEffects)
			{
				if (effect.Id == id) return true;
			}

			return false;
		}

		/// <summary>
		/// Handles removal of non-permanent status effects over time, and applies status effects that do damage / heal every second
		/// </summary>
		/// <param name="deltaTime"></param>
		public void Tick(float deltaTime)
		{
			_currentTimeStamp += deltaTime;
			
			// Iterate in reverse for safe removal
			for (int i = _statusEffects.Count - 1; i >= 0; i--)
			{
				var effect = _statusEffects[i];
				switch (effect.Type)
				{
					case StatusEffectType.ApplyForDuration when _currentTimeStamp - effect.AddedTimeStamp >= effect.Duration:
						_statusEffects.RemoveAt(i);
						continue;
					case StatusEffectType.ApplyForDuration:
					{
						// In case deltaTime is more than a second
						int calls = 1;
						if (deltaTime > 1)
						{
							calls = Mathf.CeilToInt(deltaTime);
						}

						for (int c = 0; c < calls; c++)
						{
							ModifyAttributeLogic(effect);
						}

						effect.ApplyTimeStamp = _currentTimeStamp;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Handles the case when "ApplyEverySecond" is set to true
		/// Timestamp check is skipped when called by ApplyStatusEffect
		/// </summary>
		private void ModifyAttributeLogic(StatusEffect effect, bool force = false)
		{
			if (effect.Modifiers == null) return;
			
			if (force ||
			    _currentTimeStamp - effect.ApplyTimeStamp >= 1)
			{
				foreach (var modifier in effect.Modifiers)
				{
					if (modifier.Type == StatusEffectModifierType.Attribute)
					{
						Attributes.ModifyAttribute(modifier.Attribute, modifier.Modifier);
					}
				}
				
			}
		}

		public int GetSkillValue(Skill skill)
		{
			var value = Skills.GetSkill(skill);
			
			foreach(var effect in _statusEffects)
			{
				foreach (var modifier in effect.Modifiers)
				{
					if (modifier.Type == StatusEffectModifierType.Skill && modifier.Skill == skill)
					{
						value += modifier.Modifier;
					}
				}
			}

			// Never go below 0
			return Math.Max(value, 0);
		}

		public int GetAttributeValue(Attribute attribute)
		{
			var value = Attributes.GetAttribute(attribute);

			foreach(var effect in _statusEffects)
			{
				if(effect.Type != StatusEffectType.ApplyUntilRemoved) continue;
				
				foreach (var modifier in effect.Modifiers)
				{
					if (modifier.Type == StatusEffectModifierType.Attribute && modifier.Attribute == attribute)
					{
						value += modifier.Modifier;
					}
				}
			}

			// Never go below 0
			return Math.Max(value, 0);
		}

		public void SetAttributeValue(Attribute attribute, int newValue) =>
			Attributes.SetAttribute(attribute, newValue);
	}
}