using NUnit.Framework;
using WolfRPG.Core.Statistics;

namespace WolfRPG.Character.Tests
{
	public class CharacterDataTests
	{
		[Test]
		public void ApplyStatusEffect_Attribute_AddsStatusEffect()
		{
			var target = new CharacterData();
			var statusEffect = new AttributeStatusEffect
			{
				StatusEffectName = "Bob",
				Attribute = Attribute.Agility,
				Duration = 1
			};
			
			target.ApplyStatusEffect(statusEffect);
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			
			Assert.IsTrue(actual);
		}
		
		[Test]
		public void ApplyStatusEffect_Attribute_DoesNotAddWhenDurationIsZero()
		{
			var target = new CharacterData();
			var statusEffect = new AttributeStatusEffect
			{
				StatusEffectName = "Bob",
				Attribute = Attribute.Agility,
				Duration = 0
			};
			
			target.ApplyStatusEffect(statusEffect);
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void ApplyStatusEffect_Skill_AddsStatusEffect()
		{
			var target = new CharacterData();
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Skill = Skill.Archery
			};
			
			target.ApplyStatusEffect(statusEffect);
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			
			Assert.IsTrue(actual);
		}
		
		[Test]
		public void RemoveStatusEffectSkill_RemovesStatusEffect()
		{
			var target = new CharacterData();
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Skill = Skill.Defense
			};
			
			target.ApplyStatusEffect(statusEffect);
			target.RemoveStatusEffect(statusEffect.StatusEffectName);
			
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);

