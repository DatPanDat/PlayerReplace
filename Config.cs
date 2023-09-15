namespace PlayerReplace
{
    using Exiled.API.Interfaces;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class Config : IConfig
    {
        [Description("If the plugin is enabled or not. (Make sure your disconnect_drop is set to false!)")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debugging? [Currently unutilized]")]
        public bool Debug { get; set; } = false;

        [Description("The roles to not attempt replacing.")]
        public List<RoleTypeId> RestrictedRoles { get; set; } = new()
    {
        RoleTypeId.Spectator,
        RoleTypeId.Tutorial,
        RoleTypeId.Overwatch,
        RoleTypeId.Filmmaker,
    };

        [Description("The text displayed to the player after replacing.")]
        public string ReplacedBroadcast { get; set; } = "<i>You have replaced a player who has disconnected.</i>";

        [Description("The duration of broadcast above.")]
        public ushort ReplacedBroadcastTime { get; set; } = 5;
    }
}