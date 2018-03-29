namespace TeamArg.GameLoader.AvrDude
{
    public class MemoryOperationSpecification
    {
        public string MemoryType { get; set; }
        public string Operation { get; set; }
        public string Filename { get; set; }
        public string Format { get; set; }

        public MemoryOperationSpecification(string filename, string memoryType = "flash", string operation = "w", string format = "i")
        {
            MemoryType = memoryType;
            Operation = operation;
            Filename = filename;
            Format = format;
        }

        public override string ToString()
        {
            return $"{MemoryType}:{Operation}:{Filename}:{Format}";
        }
    }
}