using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Auth.Types;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Types
    {
        /// <summary>
        ///   EVM Account preview data objects include, at least,
        ///   the address that will serve as login handler (in
        ///   place of "username" in usual login systems). This,
        ///   in addition to whatever preview they use.
        /// </summary>
        public interface IEVMRecordWithPreview<IDType, ProfileDisplayDataType> : IRecordWithPreview<
            IDType, ProfileDisplayDataType
        >
            where ProfileDisplayDataType : ISerializable
        {
            /// <summary>
            ///   The address this account relates to.
            /// </summary>
            public uint LastLoginTimestamp();
        }
    }
}