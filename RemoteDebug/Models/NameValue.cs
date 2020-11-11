namespace KemengSoft.UTILS.RemoteDebug
{
    public class NameValue<T>
    {
        private string _name;
        private T _value;
        public NameValue()
        {

        }
        public NameValue(string name, T value)
        {
            this._name = name;
            this._value = value;
        }
        public string Name { get => _name; set => _name = value; }
        public T Value { get => _value; set => _value = value; }
    }
}
