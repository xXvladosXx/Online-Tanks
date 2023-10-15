using System;
using Unity.Collections;
using Unity.Netcode;

namespace Codebase.Runtime.UI.LeaderBoard
{
    public struct LeaderBoardEntityState : INetworkSerializable, IEquatable<LeaderBoardEntityState>
    {
        public ulong ClientID;
        public FixedString32Bytes Username;
        public int Coins;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientID);
            serializer.SerializeValue(ref Username);
            serializer.SerializeValue(ref Coins);
        }

        public bool Equals(LeaderBoardEntityState other)
        {
            return ClientID == other.ClientID && Username.Equals(other.Username) && Coins == other.Coins;
        }

        public override bool Equals(object obj)
        {
            return obj is LeaderBoardEntityState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientID, Username, Coins);
        }
    }
}