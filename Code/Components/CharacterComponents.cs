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
		[AssetReference(typeof(GameObject))]
		public AssetReference Prefab { get; set; }
		public LocalizedString Name { get; set; }
		public bool Invulnerable { get; set; }
		
		[HideInInspector] public Vector3 Position { get; set; }
		[HideInInspector] public Quaternion Rotation { get; set; }
		[HideInInspector] public bool IsDead { get; set; }
		[HideInInspector] public Vector3 Velocity { get; set; }
		[HideInInspector] public string CurrentTarget { get; set; }
		public CharacterCustomizationData VisualData { get; set; }
		
		// Don't forget to update CreateInstance if you add a new value!

		[HideInInspector] public string CharacterId { get; set; }

		public CharacterComponent CreateInstance()
		{
			return new()
			{
				Name = Name,
				VisualData = VisualData,
				Prefab = Prefab,
				Invulnerable = Invulnerable
			};
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
		public bool IsShopKeeper { get; set; }
		[DBReference(3)] // category 3 = shops
		public RPGObjectReference Shop { get; set; }
		[AssetReference(typeof(ScriptableObject))] // We don't have access to the specific type here
		public AssetReference Dialogue { get; set; }

		// TODO: Refactor to remove mutable data
		public NpcComponent CreateInstance()
		{
			return new()
			{
				DefaultRoutine = DefaultRoutine,
				CurrentRoutine = CurrentRoutine,
				Demeanor = Demeanor,
				Destination = Destination,
				IsShopKeeper = IsShopKeeper,
				Shop = Shop,
				Dialogue = Dialogue
			};
		}
	}
	
	/// <summary>
	/// Attributes are a character's inherent affinities with certain skills, and also covers health and mana
	/// </summary>
	public class CharacterAttributes: IRPGComponent
	{
		[JsonIgnore] public Action<Attribute, int> OnAttributeUpdated { get; set; }
		public int Strength { get; set; } = 10;
		public int Dexterity { get; set; } = 10;
		public int Agility { get; set; } = 10;
		public int Attunement { get; set; } = 10;
		public int Health { get; set; } = 10;
		public int MaxHealth { get; set; } = 10;
		public int Mana { get; set; } = 10;
		public int MaxMana { get; set; } = 10;
		public int MaxCarryWeight { get; set; } = 50;

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
				Attribute.MaxCarryWeight => MaxCarryWeight,
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
				case Attribute.MaxCarryWeight:
					MaxCarryWeight = newValue;
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

		public CharacterAttributes CreateInstance()
		{
			return new()
			{
				Strength = Strength,
				Dexterity = Dexterity,
				Agility = Agility,
				Attunement = Attunement,
				Health = Health,
				MaxHealth = MaxHealth,
				Mana = Mana,
				MaxMana = MaxMana
			};
		}
	}

	/// <summary>
	/// Skills are a character's abilities that are improved through practice
	/// </summary>
	public class CharacterSkills: IRPGComponent
	{
		public int Swordplay { get; set; } = 1;
		public int Archery { get; set; } = 1;
		public int Defense { get; set; } = 1;
		public int Elemental { get; set; } = 1;
		public int Restoration { get; set; } = 1;
		public int Athletics { get; set; } = 1;

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

		public CharacterSkills CreateInstance()
		{
			return new()
			{
				Swordplay = Swordplay,
				Archery = Archery,
				Defense = Defense,
				Elemental = Elemental,
				Restoration = Restoration,
				Athletics = Athletics
			};
		}
	}
}