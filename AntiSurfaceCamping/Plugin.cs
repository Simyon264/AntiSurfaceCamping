/*
 * Made by Simyon#6969 
 * ==============
 * Made for DayLight Gaming discord.gg/RxzaN3jGeb
 * 
 * This plugin adds: 
 *      - A way to stop people from camping on the Surface zone (By causing harm :troll:).
 * ==================================================================================================================================================================
*/

using System;
using Exiled.API.Interfaces;
using Exiled.API.Features;
using Exiled.API.Enums;
using Handlers = Exiled.Events.Handlers;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using System.ComponentModel;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace AntiSurfaceCamping
{
    public class AntiSurface : MonoBehaviour
    {
        public Plugin plugin;
        public bool disabled = false;

        public Player player;

        public float delay = 1.0f;

        public bool clearTimer = false;

        public int SurfaceTime = 0; // How long a player has been on the Surface zone.

        void Awake()
        {
            player = Player.Get(gameObject);
            Timing.RunCoroutine(Timer());
        }


        public IEnumerator<float> Timer()
        {
            for(; ; )
            {
                yield return Timing.WaitForSeconds(delay);
                if (this.player != null)
                {
                    if (!this.disabled)
                    {
                        try
                        {
                            CampingChecker();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                }
            }
        }

        private void CampingChecker()
        {
            if (SurfaceTime < 0)
            {
                SurfaceTime = 1;
            }

            if (clearTimer)
            {
                SurfaceTime = 0;
                clearTimer = false;
            }

            if (Round.IsStarted)
            {
                if (player.IsDead || player.Role.Team == Team.SCPs)
                {
                    SurfaceTime = 0;
                    return;
                }
                Log.Debug($"Checking {player.Nickname}");
                try
                {
                    if (player.Zone == ZoneType.Surface)
                    {
                        Log.Debug($"{player.Nickname} is in Surface Zone");
                        if (plugin.Config.immuneRoles.Contains(player.Role))
                        {
                            Log.Debug($"{player.Nickname} is a immune Role.");
                            SurfaceTime = 0;
                            return;
                        }
                        if (Warhead.IsInProgress || Warhead.IsDetonated)
                        {
                            if (plugin.Config.DisableIfWarhead)
                            {
                                Log.Debug($"{player.Nickname} Warhead is on.");
                                SurfaceTime--;
                                return;
                            }
                        }
                        if (plugin.IsSCPOnSurface() && plugin.Config.DisableIfSCPOnSurface)
                        {
                            Log.Debug($"{player.Nickname} SCP is on the surface.");
                            SurfaceTime--;
                            return;
                        }
                        SurfaceTime++;
                        Log.Debug($"{player.Nickname}: {SurfaceTime}");
                        if (SurfaceTime >= plugin.Config.TimeUntilNotice && SurfaceTime <= plugin.Config.TimeUntilDamage)
                        {
                            player.Broadcast(1, plugin.Config.NoticeMessage);
                        }
                        if (SurfaceTime >= plugin.Config.TimeUntilDamage)
                        {
                            if (SurfaceTime >= plugin.Config.TimeUntilFatalDamage)
                            {
                                player.Hurt(plugin.Config.FatalDamage, DamageType.Decontamination);
                                player.Broadcast(1, plugin.Config.FatalDamageMessage);
                            }
                            else
                            {
                                player.Hurt(plugin.Config.Damage, DamageType.Decontamination);
                                player.Broadcast(1, plugin.Config.DamageMessage);
                            }
                        }
                    }
                    else
                    {
                        SurfaceTime--;
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
            }
        }
    }
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("If debug mode is enabled. When enabled, the plugin will log debug messages to the console. DON'T USE WITH MORE THEN 1 PLAYER, CONSOLE WILL FLOOD.")]
        public bool Debug { get; set; }

        [Description("The damage camping players get.")]
        public float Damage { get; set; } = 0.5f;
        [Description("The time until the player will be notified that they will start getting damage soon.")]
        public int TimeUntilNotice { get; set; } = 30;
        [Description("The message to display when players will soon start to get damage")]
        public string NoticeMessage { get; set; } = "Please do not camp on the surface Zone. You will soon get damage!";
        [Description("The message to display when the player is getting damage because they are camping.")]
        public string DamageMessage { get; set; } = "You are getting damage because you are camping on the surface.";
        [Description("The message to display when the player starts getting fatal damage because they are camping too long.")]
        public string FatalDamageMessage { get; set; } = "You are getting fatal damage because you are camping on the surface.";
        [Description("The time until the player gets damage.")]
        public int TimeUntilDamage { get; set; } = 60;
        [Description("If the Warhead should disable the camping timer.")]
        public bool DisableIfWarhead { get; set; } = true;
        [Description("If an SCP on the surface should disable the camping timer.")]
        public bool DisableIfSCPOnSurface { get; set; } = true;
        [Description("The time until fatal damage starts.")]
        public int TimeUntilFatalDamage { get; set; } = 100;
        [Description("The amount of fatal damge players should get.")]
        public float FatalDamage { get; set; } = 5f;

        [Description("Roles which dont get camping checked.")]
        public List<RoleTypeId> immuneRoles { get; set; } = new List<RoleTypeId>()
        {
            { RoleTypeId.Tutorial }
        };

    }
    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "AntiSurfaceCamping";
        public override string Prefix { get; } = "asc";
        public override string Author { get; } = "Simyon";
        public override Version Version { get; } = new Version(1, 0, 4);
        public override PluginPriority Priority { get; } = PluginPriority.High;

        private static readonly Plugin InstanceValue = new Plugin();
        private Plugin()
        {

        }

        public static Plugin StaticInstance => InstanceValue;

        // Vars
        private double LastSCPCheck = 0f;
        private bool LastSCPOnSurface = false;

        public override void OnEnabled()
        {
            Handlers.Player.Verified += OnPlayerVerified;
            Handlers.Player.ChangingRole += OnRoleChange;
            Handlers.Player.Dying += OnDeath;
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Handlers.Player.Verified -= OnPlayerVerified;
            Handlers.Player.ChangingRole -= OnRoleChange;
            Handlers.Player.Dying -= OnDeath;

            base.OnDisabled();
        }

        public bool IsSCPOnSurface()
        {
            Log.Debug($"{Round.ElapsedTime.TotalSeconds} ROUND TIME");
            Log.Debug($"{LastSCPCheck} LAST CHECK");
            if (Round.ElapsedTime.TotalSeconds < LastSCPCheck + 3)
            {
                return LastSCPOnSurface;
            } else
            {
                bool isScpOnSuface = false;
                foreach (Player player in Player.List)
                {
                    if (player.Role.Team == Team.SCPs)
                    {
                        if (player.Zone == ZoneType.Surface)
                        {
                            isScpOnSuface = true;
                        }
                    }
                }
                LastSCPCheck = Round.ElapsedTime.TotalSeconds;
                LastSCPOnSurface = isScpOnSuface;
                return isScpOnSuface;
            }
        }

        public void OnDeath(DyingEventArgs ev)
        {
            AntiSurface comp = ev.Player.GameObject.GetComponent<AntiSurface>();
            comp.player = ev.Player;
            comp.SurfaceTime = 0;
            comp.disabled = true;
        }

        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            AntiSurface comp = ev.Player.GameObject.AddComponent<AntiSurface>();
            comp.player = ev.Player;
            comp.plugin = StaticInstance;
        }

        public void OnRoleChange(ChangingRoleEventArgs ev)
        {
            AntiSurface comp = ev.Player.GameObject.GetComponent<AntiSurface>();
            comp.player = ev.Player;
            if (ev.NewRole != RoleTypeId.None || ev.NewRole != RoleTypeId.Spectator)
            {
                comp.clearTimer = true;
                comp.SurfaceTime = 0;
                comp.disabled = false;
            }
        }
    }
}
