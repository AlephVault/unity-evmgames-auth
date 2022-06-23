using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Auth.Protocols;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.EVMGames.Nethereum.Web3;
using AlephVault.Unity.EVMGames.WalletConnectSharp.NEthereum;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Unity;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   An EVM client is a complete interface with the
            ///   user (save for the required QR Image, which comes
            ///   apart) tied to a <see cref="walletConnect"/>
            ///   instance which will guide the whole lifecycle of
            ///   a Wallet Connect session and the game itself.
            /// </summary>
            [RequireComponent(typeof(NetworkClient))]
            [RequireComponent(typeof(WalletConnect))]
            public class EVMClient : MonoBehaviour
            {
                // The instance for this client.
                private WalletConnect walletConnect;

                // The related network client.
                private NetworkClient networkClient;

                /// <summary>
                ///   Tells whether the client connection (and
                ///   thus the login protocol) is automatically
                ///   managed by the session lifecycle (i.e.
                ///   auto-login on session connect, auto-close
                ///   on session disconnect).
                /// </summary>
                [SerializeField]
                private bool managesClientConnection = true;

                /// <summary>
                ///   This URL will be paid attention to when
                ///   successfully completing a new session
                ///   handshake: it will be used to create a
                ///   new Web3 client (using the instance's
                ///   current session as provider). It will
                ///   be an error to provide an empty URL.
                /// </summary>
                public string EVMGatewayURL;

                /// <summary>
                ///   The host to connect to (i.e. the game or
                ///   application host).
                /// </summary>
                public string Host;
                
                /// <summary>
                ///   The port to connect to (i.e. the game or
                ///   application port).
                /// </summary>
                public ushort Port;

                /// <summary>
                ///   The current Web3 client to interact with
                ///   the EVM network.
                /// </summary>
                public Web3 CurrentWeb3Client { get; private set; }

                private void Awake()
                {
                    walletConnect = GetComponent<WalletConnect>();
                    networkClient = GetComponent<NetworkClient>();
                }

                private void Start()
                {
                    walletConnect.DisconnectedEvent.AddListener(DisconnectedEvent);
                    walletConnect.ConnectedEvent.AddListener(ConnectedEvent);
                }

                private void OnDestroy()
                {
                    walletConnect.DisconnectedEvent.RemoveListener(DisconnectedEvent);
                    walletConnect.ConnectedEvent.RemoveListener(ConnectedEvent);
                }

                private async void DisconnectedEvent(WalletConnectUnitySession session)
                {
                    // 1. Clear the current web3 client.
                    CurrentWeb3Client = null;
                    
                    // 2. Close the current connection, if
                    //    told to.
                    if (managesClientConnection && networkClient.IsRunning)
                    {
                        await networkClient.GetComponent<IEVMAuthProtocolClientSide>().Logout();
                    }
                }
                
                private async void ConnectedEvent()
                {
                    // 1. Creates a new Web3 client.
                    CurrentWeb3Client = new Web3(walletConnect.Session.CreateProvider(
                        new Uri(EVMGatewayURL)
                    ));
                    
                    // 2. Attempts a login, if it is told to
                    //    automatically connect the network
                    //    client and do EVM login.
                    // 3. Close the current connection, if
                    //    told to.
                    if (managesClientConnection && !networkClient.IsRunning)
                    {
                        await WalletLogin();
                    }
                }

                /// <summary>
                ///   Performs a complete life-cycle of a login,
                ///   including the signature with Wallet Connect.
                ///   This method must be called accordingly if
                ///   the login failed a former time (and a custom
                ///   implementation of the EVM Auth Protocol Client
                ///   Side does not disconnect the WalletConnect
                ///   session on itself), or if there is no automatic
                ///   login management checked on in this component.
                /// </summary>
                public async Task WalletLogin()
                {
                    // It is an error to try this when there is no
                    // current web3 client / there is no session.
                    if (CurrentWeb3Client == null)
                    {
                        throw new InvalidOperationException("Cannot do a Wallet Login cycle when no " +
                                                            "WalletConnect session is established");
                    }

                    // It is an error to try this unless there is
                    // a network client and that network client is
                    // not already connected.
                    if (networkClient.IsRunning)
                    {
                        throw new InvalidOperationException("Cannot do a Wallet Login when there is " +
                                                            "an already established collectiono configured network client, or there is one " +
                                                            "but is already running");
                    }

                    // The first thing is to generate the challenge,
                    // expressed in seconds. Its string representation
                    // will be signed later.
                    uint timestamp = ChallengeUtils.CurrentTimestamp();
                    string challenge = ChallengeUtils.TimestampChallengeMessage(timestamp);
                    
                    // The next thing is to obtain a signature. An IO
                    // Error should be captured, since it means that
                    // the signature process failed or was rejected.
                    string signature = await walletConnect.Session.EthSign(
                        walletConnect.Session.Accounts[0], challenge
                    );
                    
                    // Now, both things are taken and the login is attempted.
                    IEVMAuthProtocolClientSide protocol = networkClient.GetComponent<IEVMAuthProtocolClientSide>();
                    if (protocol == default)
                    {
                        throw new InvalidOperationException("Cannot do a Wallet Login when there is " +
                                                            "no EVM login behaviour in the current client");
                    }
                    protocol.Signature = signature;
                    protocol.Timestamp = timestamp;
                    networkClient.Connect(Host, Port);
                }
            }
        }
    }
}