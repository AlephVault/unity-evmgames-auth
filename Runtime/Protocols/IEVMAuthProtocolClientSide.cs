using System;
using System.Threading.Tasks;
using AlephVault.Unity.Meetgard.Types;
using Exception = System.Exception;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Protocols
    {
        /// <summary>
        ///   This interface defines some fields that have
        ///   to be populated when doing a crypto/EVM auth
        ///   attempt (only the signature and the timestamp
        ///   according to the challenge are needed). Also,
        ///   the Logout method (already present in base
        ///   auth protocols) is matched in this interface.
        /// </summary>
        public interface IEVMAuthProtocolClientSide
        {
            /// <summary>
            ///   The signature to use as login data. This
            ///   signature is not empty, and related to
            ///   the given <see cref="Timestamp"/>.
            /// </summary>
            public string Signature { get; set; }

            /// <summary>
            ///   The timestamp the signature stands for.
            ///   This timestamp must be recent (according
            ///   to what the server side understand as a
            ///   "recent" timestamp, typically in a range
            ///   of [server's now() +/- 120 secs]).
            /// </summary>
            public uint Timestamp { get; set; }

            /// <summary>
            ///   Sends a logout message and immediately closes.
            /// </summary>
            public Task Logout();

            /// <summary>
            ///   Events that can attend disconnection of this EVM protocol.
            /// </summary>
            public event Func<Exception, Task> OnEVMClientDisconnected;
        }
    }
}