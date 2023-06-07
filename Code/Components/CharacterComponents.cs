using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using WolfRPG.Core;
using WolfRPG.Core.Localization;
using WolfRPG.Core.Quests;
using WolfRPG.Core.Statistics;
using Attribute = WolfRPG.Core.Statistics.Attribute;

// ReSharper disable Unity.RedundantAttributeOnTarget

namespace WolfRPG.Character
{
	public class CharacterComponent : IRPGComponent
	{
		public LocalizedString Name { get; set; }
		
		[HideInInspector] public Vector3 Position { get; set; }
		[HideInInspector] public Quaternion Rotation { get; set; }
		[HideInInspector] public bool IsDead { get; set; }		
		
		[HideInInspector] public Vector3 Velocity { get; set; }
		[HideInInspector] public string CurrentTarget { get; set; }
		[HideInInspector] public List<QuestProgress> QuestProgress { get; set; } = new();

		[JsonIgnore] public string CharacterId { get; set; } // TODO: This is probably not the right way about this
		[JsonIgnore] private List<QuestData> _quests;
		
		[JsonIgnore] public List<QuestData> Quests
		{
			get
			{
				if (_quests == null)
				{
					_quests = new List<QuestData>();
					foreach (var prog in QuestProgress)
					{
						_quests.Add(prog.GetQuest());
					}
				}
				return _quests;
			}
		}
	}
	
	public enum NPCDemeanor
	{
		Friendly, Neutral, Hostile
	}

	public enum NPCRoutine
	{
		Idle, Wandering, Combat
	}

	public class NpcComponent : IRPGComponent
	{
		public NPCRoutine DefaultRoutine { get; set; }
		[HideInInspector] public NPCRoutine CurrentRoutine { get; set; }
		public NPCDemeanor Demeanor { get; set; }
		[HideInInspector] public Vector3 Destination { get; set; }
	}
	
	/// <summary>
	/// Attributes are a character's inherent affinities with certain skills, and also covers health and mana
	/// </summary>
	public class CharacterAttributes: IRPGComponent
	{
		[JsonIgnore] public Action<Attribute, int> OnAttributeUpdated { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Agility { get; set; }
		public int Attunement { get; set; }
		public int Health { get; set; }
		public int MaxHealth { get; set; }
		public int Mana { get; set; }
		public int MaxMana { get; set; }

		public int GetAttribute(Attribute attribute)
		{
			return attribute switch
			{
				Attribute.DEFAULT => 0,
				Attribute.Strength => Strength,
				Attribute.Dexterity => Dexterity,
				Attribute.Agility => Agility,
				Attribute.Attunement => Attunement,
				Attribute.Health => Health,
				Attribute.MaxHealth => MaxHealth,
				Attribute.Mana => Mana,
				Attribute.MaxMana => MaxMana,
				Attribute.MAX => 0,
				_ => 0
			};
		}

		public void SetAttribute(Attribute attribute, int newValue)
		{
			OnAttributeUpdated?.Invoke(attribute, newValue);
			switch (attribute)
			{
				case Attribute.Strength:
					Strength = newValue;
					break;
				case Attribute.Dexterity:
					Dexterity = newValue;
					break;
				case Attribute.Agility:
					Agility = newValue;
					break;
				case Attribute.Attunement:
					Attunement = newValue;
					break;
				case Attribute.Health:
					Health = newValue;
					break;
				case Attribute.MaxHealth:
					MaxHealth = newValue;
					break;
				case Attribute.Mana:
					Mana = newValue;
					break;
				case Attribute.MaxMana:
					MaxMana = newValue;
					break;
			}
		}

		public void ModifyAttribute(Attribute attribute, int addition)
		{
			switch (attribute)
			{
				case Attribute.DEFAULT:
					break;
				case Attribute.Strength:
					Strength += addition;
					OnAttributeUpdated?.Invoke(attribute, Strength);
					break;
				case Attribute.Dexterity:
					Dexterity += addition;
					OnAttributeUpdated?.Invoke(attribute, Dexterity);
					break;
				case Attribute.Agility:
					Agility += addition;
					OnAttributeUpdated?.Invoke(attribute, Agility);
					break;
				case Attribute.Attunement:
					Attunement += addition;
					OnAttributeUpdated?.Invoke(attribute, Attunement);
					break;
				case Attribute.Health:
					Health += addition;
					OnAttributeUpdated?.Invoke(attribute, Health);
					break;
				case Attribute.MaxHealth:
					MaxHealth += addition;
					OnAttributeUpdated?.Invoke(attribute, MaxHealth);
					break;
				case Attribute.Mana:
					Mana += addition;
					OnAttributeUpdated?.Invoke(attribute, Mana);
					break;
				case Attribute.MaxMana:
					MaxMana += addition;
					OnAttributeUpdated?.Invoke(attribute, MaxMana);
					break;
				case Attribute.MAX:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null);
			}
		}
	}

	/// <summary>
	/// Skills are a character's abilities that are improved through practice
	/// </summary>
	public class CharacterSkills: IRPGComponent
	{
		public int Swordplay { get; set; }
		public int Archery { get; set; }
		public int Defense { get; set; }
		public int Elemental { get; set; }
		public int Restoration { get; set; }
		public int Athletics { get; set; }

		public int GetSkill(Skill skill)
		{
			return skill switch
			{
				Skill.DEFAULT => 0,
				Skill.Swordplay => Swordplay,
				Skill.Archery => Archery,
				Skill.Defense => Defense,
				Skill.Elemental => Elemental,
				Skill.Restoration => Restoration,
				Skill.Athletics => Athletics,
				Skill.MAX => 0,
				_ => 0
			};
		}
	}
}