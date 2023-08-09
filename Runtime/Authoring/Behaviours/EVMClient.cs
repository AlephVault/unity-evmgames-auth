using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Auth.Protocols;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.EVMGames.Nethereum.Web3;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using UnityEditor.PackageManager;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   An EVM client is a complete interface with the
            ///   user (save for any external provider, e.g. like
            ///   WalletConnect) both hitting an EVM network and
            ///   an authenticated game server.
            /// </summary>
            [RequireComponent(typeof(NetworkClient))]
            public class EVMClient : MonoBehaviour
            {
                // The related network client.
                private NetworkClient networkClient;

                // The related auth protocol.
                private IEVMAuthProtocolClientSide authProtocol;

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
                
                /// <summary>
                ///   The current Web3 (valid) address to interact
                ///   with the EVM network.
                /// </summary>
                public string CurrentWeb3Address { get; private set; }

                private void Awake()
                {
                    networkClient = GetComponent<NetworkClient>();
                    authProtocol = GetComponent<IEVMAuthProtocolClientSide>();
                    if (authProtocol == null)
                    {
                        throw new Exception("No EVM auth protocol client side is attached to this object");
                    }

                    authProtocol.OnEVMClientDisconnected += OnEVMClientDisconnected;
                }

                private void OnDestroy()
                {
                    authProtocol.OnEVMClientDisconnected -= OnEVMClientDisconnected;
                }

                // Clears the current EVM settings on this client.
                private async Task OnEVMClientDisconnected(Exception e)
                {
                    CurrentWeb3Client = null;
                    CurrentWeb3Address = "";
                }
                
                /// <summary>
                ///   Performs a complete life-cycle of a login,
                ///   including the signature with Web3 (using a
                ///   client that can sign!).
                /// </summary>
                /// <param name="web3">The Web3 client to use. Must have sign capabilities</param>
                /// <param name="address">The address to use (both as account and signing)</param>
                public async Task WalletLogin(Web3 web3, string address)
                {
                    if (web3 == null)
                    {
                        throw new ArgumentException(nameof(web3));
                    }

                    if (string.IsNullOrEmpty(address))
                    {
                        throw new ArgumentException(nameof(address));
                    }
                    
                    // It is an error to try this when there is an already
                    // used web3 client on this client.
                    if (CurrentWeb3Client != null)
                    {
                        throw new InvalidOperationException("Cannot do a Wallet Login cycle when a " +
                                                            "Web3 client is attached to it");
                    }

                    // It is an error to try this unless there is
                    // a network client and that network client is
                    // not already connected.
                    if (networkClient.IsRunning)
                    {
                        throw new InvalidOperationException("Cannot do a Web3 Login when there is an already " +
                                                            "established collection configured network client, or " +
                                                            "there is one but is already running");
                    }

                    // The first thing is to generate the challenge,
                    // expressed in seconds. Its string representation
                    // will be signed later.
                    uint timestamp = ChallengeUtils.CurrentTimestamp();
                    string challenge = ChallengeUtils.TimestampChallengeMessage(timestamp);
                    
                    // The next thing is to obtain a signature. An IO
                    // Error should be captured, since it means that
                    // the signature process failed or was rejected.
                    string signature = await web3.Eth.Sign.SendRequestAsync(address, challenge);

                    // Now, both things are taken and the login is attempted.
                    authProtocol.Signature = signature;
                    authProtocol.Timestamp = timestamp;
                    networkClient.Connect(Host, Port);
                    CurrentWeb3Address = address;
                    CurrentWeb3Client = web3;
                }

                /// <summary>
                ///   Forces an EVM logout.
                /// </summary>
                public Task Logout()
                {
                    return authProtocol.Logout();
                }
            }
        }
    }
}