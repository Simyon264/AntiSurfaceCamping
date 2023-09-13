using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Linq;
using System.Text;

namespace AntiSurfaceCamping
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class GetTimeCommand : ICommand
    {
        public string Command { get; } = "gettime";
        public string[] Aliases { get; } = new string[] { "atime" };
        public string Description { get; } = "Get your current AntiSurfaceCamp time.";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(((CommandSender)sender).SenderId);
            AntiSurface antiSurface = player.GameObject.GetComponent<AntiSurface>();
            if (antiSurface != null)
            {
                response = $"Your current AntiSurfaceCamp time is {antiSurface.SurfaceTime} seconds.";
                return true;
            } else
            {
                response = "You don't have an AntiSurfaceCamp component.";
                return false;
            }
        }
    }
}
