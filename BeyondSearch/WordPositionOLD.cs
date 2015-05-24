using System.IO;

namespace BeyondSearch
{
    internal class WordPositionOLD
    {
        private readonly long charPosition;
        private readonly string fileName;

        public WordPositionOLD(long charPosition, string fileName)
        {
            this.charPosition = charPosition;
            this.fileName = fileName;
        }

        public string FileName
        {
            get { return fileName; }
        }

        public long CharPosition
        {
            get { return charPosition; }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "( Pos {0} ) {1}",
                    CharPosition,
                    Path.GetFileName(FileName));
        }
    }
}