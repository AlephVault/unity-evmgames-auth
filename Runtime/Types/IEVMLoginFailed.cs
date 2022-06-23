using AlephVault.Unity.Binary;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Types
    {
        /// <summary>
        ///   This interface provides methods to hydrate
        ///   the object as one of the standard login failed
        ///   messages for this EVM/crypto login attempts.
        /// </summary>
        public interface IEVMLoginFailed<T> : ISerializable where T : IEVMLoginFailed<T>
        {
            /// <summary>
            ///   Updates the login failed reason to use a message
            ///   telling that the signature was not valid (against
            ///   the provided message).
            /// </summary>
            /// <returns>The same object</returns>
            public T WithInvalidSignatureReason();

            /// <summary>
            ///   Updates the login failed reason to use a message
            ///   telling that the account was not found.
            /// </summary>
            /// <returns>The same object</returns>
            public T WithAccountNotFoundReason();
            
            /// <summary>
            ///   Updates the login failed reason to use a message
            ///   telling that the used timestamp was out of range
            ///   (this, considering that the timestamp is an uint
            ///   of 32 bits).
            /// </summary>
            /// <returns>The same object</returns>
            public T WithTimestampOutOfRange();

            /// <summary>
            ///   Updates the login failed reason to use a message
            ///   telling that the used timestamp was the same or
            ///   prior to the last attempted timestamp on previous
            ///   login processes.
            /// </summary>
            /// <returns>The same object</returns>
            public T WithTimestampNotGreater();
        }
    }
}