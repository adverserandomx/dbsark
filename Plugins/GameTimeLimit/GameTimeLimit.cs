/* sample code from fifidong
 * heavily modified by adx
 */ 
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

namespace MBot {

    public class GameTimeLimit : IPlugin   {
		
		// ------------------------------------------------------------
		// Configuration here
		// ------------------------------------------------------------
	
		// Inherited Varaibles from Zeta.Common.Plugins.IPlugin
		public string Author { get { return "adx"; } }
        public string Description { get { return "Quit when Game is longer than  " + TIME_LIMIT; } }
		public Window DisplayWindow { get { return null; } }
		public string Name { get { return "Game Time Limit"; } }
		
		// Inherited Varaibles from System.IEquatable<T>
		public Version Version { get { return new Version(0,1); } }
		
		private bool _isRestarting;
		private DateTime _gameStartTime;

        private const uint TIME_LIMIT = 100; //setting to 100 because bot needs to repair/salvage/sell which can take up to 80 seconds

        public void OnDisabled()
        {
            Log("Disabled", Name);
            Zeta.CommonBot.GameEvents.OnGameJoined -= new EventHandler<EventArgs>(GameEvents_OnGameJoined);
            Zeta.CommonBot.GameEvents.OnGameLeft -= new EventHandler<EventArgs>(GameEvents_OnGameLeft);
            //Zeta.CommonBot.GameEvents.OnPlayerDied -= new EventHandler<EventArgs>(GameEvents_OnPlayerDied);
        }

        public void OnEnabled()
        {
            Log("Enabled. Time limit set to " + TIME_LIMIT, Name);
            Zeta.CommonBot.GameEvents.OnGameJoined += new EventHandler<EventArgs>(GameEvents_OnGameJoined);
            Zeta.CommonBot.GameEvents.OnGameLeft += new EventHandler<EventArgs>(GameEvents_OnGameLeft);

            // supported in .125 and higher only
            //Zeta.CommonBot.GameEvents.OnPlayerDied += new EventHandler<EventArgs>(GameEvents_OnPlayerDied);
            
        }

        void GameEvents_OnGameJoined(object sender, EventArgs e)
        {
            _isRestarting = false;
            _gameStartTime = DateTime.Now;
            Log("Timer started at: " + _gameStartTime, Name);
        }

        void GameEvents_OnGameLeft(object sender, EventArgs e)
        {
            //this is probably redundant but I worry about weird start/stop conditions for plugins since it seems plugin state isn't kept very well by the bot
            _isRestarting = false;
        }


        // supported in .125 and higher only
        void GameEvents_OnPlayerDied(object sender, EventArgs e)
        {
            //todo: add better handling of player death.  In these cases, we should consider instaquiting.
            return;
        }

        public void OnInitialize()
        {
            return;
        }

        public void OnPulse()
        {
            CheckIdle();
        }

        public void OnShutdown()
        {
            return;
        }

        public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }

        private bool _loggedTimerMessage = false;
  
        public void CheckIdle()
        {

            //this section is to handle printing the Game Timer message once.  
            //We need a more sophisticated way of timing ticks in general so we don't get functions called multiple times
            int idledSeconds = DateTime.Now.Second - _gameStartTime.Second;
            if (idledSeconds % 10 == 0 && _loggedTimerMessage == false)
            {
                _loggedTimerMessage = true;
                Log("Game Timer: " + idledSeconds + " sec.", Name);
            }
            else if (idledSeconds % 11 == 0)
            {
                _loggedTimerMessage = false;
            }


            if (ZetaDia.IsInGame && ZetaDia.Me.IsValid)  // Check for in-game
            {
                if (ZetaDia.Me.IsInTown == false && _isRestarting == false) // checking for IsInTown outside of game sometimes causes an exception
                {
                    if (idledSeconds > TIME_LIMIT)
                    {
                        _isRestarting = true;
                        Log("Time Limit reached:  " + TIME_LIMIT, Name);
                        RemoveAllBehavior();
                        ZetaDia.Me.UseTownPortal();
                    }
                }
                else if (ZetaDia.Me.IsInTown && _isRestarting)
                {
                    RemoveAllBehavior();
                    ZetaDia.Service.Games.LeaveGame();
                    _isRestarting = false;
                }
            }
        }


        public static void RemoveAllBehavior()
        {
            ProfileManager.CurrentProfileBehavior.ResetCachedDone();
        }

        public static void Log(string message, string name)
        {
            Logging.Write(string.Format("[{0}] {1}", name, message));
        }
    }
}
