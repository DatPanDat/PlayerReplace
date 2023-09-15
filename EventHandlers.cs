using MEC;
using System.Linq;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;

namespace PlayerReplace.EventHandler
{
    public class EventHandlers
    {
        private readonly Config config;

        private bool isRoundStarted = false;

        public void OnRoundStart() //Check if the round has started
        {
            isRoundStarted = true;
        }

        public void OnRoundEnd() => isRoundStarted = false; //If the round has ended, turn the replace off.

        public void OnPlayerDestroying(DestroyingEventArgs ev)
        {
            if (!isRoundStarted || config.RestrictedRoles.Contains(ev.Player.Role) || ev.Player.Position.y < -1997 || (ev.Player.CurrentRoom.Zone == ZoneType.LightContainment && Map.IsLczDecontaminated)) return;

            Player player = Player.List.FirstOrDefault(x => x.Role == RoleTypeId.Spectator && x.UserId != string.Empty && x.UserId != ev.Player.UserId && !x.IsOverwatchEnabled);

            if (player != null)
            {
                player.Role.Set(ev.Player.Role);

                // save info
                Vector3 pos = ev.Player.Position;
                var inventory = ev.Player.Items.ToList();

                float health = ev.Player.Health;
                float ahealth = ev.Player.ArtificialHealth;

                ushort ammo1 = ev.Player.GetAmmo(AmmoType.Nato9);
                ushort ammo2 = ev.Player.GetAmmo(AmmoType.Nato556);
                ushort ammo3 = ev.Player.GetAmmo(AmmoType.Nato762);
                ushort ammo4 = ev.Player.GetAmmo(AmmoType.Ammo12Gauge);
                ushort ammo5 = ev.Player.GetAmmo(AmmoType.Ammo44Cal);

                Timing.CallDelayed(0.3f, () =>
                {
                    player.Position = pos;
                    player.ClearInventory();
                    player.ResetInventory(inventory);

                    player.Health = health;
                    player.ArtificialHealth = ahealth;

                    player.AddAmmo(AmmoType.Nato9, ammo1);
                    player.AddAmmo(AmmoType.Nato556, ammo2);
                    player.AddAmmo(AmmoType.Nato762, ammo3);
                    player.AddAmmo(AmmoType.Ammo12Gauge, ammo4);
                    player.AddAmmo(AmmoType.Ammo44Cal, ammo5);

                    string message = config.ReplacedBroadcast;

                    ushort time = config.ReplacedBroadcastTime;


                    player.Broadcast(time, message);

                });
            }
        }
    }
}
