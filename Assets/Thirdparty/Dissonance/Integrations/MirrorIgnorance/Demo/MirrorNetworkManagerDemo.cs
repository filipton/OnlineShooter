using Mirror;

namespace Dissonance.Integrations.MirrorIgnorance.Demo
{
    class MirrorNetworkManagerDemo
        : NetworkManager
    {
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            MirrorIgnoranceServer.ForceDisconnectClient(conn);
        }
    }
}
