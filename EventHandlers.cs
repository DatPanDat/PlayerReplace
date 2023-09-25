using MEC;
using System.Linq;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Roles;
using System.Collections.Generic;
using CustomPlayerEffects;
using Exiled.API.Features.Items;
using PlayerReplace.API.Features.ExternalRoles.Enums;

namespace PlayerReplace.EventHandler
{
    public class EventHandlers
    {
        public PlayerReplace plugin;
        
        private bool isRoundStarted = false;
        public bool CanReplace = true;
        
        public void OnRoundStart() //Check if the round has started
        {
            isRoundStarted = true;
        }

        public void OnRoundEnd()
        {
            isRoundStarted = false;
        }

        public void OnLeft(LeftEventArgs ev)//THE DISCONNECTED REPLACING
        {
            
            if (!CanReplace)
            {
                Log.Debug("DC: Replacement disabled, skipping...");
                return;
            }
            
            bool isExternalRole = API.API.IsExternalRole(ev.Player);
            bool isExclused = false; //for checking later if the player was exclused or not

            if (!Round.InProgress || !isRoundStarted || PlayerReplace.Instance.Config.RestrictedRoles.Contains(ev.Player.Role) || ev.Player.Position.y < -1997 || (ev.Player.CurrentRoom.Zone == ZoneType.LightContainment && Map.IsLczDecontaminated))
            {
                isExclused = true;
                Log.Debug("DC: Exclusion checks triggered, skipping replacer...");
                return;
            }

            Log.Debug("DC: Exclusion not triggered, proceeding...");

            Vector3 pos = ev.Player.Position;
            Quaternion rot = ev.Player.Rotation;//get player pos and rotation

            IEnumerable<Item> items = ev.Player.Items;//saving current inventory
            Dictionary<ItemType, ushort> ammoAndAmount = ev.Player.Ammo;//saving current ammo(is out here for when replacement not found)
           
            List<Player> specPlayers = new();
            Player newPlayer = null;

            foreach (var player in Player.List)
            {
                if (player.Role != RoleTypeId.Spectator)
                    continue;
                specPlayers.Add(player);
            }

            if (specPlayers.Count == 0)
            {
                Log.Debug("No spectators found...");
            }

            if (specPlayers.Count != 0) newPlayer = specPlayers[Random.Range(0, specPlayers.Count - 1)];
            
            
            if (newPlayer != null)
            {
                Log.Debug("DC: Successfully gotten replacement...");
                
                switch (API.API.GetExternalRole(ev.Player))
                {
                    case ExternalRoleType.CiSpy:
                        Log.Debug("DC: Player is a Spy, replacing...");
                        API.API.CiSpyRole.SpawnRole(ev.Player, newPlayer);
                        break;
                    default:
                        Log.Debug("DC: Player does not have a valid external role, setting up as normal role");
                        newPlayer.Role.Set(ev.Player.Role, RoleSpawnFlags.None);//respawn the player
                        break;
                }
                

                float health = ev.Player.Health;
                float ahealth = ev.Player.ArtificialHealth;
                float hshield = ev.Player.HumeShield;//save current hp, ahp, hs

                ushort ammo1 = ev.Player.GetAmmo(AmmoType.Nato9);
                ushort ammo2 = ev.Player.GetAmmo(AmmoType.Nato556);
                ushort ammo3 = ev.Player.GetAmmo(AmmoType.Nato762);
                ushort ammo4 = ev.Player.GetAmmo(AmmoType.Ammo12Gauge);
                ushort ammo5 = ev.Player.GetAmmo(AmmoType.Ammo44Cal);//fuck you, wack ass ammo getting

                IEnumerable<StatusEffectBase> effs = ev.Player.ActiveEffects;//save all status effects

                //declaring for scps
                int Exp079 = 0;
                float Ap079 = 0f, Vigor106 = 0f;
                Exiled.API.Features.Camera Room079 = null;

                if (!isExternalRole)
                {
                    if (ev.Player.Role == RoleTypeId.Scp079) //Check 079 location xp and ap
                    {
                        Log.Debug("DC: SCP-079 Detected");
                        Exp079 = ev.Player.Role.As<Scp079Role>().Experience;
                        Ap079 = ev.Player.Role.As<Scp079Role>().Energy;
                        Room079 = ev.Player.Role.As<Scp079Role>().Camera;
                    }

                    if (ev.Player.Role == RoleTypeId.Scp106) //check 106 vigor
                    {
                        Log.Debug("DC: SCP-106 Detected");
                        Vigor106 = ev.Player.Role.As<Scp106Role>().Vigor;
                    }
                }

                Timing.CallDelayed(0.3f, () =>
                {
                    newPlayer.Position = pos;
                    newPlayer.Rotation = rot;//Position, rotation

                    newPlayer.Health = health;
                    newPlayer.ArtificialHealth = ahealth;
                    newPlayer.HumeShield = hshield;//HP, AHP, HS

                    if (!isExternalRole)
                    {
                        if (newPlayer.Role == RoleTypeId.Scp079) //if 079, take them to correct room with right amount of xp and ap
                        {
                            newPlayer.Role.As<Scp079Role>().Experience = Exp079;
                            newPlayer.Role.As<Scp079Role>().Energy = Ap079;
                            newPlayer.Role.As<Scp079Role>().Camera = Room079;
                        }

                        if (newPlayer.Role == RoleTypeId.Scp106) //if 106, give the right amount of vigor
                        {
                            newPlayer.Role.As<Scp106Role>().Vigor = Vigor106;
                        }
                    }

                    foreach (Item item in items)//Inventory giving
                    {
                        if (item is Armor == true)
                        {
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
                    Log.Debug("DC: Replacement completed successfully");


                });
            }

            if (newPlayer == null && !isExclused)//check if new player was not found AND wasnt exclused
            {
                Log.Debug("DC: No replacement found...");
                Ragdoll.CreateAndSpawn(ev.Player.Role, ev.Player.Nickname, PlayerReplace.Instance.Config.DCDeathReason, pos, rot);//spawn a corpse in their place
                foreach (Item item in items) item.CreatePickup(pos, rot);//dropping the items they had
                foreach (ItemType ammo in ammoAndAmount.Keys) Item.Create(ammo).CreatePickup(pos, rot);//dropping their ammos not working lmaoooooo

                if (PlayerReplace.Instance.Config.NoReplaceMessage != "")
                {
                    string FormatNonreplaceMessage = PlayerReplace.Instance.Config.NoReplaceMessage.Replace("%oldPlayerName%", ev.Player.Nickname).Replace("%oldRole%", ev.Player.Role.Type.ToString()); //Format messages with dc player infos
                    Map.Broadcast(PlayerReplace.Instance.Config.NoReplaceMessageTime, FormatNonreplaceMessage);//broadcast to the entire server about it
                }
            }
        }


        
    }
}
