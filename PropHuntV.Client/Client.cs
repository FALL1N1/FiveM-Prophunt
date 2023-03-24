using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PropHuntV.Client.Game;
using PropHuntV.Client.Player;
using PropHuntV.SharedModels;
using PropHuntV.Util;
using static CitizenFX.Core.Native.API;

// ReSharper disable once ClassNeverInstantiated.Global
namespace PropHuntV.Client
{
	public class Client : BaseScript
	{
		public static Client ActiveInstance { get; private set; }

		public GameController Game { get; }
		public PlayerController Player { get; }
		public WorldController World { get; }
		public IplController Ipl { get; }
		public TimeController Time { get; }
		public SessionManager Sessions { get; }
		public PropHunt PropHunt { get; }
		public PlayerList PlayersAvailable { get; set; }
		//public PlayerList Players { get;  }

		public Client() {
			if( ActiveInstance != null ) return; // Only instantiate once
			 
			Game = new GameController( this );
			Player = new PlayerController( this );
			World = new WorldController( this );
			Ipl = new IplController( this );
			Time = new TimeController( this );
			Sessions = new SessionManager( this );
			PropHunt = new PropHunt( this );
			PlayersAvailable =  Players ; 

			ActiveInstance = this;
			 
			EventHandlers["Client:PlaySound"] += new Action<float, float, float, float, string, float>( SoundToCoords );
		}
		 
		private void SoundToCoords( float positionX, float positionY, float positionZ, float soundRadius, string soundFile, float soundVolume ) {
			Log.Info( "PlaySound :: " + positionX + "f, " + positionY + "f, " + positionZ + "f, " + soundRadius + "f, " + "Sound: " + soundFile + ", Vol: " + soundVolume);
			Vector3 playerCoords = Player.Position; 
			float compare = Vdist( playerCoords.X, playerCoords.Y, playerCoords.Z, positionX, positionY, positionZ );
			float distanceVolumeMultiplier = (soundVolume / soundRadius);
			float distanceVolume = soundVolume - (compare * distanceVolumeMultiplier);
			Log.Info( "Compare: " + compare + ", Radius: " + soundRadius + ", distanceVolumeMultiplier: " + distanceVolumeMultiplier + ", distanceVolume: "+ distanceVolume);
			if( compare <= soundRadius ) {
				Log.Info( "OK, PLAYING!" );
				SendNuiMessage( string.Format( "{{\"submissionType\":\"sendSound\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)distanceVolume, (object)soundFile ) );
			} else {
				Log.Info( "NO, TOO FAR!" );
				Log.Info( "[FALSE] Compare: " + compare + ", Radius: " + soundRadius + ", distanceVolumeMultiplier: " + distanceVolumeMultiplier + ", distanceVolume: " + distanceVolume );
			}
		}

		public void RegisterEventHandler( string eventName, Delegate action ) {
			EventHandlers[eventName] += action;
		}

		public void RegisterTickHandler( Func<Task> tick ) {
			Tick += tick;
		}

		public void DeregisterTickHandler( Func<Task> tick ) {
			Tick -= tick;
		}

		public void RegisterNuiEventHandler( string eventName, Action<IDictionary<string, object>> action ) {
			RegisterNuiCallbackType( eventName );
			RegisterEventHandler( $"__cfx_nui:{eventName}", new Action<ExpandoObject>( o => {
				IDictionary<string, object> data = o;
				action.Invoke( data );
			} ) );
		}

		public void TriggerNuiEvent( string eventName, dynamic data = null ) {
			SendNuiMessage( JsonConvert.SerializeObject( new NuiEventModel {
				EventName = eventName,
				EventData = data ?? new object()
			} ) );
		}

		public void RegisterExport( string exportName, Delegate callback ) {
			Exports.Add( exportName, callback );
		}

		public dynamic GetExport( string resourceName ) {
			return Exports[resourceName];
		}
	}
}
