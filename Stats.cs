using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
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

//Edited and continued by Icool
//Created by: w3eZle
//Credit to: eax (http://www.thebuddyforum.com/members/144149-eax.html) No1KnowsY (http://www.thebuddyforum.com/members/11607-no1knowsy.html)
//Based script on these guys work
//Created Date: 12June2012

namespace Sarkoth
{
    public class Sarkoth 
    {
        private bool IsRestarting { get; set; }
        public Version Version { get { return new Version(0, 1); } }
        public string Author { get { return "Icool"; } }
        public string Description { get { return "Provides helpful methods while farming Sarkoth."; } }
        public string Name { get { return "Icool's Sarkoth farm helper v" + Version; } }
        public bool hasDied { get; set; }
        public int diedTickCount { get; set; }
        public int cellarsFoundInGames { get; set; }
        public int games { get; set; }
        public long startingMoney { get; set; }
        public long currentMoney { get; set; }
        public bool foundCellar { get; set; }

        public long averageFail { get; set; }
        public long averageClear { get; set; }

        public long thisRunStart { get; set; }

        public int lastSSTick { get; set; }
        public Window DisplayWindow
        {
            get
            {
                return null;
            }
        }

        private void Log(string message)
        {
            Logging.Write(string.Format("[{0}] {1}", Name, message));
        }


        public void OnInitialize()
        {
            games = 0;
            cellarsFoundInGames = 0;
            averageFail = 0;
            averageClear = 0;
            IsRestarting = false;
            lastSSTick = 0;
        }

        public void RemoveAllBehavior()
        {
            ProfileManager.CurrentProfileBehavior.ResetCachedDone();
        }

        public void OnPulse()
        {

            //Checks if 20 seconds has passed since you died
            if (hasDied)
            {
                if (System.Environment.TickCount - diedTickCount > 20000)
                {
                    Log("20seconds has passed since you died, turning of the attack");
                    hasDied = false;
                    return;
                }
            }
            
            // if we're not in game and not in the process of restarting, do nothing
            if (!ZetaDia.IsInGame || !ZetaDia.Me.IsValid || IsRestarting)
            {
                return;
            }

            //Turns on the attack for 20s after you get killed to prevent graveyard ganking
            if (ZetaDia.Actors.Me.HitpointsCurrentPct == 0.00)
            {
                Log("Wooooa, seems like i just died, lets turn on the attack for 20s");
                hasDied = true;
                ProfileManager.CurrentProfile.KillMonsters = true;
                ProfileManager.CurrentProfile.PickupLoot = true;
                diedTickCount = System.Environment.TickCount;
                return;
            }

            //Turning on the attack if you get attacked outside the cellar w3eZle's method
       	    if (ZetaDia.Actors.Me.HitpointsCurrentPct < 0.80 && ProfileManager.CurrentProfile.KillMonsters == false)
            {
                Log("Turning on KillMonsters -Health to low");
                ProfileManager.CurrentProfile.KillMonsters = true;
                ProfileManager.CurrentProfile.PickupLoot = true;
                return;
            }
	
            //Outside of DankC ellar, Diable Kill Monsters
            if (ZetaDia.Actors.Me.HitpointsCurrentPct > 0.80 && ProfileManager.CurrentProfile.KillMonsters == true && ZetaDia.CurrentWorldDynamicId == 1999503360 && !hasDied)
            {
                Log("Disabeling KillMonsters & PickupLoot");
                ProfileManager.CurrentProfile.KillMonsters = false;
                ProfileManager.CurrentProfile.PickupLoot = false;
                return;
            }



            //Inside of Dank Cellar, Enable Kill Monsters
            if (ProfileManager.CurrentProfile.KillMonsters == false && ZetaDia.CurrentWorldDynamicId == 1999568897)
            {
                Log("Enabeling KillMonsters");
                ProfileManager.CurrentProfile.KillMonsters = true;
                
                return;
            }

            //Enabling loot if loot is off while entering the cellar
            if (ProfileManager.CurrentProfile.PickupLoot == false && ZetaDia.CurrentWorldDynamicId == 1999568897)
            {
                Log("Enabling Loot");
                ProfileManager.CurrentProfile.PickupLoot = true;
                return;
            }

        }

        public void OnShutdown()
        {
        }

        public void OnEnabled()
        {
            GameEvents.OnGameJoined += new EventHandler<EventArgs>(OnGameJoined);
            GameEvents.OnGameLeft += new EventHandler<EventArgs>(OnGameLeft);
            GameEvents.OnWorldChanged += new EventHandler<EventArgs>(OnWorldChanged);
            GameEvents.OnItemLooted += new EventHandler<ItemLootedEventArgs>(OnItemLooted);
            Log("Enabled.");
        }

        public void OnGameJoined(object sender, EventArgs e)
        {
            Log("Joined Game");
            thisRunStart = System.Environment.TickCount;
            foundCellar = false;
        }

        public void OnGameLeft(object sender, EventArgs e)
        {
            Log("Left Game");
            games++;
            Log("Cellars found in " + cellarsFoundInGames + "/" + games + " games!");

            long thisTime = System.Environment.TickCount - thisRunStart;
            if (foundCellar)
            {
                averageClear = ((averageClear * (cellarsFoundInGames - 1)) + thisTime) / cellarsFoundInGames;
                Log("This Run took: " + (thisTime / 1000) + "s");
                Log("The average cellar run is: " + (averageClear / 1000) + "s");
            }
            else
            {
                averageFail = ((averageFail * ((games - cellarsFoundInGames) - 1)) + thisTime) / (games - cellarsFoundInGames);
                Log("This Run took: " + (thisTime/1000) + "s");
                Log("The average fail run is: " + (averageFail / 1000) + "s");
            }

        }

        public void OnWorldChanged(object sender, EventArgs e)
        {
            Log("Changed world to " + ZetaDia.CurrentWorldDynamicId);
            if (ZetaDia.CurrentWorldDynamicId == 1999568897)
            {
                Log("Found a cellar!");
                cellarsFoundInGames++;
                foundCellar = true;
            }
        }

        public void OnItemLooted(object sender, ItemLootedEventArgs e)
        {
        }

        public void OnDisabled()
        {
            Log("Disabled.");
        }

        public bool Equals(IPlugin other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }
    }

}
