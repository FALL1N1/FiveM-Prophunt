﻿using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using PropHuntV.Server.Player;
using PropHuntV.Util;

// ReSharper disable once ClassNeverInstantiated.Global
namespace PropHuntV.Server
{
	public class Server : BaseScript
	{
		public static Server ActiveInstance { get; private set; }

		private CachedConvar _requireSteam = new CachedConvar( "IsSteamRequired", "true", 5000 );
		/// <summary>
		/// Whether or not Steam is required to join the server.
		/// </summary>
		public bool IsSteamRequired => _requireSteam.Value.ToLower() == "true" || _requireSteam.Value == "1";

		private CachedConvar _maxPlayers = new CachedConvar( "maxPlayers", "32", 2500 );
		/// <summary>
		/// Gets the maximum amount of players defined with convar "maxPlayers".
		/// </summary>
		public int MaxPlayers => int.TryParse( _maxPlayers.Value, out var o ) ? o : 32;

		public SessionManager Sessions { get; }
		public PropHunt PropHunt { get; }

		public Server() {
			if( ActiveInstance != null ) return; // Only Instantiate Once.

			Sessions = new SessionManager( this );
			PropHunt = new PropHunt( this );

			EventHandlers["Server:SoundToClient"] += new Action<int, string, float>( ServerSoundToClient );
			EventHandlers["Server:SoundToAll"] += new Action<string, float>( ServerSoundToAll );
			EventHandlers["Server:SoundToRadius"] += new Action<int, float, string, float>( ServerSoundToRadius );
			EventHandlers["Server:SoundToCoords"] += new Action<float, float, float, float, string, float>( ServerSoundToCoords );

			ActiveInstance = this;
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

		public void RegisterExport( string exportName, Delegate callback ) {
			Exports.Add( exportName, callback );
		}

		public dynamic GetExport( string resourceName ) {
			return Exports[resourceName];
		}
		private void ServerSoundToClient( int netId, string soundFile, float soundVolume ) {
			var plyr = new PlayerList()[netId];
			plyr.TriggerEvent( "Client:SoundToClient", soundFile, soundVolume );
		}

		private void ServerSoundToAll( string soundFile, float soundVolume ) {
			TriggerClientEvent( "Client:SoundToAll", soundFile, soundVolume );
		}

		private void ServerSoundToRadius( int netid, float soundRadius, string soundFile, float soundVolume ) {
			TriggerClientEvent( "Client:SoundToRadius", netid, soundRadius, soundFile, soundVolume );
		}

		private void ServerSoundToCoords( float positionX, float positionY, float positionZ, float soundRadius, string soundFile, float soundVolume ) {
			TriggerClientEvent( "Client:SoundToCoords", positionX, positionY, positionZ, soundRadius, soundFile, soundVolume );
		}

	}
}
