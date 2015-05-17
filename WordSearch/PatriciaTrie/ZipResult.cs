using System.Diagnostics;

namespace WordSearch.PatriciaTrie
{
    [DebuggerDisplay("Head: '{CommonHead}', This: '{ThisRest}', Other: '{OtherRest}', Kind: {MatchKind}")]
    public struct ZipResult
    {
        private readonly StringPartition commonHead;
        private readonly StringPartition otherRest;
        private readonly StringPartition thisRest;

        public ZipResult(StringPartition commonHead, StringPartition thisRest, StringPartition otherRest)
        {
            this.commonHead = commonHead;
            this.thisRest = thisRest;
            this.otherRest = otherRest;
        }

        public MatchKind MatchKind
        {
            get
            {
                return thisRest.Length == 0
                    ? (otherRest.Length == 0
                        ? MatchKind.ExactMatch
                        : MatchKind.IsContained)
                    : (otherRest.Length == 0
                        ? MatchKind.Contains
                        : MatchKind.Partial);
            }
        }

        public StringPartition OtherRest
        {
            get { return otherRest; }
        }

        public StringPartition ThisRest
        {
            get { return thisRest; }
        }

        public StringPartition CommonHead
        {
            get { return commonHead; }
        }


        public bool Equals(ZipResult other)
        {
            return 
                commonHead == other.commonHead && 
                otherRest == other.otherRest &&
                thisRest == other.thisRest;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ZipResult && Equals((ZipResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = commonHead.GetHashCode();
                hashCode = (hashCode*397) ^ otherRest.GetHashCode();
                hashCode = (hashCode*397) ^ thisRest.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ZipResult left, ZipResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ZipResult left, ZipResult right)
        {
            return !(left == right);
        }
    }
}