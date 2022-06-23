using System;
using AlephVault.Unity.Binary;
using AlephVault.Unity.EVMGames.Auth.Types;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Samples
    {
        [Serializable]
        public class SampleAccountDataType : IEVMRecordWithPreview<string, SampleAccountPreviewDataType>
        {
            [SerializeField]
            private SampleAccountPreviewDataType preview = new SampleAccountPreviewDataType();
            private uint lastLoginTimestamp = 0;
        
            public void Serialize(Serializer serializer)
            {
                preview.Serialize(serializer);
            }
        
            public string GetID()
            {
                return preview.Address;
            }

            public SampleAccountPreviewDataType GetProfileDisplayData()
            {
                return preview;
            }

            public uint LastLoginTimestamp()
            {
                return lastLoginTimestamp;
            }

            public void UpdateLastLoginTimestamp(uint timestamp)
            {
                if (timestamp > lastLoginTimestamp)
                {
                    lastLoginTimestamp = timestamp;
                }
            }
        }
    }
}