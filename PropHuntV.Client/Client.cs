﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
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
			PlayersAvailable = Players;

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
