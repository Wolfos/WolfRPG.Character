using System;

namespace WolfRPG.Character
{
	public enum Gender
	{
		Female, Male
	}
	public enum CharacterCustomizationPart
	{
		Gender, Hair, BackAttachment, Head, Eyebrows, FacialHair, Torso, ArmUpperRight, ArmUpperLeft, ArmLowerRight, ArmLowerLeft, HandRight, HandLeft, Hips, LegRight, LegLeft, SkinColor
	}
	
	public class CharacterCustomizationData
	{
		public Gender Gender { get; set; }
		public int Hair { get; set; }
		public int BackAttachment { get; set; }
		public int Head { get; set; }
		public int Eyebrows { get; set; }
		public int FacialHair { get; set; }
		public int Torso { get; set; }
		public int ArmUpperRight { get; set; }
		public int ArmUpperLeft { get; set; }
		public int ArmLowerRight { get; set; }
		public int ArmLowerLeft { get; set; }
		public int HandRight { get; set; }
		public int HandLeft { get; set; }
		public int Hips { get; set; }
		public int LegRight { get; set; }
		public int LegLeft { get; set; }
		public int SkinColor { get; set; }
	}
}