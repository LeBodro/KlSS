using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

public class AudioLibrary
{
	IDictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

	static AudioLibrary _instance;
	public static AudioLibrary Instance
	{
		get
		{
			if (_instance == null)
				_instance = new AudioLibrary();
			return _instance;
		}
	}

	AudioLibrary() { }

	public void Add(string name, SoundEffect sound)
	{
		sounds.Add(name, sound);
	}

	public void Play(string name)
	{
		sounds[name].Play();
	}
}