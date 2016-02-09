namespace GXPEngine.GXPEngine
{
	/// <summary>
	/// This class represents a sound channel on the soundcard.
	/// </summary>
	public class SoundChannel
	{
		private int _id = 0;
		
		public SoundChannel( int id )
		{
			_id = id;
		}
		
		/// <summary>
		/// Gets or sets the channel frequency.
		/// </summary>
		/// <value>
		/// The frequency. Defaults to the sound frequency. (Usually 44100Hz)
		/// </value>
		public float Frequency 
		{
			get 
			{
				float frequency;
				FMOD.FMOD.Channel_GetFrequency( _id, out frequency );
				return frequency;
			}
			set
			{
				FMOD.FMOD.Channel_SetFrequency( _id, value );
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GXPEngine.SoundChannel"/> is mute.
		/// </summary>
		/// <value>
		/// <c>true</c> if you want to mute the sound
		/// </value>
		public bool Mute   
		{
			get 
			{
				bool mute;
				FMOD.FMOD.Channel_GetMute( _id, out mute );
				return mute;
			}
			set
			{
				FMOD.FMOD.Channel_SetMute( _id, value );
			}
		}

		/// <summary>
		/// Gets or sets the pan. Value should be in range -1..0..1, for left..center..right
		/// </summary>
		public float Pan   
		{
			get 
			{
				float pan;
				FMOD.FMOD.Channel_GetPan( _id, out pan );
				return pan;
			}
			set
			{
				FMOD.FMOD.Channel_SetPan( _id, value );
			}
		}		

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GXPEngine.Channel"/> is paused.
		/// </summary>
		/// <value>
		/// <c>true</c> if paused; otherwise, <c>false</c>.
		/// </value>
		public bool IsPaused   
		{
			get 
			{
				bool paused;
				FMOD.FMOD.Channel_GetPaused( _id, out paused );
				return paused;
			}
			set
			{
				FMOD.FMOD.Channel_SetPaused( _id, value );
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GXPEngine.Channel"/> is playing. (readonly)
		/// </summary>
		/// <value>
		/// <c>true</c> if playing; otherwise, <c>false</c>.
		/// </value>
		public bool IsPlaying  
		{
			get 
			{
				bool playing;
				FMOD.FMOD.Channel_IsPlaying( _id, out playing );
				return playing;
			}
		}		
		
		/// <summary>
		/// Stop the channel.
		/// </summary>
		public void Stop()
		{
			FMOD.FMOD.Channel_Stop( _id );
			_id = 0;
		}
	
		/// <summary>
		/// Gets or sets the volume. Should be in range 0...1
		/// </summary>
		/// <value>
		/// The volume.
		/// </value>
		public float Volume 
		{
			get 
			{
				float volume;
				FMOD.FMOD.Channel_GetVolume( _id, out volume );
				return volume;
			}
			set
			{
				FMOD.FMOD.Channel_SetVolume( _id, value );
			}
		}
		
	}
}