			Assert.IsFalse(actual);
		}

		[Test]
		public void RemoveAllStatusEffects_RemovesAllStatusEffects()
		{
			var target = new CharacterData();
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Skill = Skill.Archery,
				Duration = 1
			};
			var statusEffect2 = new AttributeStatusEffect
			{
				StatusEffectName = "Bob",
				Attribute = Attribute.Agility,
				Duration = 1
			};
			
			target.ApplyStatusEffect(statusEffect);
			target.ApplyStatusEffect(statusEffect2);
			
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsTrue(actual);
			actual = target.HasStatusEffect(statusEffect2.StatusEffectName);
			Assert.IsTrue(actual);
			
			target.RemoveAllStatusEffects();
			
			actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsFalse(actual);
			actual = target.HasStatusEffect(statusEffect2.StatusEffectName);
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void RemoveAllStatusEffects_RemovesAllStatusEffectsForAttribute()
		{
			var target = new CharacterData();
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Skill = Skill.Archery,
				Duration = 1
			};
			var statusEffect2 = new AttributeStatusEffect
			{
				StatusEffectName = "Bob",
				Attribute = Attribute.Agility,
				Duration = 1
			};
			
			target.ApplyStatusEffect(statusEffect);
			target.ApplyStatusEffect(statusEffect2);
			
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsTrue(actual);
			actual = target.HasStatusEffect(statusEffect2.StatusEffectName);
			Assert.IsTrue(actual);
			
			target.RemoveAllStatusEffects(Attribute.Agility);
			
			actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsTrue(actual);
			actual = target.HasStatusEffect(statusEffect2.StatusEffectName);
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void RemoveAllStatusEffects_RemovesAllStatusEffectsForSkill()
		{
			var target = new CharacterData();
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Skill = Skill.Archery,
				Duration = 1
			};
			var statusEffect2 = new AttributeStatusEffect
			{
				StatusEffectName = "Bob",
				Attribute = Attribute.Agility,
				Duration = 1
			};
			
			target.ApplyStatusEffect(statusEffect);
			target.ApplyStatusEffect(statusEffect2);
			
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsTrue(actual);
			actual = target.HasStatusEffect(statusEffect2.StatusEffectName);
			Assert.IsTrue(actual);
			
			target.RemoveAllStatusEffects(Skill.Archery);
			
			actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsFalse(actual);
			actual = target.HasStatusEffect(statusEffect2.StatusEffectName);
			Assert.IsTrue(actual);
		}

		[Test]
		public void Tick_DoesNotThrowException()
		{
			var target = new CharacterData();
			target.Tick(0);
		}

		[Test]
		public void Tick_RemovesExpiredStatusEffect()
		{
			var target = new CharacterData();
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Duration = 10,
				Skill = Skill.Elemental
			};
			target.ApplyStatusEffect(statusEffect);
			
			target.Tick(5);
			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsTrue(actual);
			
			target.Tick(5.01f);

			actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void Tick_IgnoresDurationWhenPermanent()
		{
			var target = new CharacterData();
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Duration = 0,
				Permanent = true,
				Skill = Skill.Defense
			};
			target.ApplyStatusEffect(statusEffect);

			target.Tick(10.01f);

			var actual = target.HasStatusEffect(statusEffect.StatusEffectName);
			Assert.IsTrue(actual);
		}

		[Test]
		public void Tick_ApplyEverySecond_AppliesOverTime()
		{
			var attributes = new CharacterAttributes
			{
				Health = 0
			};
			var target = new CharacterData(attributes, new ());
			var statusEffect = new AttributeStatusEffect()
			{
				StatusEffectName = "Healing Potion",
				Duration = 10,
				Effect = 1,
				Attribute = Attribute.Health,
				ApplyEverySecond = true
			};
			target.ApplyStatusEffect(statusEffect);


			int actual;
			for (var expected = 1; expected < 10; expected++)
			{
				actual = target.GetAttributeValue(Attribute.Health);
				Assert.AreEqual(expected, actual);
				
				target.Tick(1);
			}

			// Call tick a bunch more times to assert it doesn't go over the expected value
			for (var i = 0; i < 10; i++)
			{
				target.Tick(1);
			}
			
			actual = target.GetAttributeValue(Attribute.Health);
			Assert.AreEqual(10, actual);
		}
		
		[Test]
		public void Tick_ApplyEverySecond_CorrectWhenDeltaTimeIsOverOne()
		{
			var attributes = new CharacterAttributes
			{
				Health = 0
			};
			var target = new CharacterData(attributes, new ());
			var statusEffect = new AttributeStatusEffect()
			{
				StatusEffectName = "Healing Potion",
				Duration = 10,
				Effect = 1,
				Attribute = Attribute.Health,
				ApplyEverySecond = true
			};
			target.ApplyStatusEffect(statusEffect);


			int actual;
			for (var expected = 1; expected < 5; expected++)
			{
				actual = target.GetAttributeValue(Attribute.Health);
				Assert.AreEqual(expected * 2 - 1, actual);
				
				target.Tick(2);
			}

			// Call tick a bunch more times to assert it doesn't go over the expected value
			for (var i = 0; i < 10; i++)
			{
				target.Tick(1);
			}
			
			actual = target.GetAttributeValue(Attribute.Health);
			Assert.AreEqual(10, actual);
		}

		[Test]
		public void TestCase_EffectThatOnlyAppliesOnce()
		{
			var attributes = new CharacterAttributes
			{
				Health = 0
			};
			var target = new CharacterData(attributes, new ());
			var statusEffect = new AttributeStatusEffect()
			{
				StatusEffectName = "Healing Potion",
				Duration = 0,
				Effect = 1,
				Attribute = Attribute.Health,
				ApplyEverySecond = true
			};
			target.ApplyStatusEffect(statusEffect);
			
			int actual = target.GetAttributeValue(Attribute.Health);
			Assert.AreEqual(1, actual);

			// Call tick a bunch more times to assert it doesn't go over the expected value
			for (var i = 0; i < 10; i++)
			{
				target.Tick(1);
			}
			
			actual = target.GetAttributeValue(Attribute.Health);
			Assert.AreEqual(1, actual);
		}
		
		[Test]
		public void GetSkillValue_ReturnsSkillValue()
		{
			var attributes = new CharacterAttributes();
			var skills = new CharacterSkills()
			{
				Archery = 10
			};
			var target = new CharacterData(attributes,skills);

			var actual = target.GetSkillValue(Skill.Archery);
			Assert.AreEqual(10, actual);
		}

		[Test]
		public void GetSkillValue_IncludesStatusEffect()
		{
			var attributes = new CharacterAttributes();
			var skills = new CharacterSkills()
			{
				Archery = 10
			};
			var target = new CharacterData(attributes,skills);
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Permanent = true,
				Skill = Skill.Archery,
				Effect = 10
			};
			target.ApplyStatusEffect(statusEffect);

			var actual = target.GetSkillValue(Skill.Archery);
			Assert.AreEqual(20, actual);
		}
		
		[Test]
		public void GetSkillValue_DoesNotGoBelowZero()
		{
			var attributes = new CharacterAttributes();
			var skills = new CharacterSkills()
			{
				Archery = 10
			};
			var target = new CharacterData(attributes,skills);
			var statusEffect = new SkillStatusEffect
			{
				StatusEffectName = "Henk",
				Permanent = true,
				Skill = Skill.Archery,
				Effect = -20
			};
			target.ApplyStatusEffect(statusEffect);

			var actual = target.GetSkillValue(Skill.Archery);
			Assert.AreEqual(0, actual);
		}
		[Test]
		public void GetAttributeValue_ReturnsAttributeValue()
		{
			var attributes = new CharacterAttributes
			{
				Agility = 10
			};
			var skills = new CharacterSkills();
			var target = new CharacterData(attributes,skills);

			var actual = target.GetAttributeValue(Attribute.Agility);
			Assert.AreEqual(10, actual);
		}

		[Test]
		public void GetAttributeValue_IncludesStatusEffect()
		{
			var attributes = new CharacterAttributes
			{
				Agility = 10
			};
			var skills = new CharacterSkills();
			var target = new CharacterData(attributes,skills);
			var statusEffect = new AttributeStatusEffect
			{
				StatusEffectName = "Bob",
				Permanent = true,
				Attribute = Attribute.Agility,
				Effect = 10,
				Duration = 10
			};
			target.ApplyStatusEffect(statusEffect);

			var actual = target.GetAttributeValue(Attribute.Agility);
			Assert.AreEqual(20, actual);
		}
		
		[Test]
		public void GetAttributeValue_DoesNotGoBelowZero()
		{
			var attributes = new CharacterAttributes
			{
				Agility = 10
			};
			var skills = new CharacterSkills();
			var target = new CharacterData(attributes,skills);
			var statusEffect = new AttributeStatusEffect
			{
				StatusEffectName = "Bob",
				Permanent = true,
				Attribute = Attribute.Agility,
				Effect = -20,
				Duration = 10
			};
			target.ApplyStatusEffect(statusEffect);

			var actual = target.GetAttributeValue(Attribute.Agility);
			Assert.AreEqual(0, actual);
		}
		
	}
}