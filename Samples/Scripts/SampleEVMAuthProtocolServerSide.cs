using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Auth.Protocols;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.Meetgard.Auth.Samples;
using AlephVault.Unity.Meetgard.Auth.Types;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Samples
    {
        public class SampleEVMAuthProtocolServerSide : EVMAuthProtocolServerSide<
            EVMLoginFailed, Kicked, SampleAccountPreviewDataType, SampleAccountDataType
        >
        {
            /// <summary>
            ///   The sample accounts.
            /// </summary>
            [SerializeField]
            private SampleAccountDataType[] accounts;
            
            protected override void Setup()
            {
                base.Setup();
                OnSessionStarting += SampleEVMAuthProtocolServerSide_OnSessionStarting;
                OnSessionTerminating += SampleEVMAuthProtocolServerSide_OnSessionTerminating;
            }

            private async Task SampleEVMAuthProtocolServerSide_OnSessionTerminating(ulong arg1, Kicked arg2)
            {
                Debug.Log($"EVM Auth Server::Session terminating for connection {arg1}");
            }

            private async Task SampleEVMAuthProtocolServerSide_OnSessionStarting(ulong arg1, SampleAccountDataType arg2)
            {
                Debug.Log($"EVM Auth Server::Session starting for connection {arg1} - {arg2.GetID()}");
            }

            protected void OnDestroy()
            {
                OnSessionStarting -= SampleEVMAuthProtocolServerSide_OnSessionStarting;
                OnSessionTerminating -= SampleEVMAuthProtocolServerSide_OnSessionTerminating;
            }

            protected override async Task<SampleAccountDataType> FindAccount(string id)
            {
                foreach (SampleAccountDataType account in accounts)
                {
                    if (account.GetID() == id) return account;
                }

                return null;
            }

            protected override AccountAlreadyLoggedManagementMode IfAccountAlreadyLoggedIn()
            {
                return AccountAlreadyLoggedManagementMode.Ghost;
            }

            protected override async Task OnSessionError(ulong clientId, SessionStage stage, Exception error)
            {
                Debug.Log("EVM Auth Server::Session error");
            }

            protected override async Task SetLastLoginTime(string address, uint timestamp)
            {
                Debug.Log($"EVM Auth Server::New login timestamp: {timestamp}");
                (await FindAccount(address)).UpdateLastLoginTimestamp(timestamp);
            }
        }
    }
}