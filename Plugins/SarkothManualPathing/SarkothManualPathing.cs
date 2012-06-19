/* original code by McCloud
 * Modified by adx
 * Integrated several other features such as stat tracking and anti-idle detection
 * TODO: Add manual pathing for selling/reparing and salvaging
 */
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Common.Plugins;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MBot
{
    public class Sarkoth : IPlugin
    {
        private bool IsRestarting { get; set; }
        public Version Version { get { return new Version(1, 1); } }
        public string Author { get { return "adx"; } }
        public string Description { get { return "Sarkoth runner with anti-idle and stats."; } }
        public string Name { get { return "DH Sarkoth Manual Bot v" + Version; } }
        public Window DisplayWindow { get { return null; } }

        #region IPlugin implementation
        public void OnShutdown() { OnDisabled(); }

        public void OnEnabled()
        {
            Log("Enabled", Name);
            Zeta.CommonBot.GameEvents.OnGameJoined += new EventHandler<EventArgs>(GameEvents_GameJoined);
            Zeta.CommonBot.GameEvents.OnGameLeft += new EventHandler<EventArgs>(GameEvents_GameLeft);
            //Zeta.CommonBot.GameEvents.OnPlayerDied += new EventHandler<EventArgs>(GameEvents_PlayerDied); //re-enable when we know build 125 and later are stable.  build 103 is rock solid and doesn't support this event
            running = true;
            if (ZetaDia.IsInGame && !ZetaDia.IsLoadingWorld) 
                Execute();
        }

        public void OnInitialize() { }

        public void OnDisabled()
        {
            Log("Disabled", Name);
            Zeta.CommonBot.GameEvents.OnGameJoined -= GameEvents_GameJoined;
            Zeta.CommonBot.GameEvents.OnGameLeft -= GameEvents_GameLeft;
            //Zeta.CommonBot.GameEvents.OnPlayerDied -= GameEvents_PlayerDied;
            running = false;
        }
        public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }
        #endregion

        void GameEvents_GameLeft(object sender, EventArgs e)
        {
            Log("Fired Game Left Event", Name);
        }

        public void GameEvents_GameJoined(object sender, EventArgs e) 
        {
            Log("Joining game", Name);
            step = 0; 
            Execute(); 
        }

        public void GameEvents_PlayerDied(object sender, EventArgs e)
        {
            //if we die just quit.  Don't try to do anything fancy since it really screws with the bot.
            Log("Quitting on death", Name);
            step = 13;
            RemoveAllBehavior();
            ZetaDia.Service.Games.LeaveGame();
        }


        public int step = 0;
        public bool running = false;

        /// <summary>
        /// TODO: Get rid of case switching and stepping. Makes it harder to reorder functions
        /// </summary>
        public void Execute()
        {
            while (running && BotMain.IsRunning)
            {
                BotMain.CurrentBot.Pulse();
                switch (step)
                {
                    case 0:
                        if (ZetaDia.IsInGame && !ZetaDia.IsLoadingWorld && ZetaDia.Me.IsValid)
                        {
                            ZetaDia.Me.UsePower(SNOPower.DemonHunter_Companion, ZetaDia.Me.Position); //if i don't cast it here, often when i get into cellar, I'm out of discipline.  However, ferrets do agro mobs...
                            step++;
                        }
                        break;
                    case 1:
                        if (MoveWithPower(1993, 2603))
                
                            step++;
                        break;
                    case 2:
                        if (MoveWithPower(2044, 2529)) //MoveSuperFuckingFast(2026, 2557) //2044, 2529
                            step++;
                        break;
                    case 3:
                        ZetaDia.Me.UsePower(SNOPower.DemonHunter_Preparation, ZetaDia.Me.Position);
                        if (!ConditionParser.ActorExistsAt(176007, 2059, 2478, 27, 15))
                        {
                            ZetaDia.Me.UsePower(SNOPower.DemonHunter_SmokeScreen, ZetaDia.Me.Position);
                            step = 13;
                            break;
                        }
                        step++;
                        break;
                    case 4:
                        if (MoveWithPower(2046, 2527))
                            step++;
                        break;
                    case 5:
                        if (MoveWithPower(2078, 2492))
                            step++;
                        break;
                    case 6:

                        //ZetaDia.Me.UsePower(SNOPower.DemonHunter_Vault, new Vector3(2066, 2477, ZetaDia.Me.Position.Z), ZetaDia.Me.WorldDynamicId, 2, -1);
                        if (MoveWithPower(2066, 2477))
                            step++;
                        break;
                    case 7:
                        ZetaDia.Me.UsePower(SNOPower.DemonHunter_Preparation, ZetaDia.Me.Position);
                        step++;
                        break;
                    case 8:
                        if (Zeta.ZetaDia.CurrentWorldId == 71150)
                            Interact("g_Portal_Square_Blue");
                        step++;
                        break;
                    case 9:
                        if (Zeta.ZetaDia.CurrentWorldId == 71150)
                            step = 7;
                        else step++;
                        break;
                    case 10:
                        ZetaDia.Me.UsePower(SNOPower.DemonHunter_Companion, ZetaDia.Me.Position);
                        step++;
                        break;
                    case 11:
                        if (MoveWithPower(125, 160))
                            step++;
                        break;
                    case 12:
                        if (MoveTo(120, 137))
                            step++;
                        break;
					case 13:
                        //check for repair and sell

                        
                        //manually quit
                        //Helper.RemoveAllBehavior();
                        //ZetaDia.Me.UseTownPortal();
                        //ZetaDia.Service.Games.LeaveGame();
                        break;
                }
				//if (ZetaDia.Me.IsDead) step = 13;
                if (step == 13) 
                    break;
            }

        }

        public void OnPulse()
        {
            

        }

        public bool Interact(string name)
        {
            DiaObject unit = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault(u => u.Name.Contains(name));
            if (unit.Name.Contains(name))
                return unit.Interact();
            else return false;
        }

        public bool Interact(DiaObject u)
        {
            return ZetaDia.Me.UsePower(u.ActorType == Zeta.Internals.SNO.ActorType.Gizmo || u.ActorType == Zeta.Internals.SNO.ActorType.Item ? SNOPower.Axe_Operate_Gizmo : SNOPower.Axe_Operate_NPC, u.Position);
        }

        public bool MoveTo(float x, float y)
        {
			if (ZetaDia.Me.IsDead) { step = 13; }
            if ((float)Math.Sqrt(Math.Pow(ZetaDia.Me.Position.X - x, 2) + Math.Pow(ZetaDia.Me.Position.Y - y, 2)) < 5)
                return true;
            Zeta.Navigation.Navigator.PlayerMover.MoveTowards(new Vector3(x, y, ZetaDia.Me.Position.Z));
            return false;
        }

        public bool MoveWithPower(float x, float y)
        {
            while (new Vector3(x, y, ZetaDia.Me.Position.Z).Distance(ZetaDia.Me.Position) > 10 && running && BotMain.IsRunning)
            {
                if (new Vector3(x, y, ZetaDia.Me.Position.Z).Distance(ZetaDia.Me.Position) > 20 && Zeta.CommonBot.PowerManager.CanCast(SNOPower.DemonHunter_Vault))
                {
                    ZetaDia.Me.UsePower(SNOPower.DemonHunter_Vault, new Vector3(x, y, ZetaDia.Me.Position.Z), ZetaDia.Me.WorldDynamicId, 2, -1);
                    Thread.Sleep(350);
                }
                //don't waste discipline smokescreening.  You don't have enough to vault all the way if you do
                //else if (new Vector3(x, y, ZetaDia.Me.Position.Z).Distance(ZetaDia.Me.Position) > 30 && Zeta.CommonBot.PowerManager.CanCast(SNOPower.DemonHunter_SmokeScreen))
                //{
                //    ZetaDia.Me.UsePower(SNOPower.DemonHunter_SmokeScreen, new Vector3(x, y, ZetaDia.Me.Position.Z), ZetaDia.Me.WorldDynamicId, 2, -1);
                //    MoveTo(x, y);
                //}
                else
                {
                    MoveTo(x, y);
                }
            }
            return true;
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