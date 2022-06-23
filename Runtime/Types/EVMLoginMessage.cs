using AlephVault.Unity.Binary;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Types
    {
        /// <summary>
        ///   A login message involving a crypto wallet.
        ///   This message has two parts, actually: One
        ///   integer value to serve as timestamp, and
        ///   one integer value to serve as signature
        ///   of that timestamp. If the signature is
        ///   valid, the address will be inferred out
        ///   of it (from the public key).
        /// </summary>
        public class EVMLoginMessage : ISerializable
        {
            /// <summary>
            ///   The signature of the timestamp used
            ///   as login challenge.
            /// </summary>
            public string Signature;
            
            /// <summary>
            ///   The timestamp used as login challenge,
            ///   which will be the signature's source.
            /// </summary>
            public uint Timestamp;

            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref Timestamp);
                serializer.Serialize(ref Signature);
            }
        }
    }
}