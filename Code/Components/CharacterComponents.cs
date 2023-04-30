using System;
using WolfRPG.Core;
using WolfRPG.Core.Statistics;
using Attribute = WolfRPG.Core.Statistics.Attribute;

namespace WolfRPG.Character
{
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