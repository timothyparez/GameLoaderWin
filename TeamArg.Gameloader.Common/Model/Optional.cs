namespace TeamArg.GameLoader.Common.Model
{
    public class Optional<T>
    {
        public bool HasValue { get; }
        public T Value { get; }
        public Optional(T value)
        {
            Value = value;
            HasValue = true;
        }

        public Optional()
        {
            HasValue = false;
        }
    }
}