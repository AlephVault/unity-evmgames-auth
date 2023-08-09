using AlephVault.Unity.EVMGames.Auth.Protocols;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Samples
    {
        [RequireComponent(typeof(Button))]
        public class DisconnectButton : MonoBehaviour
        {
            private Button button;

            [SerializeField]
            private NetworkClient client;
            
            private void Awake()
            {
                button = GetComponent<Button>();
            }

            private void Start()
            {
                button.onClick.AddListener(DisconnectButton_Click);
            }

            private void OnDestroy()
            {
                button.onClick.RemoveListener(DisconnectButton_Click);
            }

            private void DisconnectButton_Click()
            {
                client.GetComponent<IEVMAuthProtocolClientSide>().Logout();
            }
        }
    }
}