using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Unity;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.Meetgard.Auth.Types;
using AlephVault.Unity.Meetgard.Types;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Protocols
    {
        /// <summary>
        ///   A protocol client side to perform an EVM login (this
        ///   means: a login attempt linked to cryptographic signing
        ///   processes, intended to be compatible with any EVM-like
        ///   network, like Ethereum, Polygon or BSC). This process
        ///   is meant to be fed with a unix UTC timestamp (as close
        ///   as possible) and the signature of that timestamp, by
        ///   using, typically, WalletConnect).
        /// </summary>
        /// <typeparam name="LoginFailed">A custom message for when the login fails</typeparam>
        /// <typeparam name="Kicked">A custom message for when a user is kicked</typeparam>
        public class EVMAuthProtocolClientSide<LoginFailed, Kicked> : SimpleAuthProtocolClientSide<
            EVMAuthProtocolDefinition<Nothing, LoginFailed, Kicked>, Nothing, LoginFailed, Kicked
        >, IEVMAuthProtocolClientSide
            where LoginFailed : IEVMLoginFailed<LoginFailed>, new()
            where Kicked : IKickMessage<Kicked>, new()
        {
            // A linked WalletConnect instance, if any.
            private WalletConnect walletConnect;
            
            /// <summary>
            ///   Tells whether, when also having the
            ///   <see cref="WalletConnect"/> behaviour
            ///   attached, its session will be closed
            ///   when the logged session is also logged
            ///   out (via a LoggedOut event).
            /// </summary>
            [SerializeField]
            private bool managesWalletConnectSession = true;

            /// <summary>
            ///   The signature to use as login data. This
            ///   signature is not empty, and related to
            ///   the given <see cref="Timestamp"/>.
            /// </summary>
            public string Signature { get; set; }

            /// <summary>
            ///   The timestamp the signature stands for.
            ///   This timestamp must be recent (according
            ///   to what the server side understand as a
            ///   "recent" timestamp, typically in a range
            ///   of [server's now() +/- 120 secs]).
            /// </summary>
            public uint Timestamp { get; set; }
            
            // This pointer will be used to perform the callback.
            private Func<EVMLoginMessage, Task> SendEVMLogin;
            
            /// <summary>
            ///   It only defines the Login:EVM message.
            /// </summary>
            protected override void MakeLoginRequestSenders()
            {
                SendEVMLogin = MakeLoginRequestSender<EVMLoginMessage>("EVM");
            }

            protected new void Awake()
            {
                walletConnect = GetComponent<WalletConnect>();
                base.Awake();
                Handshake.OnWelcome += EVMAuthProtocolClientSide_OnWelcome;
                OnLoggedOut += EVMAuthProtocolClientSide_OnLoggedOut;
            }

            private async Task EVMAuthProtocolClientSide_OnLoggedOut()
            {
                if (managesWalletConnectSession)
                {
                    walletConnect.CloseSession(false);
                }
            }

            private async Task EVMAuthProtocolClientSide_OnWelcome()
            {
                await SendEVMLogin(new EVMLoginMessage()
                {
                    Signature = Signature,
                    Timestamp = Timestamp
                });
            }

            protected void OnDestroy()
            {
                Handshake.OnWelcome -= EVMAuthProtocolClientSide_OnWelcome;
                OnLoggedOut -= EVMAuthProtocolClientSide_OnLoggedOut;
            }
        }
    }
}