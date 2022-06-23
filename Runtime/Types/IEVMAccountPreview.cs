using AlephVault.Unity.Binary;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Types
    {
        /// <summary>
        ///   EVM Account preview data objects include, at least,
        ///   the address that will serve as login handler (in
        ///   place of "username" in usual login systems).
        /// </summary>
        public interface IEVMAccountPreviewData : ISerializable
        {
            /// <summary>
            ///   The address this account relates to.
            /// </summary>
            public string Address { get; set; }
        }
    }
}