using AlephVault.Unity.Binary;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Types
    {
        /// <summary>
        ///   A default implementation of <see cref="IEVMLoginFailed{T}"/>,
        ///   without custom reasons to reject a login.
        /// </summary>
        public class EVMLoginFailed : IEVMLoginFailed<EVMLoginFailed>
        {
            /// <summary>
            ///   One of the default rejection reasons.
            /// </summary>
            public enum RejectionReason
            {
                /// <summary>
                ///   The signature was of an invalid format.
                /// </summary>
                InvalidSignature,
                
                /// <summary>
                ///   The account of the as-per-signature value
                ///   was not found.
                /// </summary>
                AccountNotFound,
                
                /// <summary>
                ///   The timestamp in the challenge is out of
                ///   range (with respect to the server side's
                ///   current timestamp and a tolerance).
                /// </summary>
                TimestampOutOfRange,
                
                /// <summary>
                ///   Another timestamp, perhaps greater or
                ///   equal to this one, was already used in
                ///   a previous login attempt.
                /// </summary>
                TimestampNotGreater
            }

            /// <summary>
            ///   The rejection reason.
            /// </summary>
            public RejectionReason Reason;
            
            /// <summary>
            ///   Sets the <see cref="RejectionReason.AccountNotFound"/>
            ///   rejection reason.
            /// </summary>
            /// <returns>The same object</returns>
            public EVMLoginFailed WithAccountNotFoundReason()
            {
                Reason = RejectionReason.AccountNotFound;
                return this;
            }

            /// <summary>
            ///   Sets the <see cref="RejectionReason.TimestampOutOfRange"/>
            ///   rejection reason.
            /// </summary>
            /// <returns>The same object</returns>
            public EVMLoginFailed WithTimestampOutOfRange()
            {
                Reason = RejectionReason.TimestampOutOfRange;
                return this;
            }

            /// <summary>
            ///   Sets the <see cref="RejectionReason.TimestampNotGreater"/>
            ///   rejection reason.
            /// </summary>
            /// <returns>The same object</returns>
            public EVMLoginFailed WithTimestampNotGreater()
            {
                Reason = RejectionReason.TimestampNotGreater;
                return this;
            }

            /// <summary>
            ///   Sets the <see cref="RejectionReason.InvalidSignature"/>
            ///   rejection reason.
            /// </summary>
            /// <returns>The same object</returns>
            public EVMLoginFailed WithInvalidSignatureReason()
            {
                Reason = RejectionReason.InvalidSignature;
                return this;
            }
            
            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref Reason);
            }
        }
    }
}