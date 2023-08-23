using NUnit.Framework;
using WolfRPG.Core.Statistics;

namespace WolfRPG.Character.Tests
{
	public class CharacterDataTests
	{
		[Test]
		public void ApplyStatusEffect_AddsStatusEffect()
		{
			var target = new CharacterData();
			var statusEffect = new StatusEffect()
			{
				Id = 1337,
				Duration = 1
			};
			
			target.ApplyStatusEffect(statusEffect);
			var actual = target.HasStatusEffect(statusEffect.Id);
			
			Assert.IsTrue(actual);
		}
		
		[Test]
		public void ApplyStatusEffect_DoesNotAddWhenApplyOnce()
		{
			var target = new CharacterData();
			var statusEffect = new StatusEffect()
			{
				Id = 1337,
				Duration = 0,
				Type = StatusEffectType.ApplyOnce
			};
			
			target.ApplyStatusEffect(statusEffect);
			var actual = target.HasStatusEffect(statusEffect.Id);
			
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void RemoveStatusEffect_RemovesStatusEffect()
		{
			var target = new CharacterData();
			var statusEffect = new StatusEffect
			{
				Id = 1337
			};
			
			target.ApplyStatusEffect(statusEffect);
			target.RemoveStatusEffect(statusEffect.Id);
			
			var actual = target.HasStatusEffect(statusEffect.Id);

			Assert.IsFalse(actual);
		}

		[Test]
		public void RemoveAllStatusEffects_RemovesAllStatusEffects()
		{
			var target = new CharacterData();
			var statusEffect = new StatusEffect()
			{
				Id = 1337,
				Duration = 1
			};
			var statusEffect2 = new StatusEffect()
			{
				Id = 1338,
				Duration = 1
			};
			
			target.ApplyStatusEffect(statusEffect);
			target.ApplyStatusEffect(statusEffect2);
			
			var actual = target.HasStatusEffect(statusEffect.Id);
			Assert.IsTrue(actual);
			actual = target.HasStatusEffect(statusEffect2.Id);
			Assert.IsTrue(actual);
			
			target.RemoveAllStatusEffects();
			
			actual = target.HasStatusEffect(statusEffect.Id);
			Assert.IsFalse(actual);
			actual = target.HasStatusEffect(statusEffect2.Id);
			Assert.IsFalse(actual);
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
			var statusEffect = new StatusEffect()
			{
				Id = 1337,
				Duration = 10,
				Type = StatusEffectType.ApplyForDuration
			};
			target.ApplyStatusEffect(statusEffect);
			
			target.Tick(5);
			var actual = target.HasStatusEffect(statusEffect.Id);
			Assert.IsTrue(actual);
			
			target.Tick(5.01f);

			actual = target.HasStatusEffect(statusEffect.Id);
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void Tick_IgnoresDurationWhenPermanent()
		{
			var target = new CharacterData();
			var statusEffect = new StatusEffect
			{
				Id = 12,
				Duration = 0,
				Type = StatusEffectType.ApplyUntilRemoved,
			};
			target.ApplyStatusEffect(statusEffect);

			target.Tick(10.01f);

			var actual = target.HasStatusEffect(statusEffect.Id);
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
			var statusEffect = new StatusEffect()
			{
				Id = 2355325,
				Duration = 10,
				Modifiers = new []{new StatusEffectModifier
				{
					Attribute = Attribute.Health,
					Modifier = 1
				}},
				Type = StatusEffectType.ApplyForDuration
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
			var statusEffect = new StatusEffect()
			{
				Id = 2355325,
				Duration = 10,
				Modifiers = new []{new StatusEffectModifier
				{
					Type = StatusEffectModifierType.Attribute,
					Attribute = Attribute.Health,
					Modifier = 1
				}},
				Type = StatusEffectType.ApplyForDuration
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
			var statusEffect = new StatusEffect()
			{
				Id = 2355325,
				Duration = 10,
				Modifiers = new []{new StatusEffectModifier
				{
					Type = StatusEffectModifierType.Attribute,
					Attribute = Attribute.Health,
					Modifier = 1
				}},
				Type = StatusEffectType.ApplyOnce
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
			var statusEffect = new StatusEffect()
			{
				Id = 256,
				Duration = 10,
				Modifiers = new []{new StatusEffectModifier
				{
					Type = StatusEffectModifierType.Skill,
					Skill = Skill.Archery,
					Modifier = 10
				}},
				Type = StatusEffectType.ApplyUntilRemoved
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
			var statusEffect = new StatusEffect
			{
				Id = 257,
				Duration = 10,
				Modifiers = new []{new StatusEffectModifier
				{
					Type = StatusEffectModifierType.Skill,
					Skill = Skill.Archery,
					Modifier = -20
				}},
				Type = StatusEffectType.ApplyUntilRemoved
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
			
			var statusEffect = new StatusEffect()
			{
				Id = 5634654,
				Duration = 10,
				Modifiers = new []{new StatusEffectModifier
				{
					Type = StatusEffectModifierType.Attribute,
					Attribute = Attribute.Agility,
					Modifier = 10
				}},
				Type = StatusEffectType.ApplyOnce
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
			var statusEffect = new StatusEffect()
			{
				Id = 5634654,
				Duration = 10,
				Modifiers = new []{new StatusEffectModifier
				{
					Type = StatusEffectModifierType.Attribute,
					Attribute = Attribute.Agility,
					Modifier = -20
				}},
				Type = StatusEffectType.ApplyOnce
			};
			target.ApplyStatusEffect(statusEffect);

			var actual = target.GetAttributeValue(Attribute.Agility);
			Assert.AreEqual(0, actual);
		}
		
	}
}