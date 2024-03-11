using System;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Types
    {
        /// <summary>
        ///   Challenge utils related to timestamps (and
        ///   their checks) to avoid replay attacks.
        /// </summary>
        public static class ChallengeUtils
        {
            /// <summary>
            ///   Generates the current timestamp for the
            ///   challenge.
            /// </summary>
            /// <returns>A timestamp, with microseconds</returns>
            public static uint CurrentTimestamp()
            {
                return (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }

            /// <summary>
            ///   Creates a challenge message from a given
            ///   timestamp. This challenge message will be
            ///   generated in both sides given the timestamp
            ///   (once to sign, once to verify).
            /// </summary>
            /// <param name="timestamp">The timestamp from which the challenge message will be generated</param>
            /// <returns>The challenge message</returns>
            public static string TimestampChallengeMessage(uint timestamp)
            {
                return $"AlephVault.Unity.EVMGames.Auth:Challenge:{timestamp}";
            }

            /// <summary>
            ///   Checks whether the given timestamp is close
            ///   to the current timestamp, given a tolerance.
            /// </summary>
            /// <param name="timestamp">The given timestamp</param>
            /// <param name="tolerance">The tolerance</param>
            /// <returns>
            ///   Whether the given timestamp is close to the current timestamp (in terms of the tolerance)
            /// </returns>
            public static bool IsCloseToNow(uint timestamp, uint tolerance)
            {
                uint now = CurrentTimestamp();
                uint diff = timestamp > now ? timestamp - now : now - timestamp;
                return diff < tolerance;
            }
        }
    }
}