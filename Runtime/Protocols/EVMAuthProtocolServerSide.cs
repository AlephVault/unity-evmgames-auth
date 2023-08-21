using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.Signer;
using AlephVault.Unity.EVMGames.Auth.Types;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.Meetgard.Auth.Types;
using AlephVault.Unity.Meetgard.Types;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Protocols
    {
        /// <summary>
        ///   A protocol server side to perform an EVM login (this
        ///   means: a login attempt linked to cryptographic signing
        ///   processes, intended to be compatible with any EVM-like
        ///   network, like Ethereum, Polygon or BSC). The process
        ///   is meant to receive both a signature and a the signed
        ///   value, which is a timestamp satisfying a challenge:
        ///   it must be in a certain interval centered on the value
        ///   of the current timestamp, and it also must be above
        ///   the last timestamp the account successfully logged in
        ///   previously (this, to avoid even recent replay attacks).
        /// </summary>
        /// <typeparam name="LoginFailed">A custom message for when the login fails</typeparam>
        /// <typeparam name="Kicked">A custom message for when a user is kicked</typeparam>
        /// <typeparam name="AccountPreviewDataType">
        ///   The data type for the account preview. It includes the account address
        /// </typeparam>
        /// <typeparam name="AccountDataType">
        ///   The data type for the full account data. It includes the last successful
        ///   login timestamp
        /// </typeparam>
        public abstract class EVMAuthProtocolServerSide<
            LoginFailed, Kicked, AccountPreviewDataType, AccountDataType
        > : SimpleAuthProtocolServerSide<
            EVMAuthProtocolDefinition<Nothing, LoginFailed, Kicked>,
            Nothing, LoginFailed, Kicked, string, AccountPreviewDataType, AccountDataType
        >
            where LoginFailed : IEVMLoginFailed<LoginFailed>, new()
            where Kicked : IKickMessage<Kicked>, new()
            where AccountPreviewDataType : IEVMAccountPreviewData, new()
            where AccountDataType : IEVMRecordWithPreview<string, AccountPreviewDataType>
        {
            /// <summary>
            ///   The tolerance for which the timestamps
            ///   are considered. This tolerance extends
            ///   both for future and past with respect
            ///   to the current instant in which the
            ///   login message is received and processed.
            /// </summary>
            [SerializeField]
            private uint timestampTolerance;
            
            /// <summary>
            ///   It only attends the Login:EVM message.
            /// </summary>
            protected override void SetLoginMessageHandlers()
            {
                AddLoginMessageHandler<EVMLoginMessage>("EVM", async (msg) =>
                {
                    string signature = msg.Signature;
                    uint timestamp = msg.Timestamp;
                    string message = ChallengeUtils.TimestampChallengeMessage(timestamp);
                    
                    // 1. Validate signature. Get its address.
                    EthereumMessageSigner signer = new EthereumMessageSigner();
                    string address = "";
                    try
                    {
                        address = signer.EncodeUTF8AndEcRecover(message, signature);
                    }
                    catch (System.Exception e)
                    {
                        return RejectLogin(new LoginFailed().WithInvalidSignatureReason());
                    }
                    // 2. Validate timestamp to be +/- the
                    //    tolerance value.
                    if (!ChallengeUtils.IsCloseToNow(timestamp, timestampTolerance))
                    {
                        return RejectLogin(new LoginFailed().WithTimestampOutOfRange());
                    }
                    // 3. Validate the account to exist.
                    AccountDataType account = await FindAccount(address);
                    if (EqualityComparer<AccountDataType>.Default.Equals(account, default))
                    {
                        return RejectLogin(new LoginFailed().WithAccountNotFoundReason());
                    }
                    // 4. Check whether there is any other reason
                    //    to reject a login attempt.
                    LoginFailed rejection = await CheckLoginAttempt(account);
                    if (!EqualityComparer<LoginFailed>.Default.Equals(rejection, default))
                    {
                        return RejectLogin(rejection);
                    }
                    // 5. Validate the account's last login
                    //    stamp to be LOWER than the current
                    //    timestamp provided in the message.
                    //    May warn about a possible replay
                    //    attack on this account.
                    if (account.LastLoginTimestamp() >= timestamp)
                    {
                        await NotifyPossibleReplyAttack(account);
                        return RejectLogin(new LoginFailed().WithTimestampNotGreater());
                    }
                    // 6. Everything is OK, but ensure the
                    //    new stamp is stored.
                    await SetLastLoginTime(address, timestamp);
                    await NotifySuccessfulLogin(account);

                    // Return OK.
                    return AcceptLogin(Nothing.Instance, address);
                });
            }

            /// <summary>
            ///   Updates the last login timestamp of an account.
            /// </summary>
            /// <param name="address">The address of the account being updated</param>
            /// <param name="timestamp">The timestamp to set as last login time for them</param>
            protected abstract Task SetLastLoginTime(string address, uint timestamp);

            /// <summary>
            ///   Checks the account for any reason to not allow
            ///   it to be logged in (e.g. a ban, or account block
            ///   due to many recent attempts).
            /// </summary>
            /// <param name="account">The account to check</param>
            /// <returns><code>null</code> if there is no reason to reject, or a custom value otherwise</returns>
            protected virtual async Task<LoginFailed> CheckLoginAttempt(AccountDataType account)
            {
                return default;
            }

            /// <summary>
            ///   Warns the account (or somehow the engine) about
            ///   a possible reply attack attempt on this account.
            ///   By default, nothing is done.
            /// </summary>
            /// <param name="account">The account to warn about</param>
            protected virtual async Task NotifyPossibleReplyAttack(AccountDataType account) {}

            /// <summary>
            ///   Tells the account was successful on login.
            /// </summary>
            /// <param name="account">The account to notify about</param>
            protected virtual async Task NotifySuccessfulLogin(AccountDataType account) {}
        }
    }
}