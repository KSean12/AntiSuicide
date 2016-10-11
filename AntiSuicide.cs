using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using SDG;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AntiSuicide
{
    class AntiSuicide : RocketPlugin<AntiSuicideConfiguration>
    {

        public static Dictionary<string, SpawnLocation> SpawnLocations = new Dictionary<string, SpawnLocation>();
        public static Dictionary<string, byte> SuicideCount = new Dictionary<string, byte>();
        public static AntiSuicide Instance;
        protected override void Load()
        {
            Instance = this;
            if (!Configuration.Instance.Enabled) return;
            UnturnedPlayerEvents.OnPlayerRevive += OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerRevive(UnturnedPlayer player ,  Vector3 position, byte angle)
        {
            if (player.IsAdmin || R.Permissions.HasPermission(player , "anti-suicide.imune")) return;
            if (!validateKick(player)) validateRelocation(player, position, angle);
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (player.IsAdmin || player.HasPermission("anti-suicide.imune")) return;

            if (cause != EDeathCause.SUICIDE && cause != EDeathCause.BREATH)
            {
                if (SpawnLocations.ContainsKey(player.ToString()))
                    SpawnLocations.Remove(player.ToString());

                if (SuicideCount.ContainsKey(player.ToString()))
                    SuicideCount.Remove(player.ToString());
            }
        }
        private Timer timer;

        private void validateRelocation(UnturnedPlayer player, Vector3 position, float angle, bool generate = true)
        {
            if (SpawnLocations.ContainsKey(player.ToString()))
            {
                timer = new Timer(obj =>
                {
                    teleport(player);
                    timer.Dispose();
                    timer = null;
                }, null, 500, Timeout.Infinite);
            }
            else
            {
               if (generate) SpawnLocations.Add(player.ToString(), new SpawnLocation(position, angle));
            }
        }

        private bool validateKick(UnturnedPlayer player)
        {
            if (SuicideCount.ContainsKey(player.ToString()))
                SuicideCount[player.ToString()] += 1;
            else
                SuicideCount.Add(player.ToString(), 1);

            if (SuicideCount[player.ToString()] >= Configuration.Instance.MaxSuicidesBeforeKick)
            {
                player.Kick(Translate("anti_suicide_kick_reason"));
                log(Translate("anti_suicide_kick_log", player.CharacterName));
                if (Configuration.Instance.ResetSuicideCountAfterKick && SuicideCount.ContainsKey(player.ToString()))
                    SuicideCount.Remove(player.ToString());
                return true;
            }
            return false;
        }

        private void teleport(UnturnedPlayer player)
        {
            if (SpawnLocations.ContainsKey(player.ToString()))
            {
                UnturnedChat.Say(player, Translate("anti_suicide_warning"));
                player.Teleport(SpawnLocations[player.ToString()].Position, SpawnLocations[player.ToString()].Angle);
            }
                
        }

        private void log(string message)
        {
            Logger.LogWarning("#################### ");
            Logger.LogWarning("##  ANTI-SUICIDE  ## : " + message);
            Logger.LogWarning("#################### ");
        }

        // Translations

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"anti_suicide_kick_log","{0} was kicked for multiple suicides!"},
                    {"anti_suicide_kick_reason","Anti-Suicide - Please stop that!"},
                    {"anti_suicide_warning","Anti-Suicide - Teleporting to last Location!"}
                };
            }
        }
    }
}
