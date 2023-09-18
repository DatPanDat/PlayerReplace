using MEC;
using System.Linq;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using Exiled.API.Features.Roles;
using System.Collections.Generic;
using CustomPlayerEffects;
using Exiled.API.Features.Items;
using RemoteAdmin.Communication;

namespace PlayerReplace.EventHandler
{
    public class EventHandlers
    {

        private bool isRoundStarted = false;
        public void OnRoundStart() //Check if the round has started
        {
            isRoundStarted = true;
        }

        public void OnRoundEnd() => isRoundStarted = false; //If the round has ended, toggle replace off.

        public void OnLeft(LeftEventArgs evnt)
        {

            bool isExclused = false; //for checking later if the player was exclused or not

            if (!isRoundStarted || PlayerReplace.Instance.Config.RestrictedRoles.Contains(evnt.Player.Role) || evnt.Player.Position.y < -1997 || (evnt.Player.CurrentRoom.Zone == ZoneType.LightContainment && Exiled.API.Features.Map.IsLczDecontaminated))
                {
                    isExclused = true;
                    Log.Debug("Exclusion checks triggered, skipping replacer...");
                    return;
                }

            Log.Debug("Exclusion not triggered, proceeding...");

            Vector3 pos = evnt.Player.Position;
            Quaternion rot = evnt.Player.Rotation;//get player pos and rotation

            IEnumerable<Exiled.API.Features.Items.Item> items = evnt.Player.Items;//saving current inventory
            Dictionary<ItemType, ushort> ammoAndAmount = evnt.Player.Ammo;//saving current ammo(is out here for when replacement not found)

            Exiled.API.Features.Player newPlayer = Exiled.API.Features.Player.List.FirstOrDefault(x => x.Role == RoleTypeId.Spectator && x.UserId != string.Empty && x.UserId != evnt.Player.UserId && !x.IsOverwatchEnabled);

            if (newPlayer != null)
            {
                Log.Debug("Successfully gotten replacement...");

                newPlayer.Role.Set(evnt.Player.Role, RoleSpawnFlags.None);//respawn the player

                float health = evnt.Player.Health;
                float ahealth = evnt.Player.ArtificialHealth;
                float hshield = evnt.Player.HumeShield;//save current hp, ahp, hs

                ushort ammo1 = evnt.Player.GetAmmo(AmmoType.Nato9);
                ushort ammo2 = evnt.Player.GetAmmo(AmmoType.Nato556);
                ushort ammo3 = evnt.Player.GetAmmo(AmmoType.Nato762);
                ushort ammo4 = evnt.Player.GetAmmo(AmmoType.Ammo12Gauge);
                ushort ammo5 = evnt.Player.GetAmmo(AmmoType.Ammo44Cal);//fuck you, wack ass ammo getting

                IEnumerable<StatusEffectBase> effs = evnt.Player.ActiveEffects;//save all status effects

                //declaring for scps
                int Exp079 = 0;
                float Ap079 = 0f, Vigor106 = 0f;
                Exiled.API.Features.Camera Room079 = null;

                if (evnt.Player.Role == RoleTypeId.Scp079)//Check 079 location xp and ap
                {
                    Log.Debug("SCP-079 Detected");
                    Exp079 = evnt.Player.Role.As<Scp079Role>().Experience;
                    Ap079 = evnt.Player.Role.As<Scp079Role>().Energy;
                    Room079 = evnt.Player.Role.As<Scp079Role>().Camera;
                }

                if (evnt.Player.Role == RoleTypeId.Scp106)//check 106 vigor
                {
                    Log.Debug("SCP-106 Detected");
                    Vigor106 = evnt.Player.Role.As<Scp106Role>().Vigor;
                }
                
                Timing.CallDelayed(0.3f, () =>
                {
                    newPlayer.Position = pos;
                    newPlayer.Rotation = rot;//Position, rotation

                    newPlayer.Health = health;
                    newPlayer.ArtificialHealth = ahealth;
                    newPlayer.HumeShield = hshield;//HP, AHP, HS

                    if (evnt.Player.Role == RoleTypeId.Scp079)//if 079, take them to correct room with right amount of xp and ap
                    {
                        newPlayer.Role.As<Scp079Role>().Experience = Exp079;
                        newPlayer.Role.As<Scp079Role>().Energy = Ap079;
                        newPlayer.Role.As<Scp079Role>().Camera = Room079;
                    }

                    if (evnt.Player.Role == RoleTypeId.Scp106)//if 106, give the right amount of vigor
                    {
                        newPlayer.Role.As<Scp106Role>().Vigor = Vigor106;
                    }

                    foreach (Exiled.API.Features.Items.Item item in items)//Inventory giving
                    {
                        if (item is Armor == true)
                        {
                            Log.Debug("Armor detected. Doing some weird shit...");
                            newPlayer.AddItem(item.Type);
                        }

                        else
                        {
                            newPlayer.AddItem(item);
                        }
                    }

                    newPlayer.SetAmmo(AmmoType.Nato9, ammo1);
                    newPlayer.SetAmmo(AmmoType.Nato556, ammo2);
                    newPlayer.SetAmmo(AmmoType.Nato762, ammo3);
                    newPlayer.SetAmmo(AmmoType.Ammo12Gauge, ammo4);
                    newPlayer.SetAmmo(AmmoType.Ammo44Cal, ammo5);//wack ass ammo giving

                    foreach (StatusEffectBase effect in effs)//Status effects giving
                    {
                        newPlayer.EnableEffect(effect);
                    }

                    newPlayer.Broadcast(PlayerReplace.Instance.Config.ReplacedMessageTime, PlayerReplace.Instance.Config.ReplacedMessage);//broadcast to the replacement
                    Log.Debug("Replacement completed successfully");


                });
            }

            if (newPlayer == null && !isExclused)//check if new player was not found AND wasnt exclused
            {
                Log.Debug("No replacement found...");
                Ragdoll.CreateAndSpawn(evnt.Player.Role, evnt.Player.Nickname, PlayerReplace.Instance.Config.DCDeathReason, pos, rot);//spawn a corpse in their place
                foreach (Exiled.API.Features.Items.Item item in items) item.CreatePickup(pos, rot);//dropping the items they had
                foreach (ItemType ammo in ammoAndAmount.Keys) Exiled.API.Features.Items.Item.Create(ammo).CreatePickup(pos, rot);//dropping their ammos
                string FormatNonreplaceMessage = PlayerReplace.Instance.Config.NonReplaceMessage.Replace("%oldPlayerName%", evnt.Player.Nickname).Replace("%oldRole%", evnt.Player.Role.Type.ToString());//Format messages with dc player infos
                Exiled.API.Features.Map.Broadcast(PlayerReplace.Instance.Config.NonReplaceMessageTime, FormatNonreplaceMessage);//broadcast to the entire server about it

            }
        }

    }
}
