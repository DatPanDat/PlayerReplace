using Exiled.API.Features;
using System;
using Exiled.API.Enums;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;

using PlayerReplace.EventHandler;

namespace PlayerReplace
{
    
    public class PlayerReplace : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public override string Author { get; } = "@Cyanox62, \"\"\"Improved\"\"\" by @DatPanDat, actually got it to work by @User_NonExist";
        public override string Name { get; } = "PlayerReplace";
        public override string Prefix { get; } = "PlayerReplace";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 2, 1);

        public static PlayerReplace Instance;

        public EventHandlers evnt;

        public override void OnEnabled()
        {
            Instance = this;


            evnt = new EventHandlers();

            Player.Left += evnt.OnLeft;
            Server.RoundStarted += evnt.OnRoundStart;
        }

        public override void OnDisabled()
        {
            Instance = null;

            Player.Left -= evnt.OnLeft;
            Server.RoundStarted -= evnt.OnRoundStart;

            evnt = null;
        }

    }
}
