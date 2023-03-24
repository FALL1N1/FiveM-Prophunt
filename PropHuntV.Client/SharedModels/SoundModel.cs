using System.Collections.Generic;
using CitizenFX.Core;

namespace PropHuntV.SharedModels
{
	public class SoundModel
	{
		public readonly List<string> ShortSounds = new List<string> {
			"please_come_again.wav",
			"doh.wav",
			"woohoo.wav",
			"i_am_the_one_and_only.mp3",
			"ill_be_back.wav",
			"negative.wav",
			"boom_headshot.wav",
			"honk.wav",
			"what_is_wrong_with_you.wav", 
		};
		public readonly List<string> MediumSounds = new List<string> {
			"you_underestimate_the_power.wav",
			"go_away_or_i_shall.wav",
			"you_dont_know_the_power.wav",
			"this_is_sparta.wav",
			"snooping_as_usual.wav",
			"leeroy_jenkins.wav",
			"laundry_just_got_easier.wav", 
		};
		public readonly List<string> LongSounds = new List<string> {
			"bad_boys.mp3",
			"long_sound.wav",
			"aaah.mp3",
			"papapapapapa.wav", 
		}; 
	}

	public enum SoundReward
	{
		Small = 1,
		Medium = 3,
		Large = 7
	}
}
