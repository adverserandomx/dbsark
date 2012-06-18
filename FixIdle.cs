using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Logic;
using Zeta.Common.Plugins;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.Service;

namespace fifidong {
	// Inherit IPlugin
	//		Zeta.Common.Plugins
    public class FixIdle : IPlugin {
		
		// ------------------------------------------------------------
		// Configuration here
		// ------------------------------------------------------------
		private uint _TIMELIMIT = 5;	// 5 Second Countdown.
	
		// Inherited Varaibles from Zeta.Common.Plugins.IPlugin
		public string Author { get { return "fifidong"; } }
		public string Description { get { return "Check for Idle and Fix it!"; } }
		public Window DisplayWindow { get { return null; } }
		public string Name { get { return "Fix Idle Version Alpha!"; } }
		
		// Inherited Varaibles from System.IEquatable<T>
		public Version Version { get { return new Version(0,1); } }
		
		// FixIdle Plugin Local Variables
		private bool isInValidGame;
		private bool isRestarting;
		private Vector3 curPos;
		private uint counter;
		private DateTime curTime;
		
		// Debugging variables
		private bool firstDBG = false;
		
		// Commonly used functions
		private void Log( string strMsg ) { Logging.Write( string.Format("[FixIdle] {0}", strMsg ) ); }
		
		// Functions
		public void OnDisabled() { Log( "Disabling the Plugin" ); }
		public void OnEnabled() { Log( "Enabling the Plugin" ); }
		
		// Initialize the Plugin Local Variables
		public void OnInitialize() {
			Log( "Initializing the Plugin" );
			isInValidGame = false;
			isRestarting = false;
			counter = 0;
			curTime = DateTime.Now;
			curPos = ZetaDia.Me.Position;
		}
		
		public void RemoveAllBehavior() {
			ProfileManager.CurrentProfileBehavior.ResetCachedDone();
		}
		
		// Do as little as possible here!
		public void OnPulse() {
		
			// Cond: Am I in aValid Game?
			if( ZetaDia.IsInGame && ZetaDia.Me.IsValid ) {
				
				// Cond: Am I in a town?
				if( !ZetaDia.Me.IsInTown && !isRestarting ) {
					if(  (DateTime.Now.Second != curTime.Second) ) { // Count every one second
						
						// Cond: Position is same and Not in combat!
						if( curPos == ZetaDia.Me.Position ) {
							// Log( "Counting " + counter.ToString() );
							bool isReadyToPort = (counter++ >= _TIMELIMIT);
							// Only when Position is same and Not in Combat... Do something.
							if( isReadyToPort ) {
							
								Log( "Using Town Portal" );
								
								// Now time has elapsed.  OH NOEZ.
								RemoveAllBehavior();
								ZetaDia.Me.UseTownPortal();
								isRestarting = true;
							}
							
						} else counter = 0;
						
						
						// And update the time variable
						curTime = DateTime.Now;
						curPos = ZetaDia.Me.Position;
						
					}
					
				// Cond: Did I invoke Restart and finally got to the Town?	
				} else if( ZetaDia.Me.IsInTown && isRestarting ) {
					
					// Leave the game and reset the flag
					ZetaDia.Service.Games.LeaveGame();
					isRestarting = false;
				}
			}
		}
		public void OnShutdown() {}
		public bool Equals( IPlugin inPlug ) {	
			return ( (inPlug.Name == Name) && (inPlug.Version == Version) && (inPlug.Author == Author) );
		}
		
    }
}
