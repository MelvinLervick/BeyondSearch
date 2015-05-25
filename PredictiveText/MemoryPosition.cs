using System;
using System.IO;

namespace PredictiveText
{
    [Serializable]
    public class MemoryPosition
    {
        private readonly long charPosition;
        private readonly string fileName;

        public MemoryPosition(long charPosition, string fileName)
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
                    "( Line {0} ) {1}",
                    CharPosition,
                    FileName);
        }
    }
}