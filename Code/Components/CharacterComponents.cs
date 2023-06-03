using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using WolfRPG.Core;
using WolfRPG.Core.Quests;
using WolfRPG.Core.Statistics;
using Attribute = WolfRPG.Core.Statistics.Attribute;

// ReSharper disable Unity.RedundantAttributeOnTarget

namespace WolfRPG.Character
{
	public class CharacterComponent : IRPGComponent
	{
		[JsonIgnore] public string CharacterId { get; set; }
		[HideInInspector] public Vector3 Position { get; set; }
		[HideInInspector] public Quaternion Rotation { get; set; }
		[HideInInspector] public bool IsDead { get; set; }
		[HideInInspector] public float Health { get; set; }
		public float MaxHealth { get; set; }
		
		
		[HideInInspector] public Vector3 Velocity { get; set; }
		[HideInInspector] public string CurrentTarget { get; set; }
		[HideInInspector] public List<QuestProgress> QuestProgress { get; set; } = new();
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

		public void ModifyAttribute(Attribute attribute, int addition)
		{
			switch (attribute)
			{
				case Attribute.DEFAULT:
					break;
				case Attribute.Strength:
					Strength += addition;
					break;
				case Attribute.Dexterity:
					Dexterity += addition;
					break;
				case Attribute.Agility:
					Agility += addition;
					break;
				case Attribute.Attunement:
					Attunement += addition;
					break;
				case Attribute.Health:
					Health += addition;
					break;
				case Attribute.MaxHealth:
					MaxHealth += addition;
					break;
				case Attribute.Mana:
					Mana += addition;
					break;
				case Attribute.MaxMana:
					MaxMana += addition;
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