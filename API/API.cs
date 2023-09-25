using System;
using Exiled.API.Features;
using PlayerReplace.API.Features.ExternalRoles;
using PlayerReplace.API.Features.ExternalRoles.Enums;

namespace PlayerReplace.API;

public static class API
{
    public static Func<Player, bool> IsSpy => CiSpyRole.IsRole;
    public static Action<Player, Player> SpawnRole => CiSpyRole.SpawnRole;
    
    public static void TogglePlayerReplace(bool enabled, string pluginName = null)
    {
        if (pluginName != null)
            Log.Debug($"DC: Incoming request from {pluginName} to toggle PlayerReplace.");
        else
            Log.Debug($"DC: Incoming request from a plugin to toggle PlayerReplace.");
        
        if (enabled)
        {
            PlayerReplace.Instance.ev.CanReplace = true;
            Log.Debug("DC: PlayerReplace is now enabled.");
        }
        else
        {
            PlayerReplace.Instance.ev.CanReplace = false;
            Log.Debug("DC: PlayerReplace is now disabled.");
        }
    }

    public static bool IsExternalRole(Player player)
    {
        if (CiSpyRole.IsRole(player))
            return true;
        
        return false;
    }
    
    public static ExternalRoleType GetExternalRole(Player player)
    {
        if (CiSpyRole.IsRole(player))
            return ExternalRoleType.CiSpy;
        
        return ExternalRoleType.None;
    }

    internal static readonly ExternalRoleChecker CiSpyRole = new CiSpyRole();
}