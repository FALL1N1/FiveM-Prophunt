using System.Collections.Generic;
using CitizenFX.Core;

namespace PropHuntV.SharedModels
{
	public class SoundModel
	{
		public readonly List<string> ShortSounds = new List<string> {
			"please_come_again",
			"doh",
			"woohoo",
			"i_am_the_one_and_only",
			"ill_be_back",
			"negative",
			"boom_headshot",
			"honk",
			"what_is_wrong_with_you", 
		};
		public readonly List<string> MediumSounds = new List<string> {
			"you_underestimate_the_power",
			"go_away_or_i_shall",
			"you_dont_know_the_power",
			"this_is_sparta",
			"snooping_as_usual",
			"leeroy_jenkins",
			"laundry_just_got_easier", 
		};
		public readonly List<string> LongSounds = new List<string> {
			"bad_boys",
			"long_sound",
			"aaah",
			"papapapapapa", 
		};

		public enum SoundReward
		{
			Short = 1,
			Medium = 3,
			Long = 7
		}

		public enum SoundType
		{
			Short = 1,
			Medium = 2,
			Long = 3
		}
	}
	 
}
