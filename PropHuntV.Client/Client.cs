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

		public Client() {
			if( ActiveInstance != null ) return; // Only instantiate once

			Game = new GameController( this );
			Player = new PlayerController( this );
			World = new WorldController( this );
			Ipl = new IplController( this );
			Time = new TimeController( this );
			Sessions = new SessionManager( this );
			PropHunt = new PropHunt( this );

			ActiveInstance = this;

			EventHandlers["Client:SoundToClient"] += new Action<string, float>( SoundToClient );
			EventHandlers["Client:SoundToAll"] += new Action<string, float>( SoundToAll );
			EventHandlers["Client:SoundToRadius"] += new Action<int, float, string, float>( SoundToRadius );
			EventHandlers["Client:SoundToCoords"] += new Action<float, float, float, float, string, float>( SoundToCoords );
		}

		private void SoundToClient( string soundFile, float soundVolume ) {
			SendNuiMessage( string.Format( "{{\"submissionType\":\"sendSound\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)soundVolume, (object)soundFile ) );
		}

		private void SoundToAll( string soundFile, float soundVolume ) {
			SendNuiMessage( string.Format( "{{\"submissionType\":\"sendSound\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)soundVolume, (object)soundFile ) );
		}

		private void SoundToRadius( int networkId, float soundRadius, string soundFile, float soundVolume ) {
			//Vector3 playerCoords = Game.Player.Character.Position;
			Vector3 playerCoords = NetworkGetPlayerCoords( networkId );
			Vector3 targetCoords = GetEntityCoords( NetworkGetEntityFromNetworkId( networkId ), true );
			float distance = Vdist( playerCoords.X, playerCoords.Y, playerCoords.Z, targetCoords.X, targetCoords.Y, targetCoords.Z );
			float distanceVolumeMultiplier = (soundVolume / soundRadius);
			float distanceVolume = soundVolume - (distance * distanceVolumeMultiplier);
			if( distance <= soundRadius ) {
				SendNuiMessage( string.Format( "{{\"submissionType\":\"sendSound\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)distanceVolume, (object)soundFile ) );
			}

		}

		private void SoundToCoords( float positionX, float positionY, float positionZ, float soundRadius, string soundFile, float soundVolume ) {
			//Vector3 playerCoords = Game.Player.Character.Position;
			Vector3 playerCoords = NetworkGetPlayerCoords( Player.PlayerPed.NetworkId );
			float compare = Vdist( playerCoords.X, playerCoords.Y, playerCoords.Z, positionX, positionY, positionZ );
			float distanceVolumeMultiplier = (soundVolume / soundRadius);
			float distanceVolume = soundVolume - (compare * distanceVolumeMultiplier);
			if( compare <= soundRadius ) {
				SendNuiMessage( string.Format( "{{\"submissionType\":\"sendSound\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)distanceVolume, (object)soundFile ) );
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
