namespace WordSearch.PatriciaTrie
{
    public struct SplitResult
    {
        private readonly StringPartition head;
        private readonly StringPartition rest;

        public SplitResult(StringPartition head, StringPartition rest)
        {
            this.head = head;
            this.rest = rest;
        }

        public StringPartition Rest
        {
            get { return rest; }
        }

        public StringPartition Head
        {
            get { return head; }
        }

        public bool Equals(SplitResult other)
        {
            return head == other.head && rest == other.rest;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SplitResult && Equals((SplitResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (head.GetHashCode()*397) ^ rest.GetHashCode();
            }
        }

        public static bool operator ==(SplitResult left, SplitResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SplitResult left, SplitResult right)
        {
            return !(left == right);
        }
    }
}