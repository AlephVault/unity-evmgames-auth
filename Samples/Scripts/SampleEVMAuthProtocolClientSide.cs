using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Auth.Protocols;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.Meetgard.Auth.Samples;
using AlephVault.Unity.Meetgard.Types;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Samples
    {
        public class SampleEVMAuthProtocolClientSide : EVMAuthProtocolClientSide<EVMLoginFailed, Kicked>
        {
            protected override void Setup()
            {
                base.Setup();
                OnLoginOK += SampleEVMAuthProtocolClientSide_OnLoginOK;
                OnLoggedOut += SampleEVMAuthProtocolClientSide_OnLoggedOut;
                OnLoginFailed += SampleEVMAuthProtocolClientSide_OnLoginFailed;
            }
            
            protected void OnDestroy()
            {
                OnLoginOK -= SampleEVMAuthProtocolClientSide_OnLoginOK;
                OnLoggedOut -= SampleEVMAuthProtocolClientSide_OnLoggedOut;
                OnLoginFailed -= SampleEVMAuthProtocolClientSide_OnLoginFailed;
            }

            private async Task SampleEVMAuthProtocolClientSide_OnLoginOK(Nothing arg)
            {
                Debug.Log("Sample EVM Client::Logged in");
            }
            
            private async Task SampleEVMAuthProtocolClientSide_OnLoggedOut()
            {
                Debug.Log("Sample EVM Client::Logged out");
            }
            
            private async Task SampleEVMAuthProtocolClientSide_OnLoginFailed(EVMLoginFailed arg)
            {
                Debug.Log($"Sample EVM Client::Login failed: {arg.Reason}");
            }
        }
    }
}