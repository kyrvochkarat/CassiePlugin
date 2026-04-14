using System;
using Exiled.API.Features;

namespace CassiePlugin
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "CassiePlugin";
        public override string Author => "vityanvsk";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 0, 0);

        private EventHandlers eventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            eventHandlers = new EventHandlers(this);
            
            RegisterEvents();
            
            Log.Info($"{Name} v{Version} загружен!");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            eventHandlers = null;
            Instance = null;
            
            Log.Info($"{Name} выгружен!");
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += eventHandlers.OnRespawningTeam;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage += eventHandlers.OnSendingCassieMessage;
            Exiled.Events.Handlers.Server.RoundStarted += eventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += eventHandlers.OnRoundEnded;
        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= eventHandlers.OnRespawningTeam;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage -= eventHandlers.OnSendingCassieMessage;
            Exiled.Events.Handlers.Server.RoundStarted -= eventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= eventHandlers.OnRoundEnded;
        }
    }
}
