using AlephVault.Unity.Binary;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.Meetgard.Auth.Types;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Protocols
    {
        public class EVMAuthProtocolDefinition<LoginOK, LoginFailed, Kicked> : SimpleAuthProtocolDefinition<
            LoginOK, LoginFailed, Kicked
        >
            where LoginOK : ISerializable, new()
            where LoginFailed : IEVMLoginFailed<LoginFailed>, new()
            where Kicked : IKickMessage<Kicked>, new()
        {
            protected override void DefineLoginMessages()
            {
                DefineLoginMessage<EVMLoginMessage>("EVM");
            }
        }
    }
}