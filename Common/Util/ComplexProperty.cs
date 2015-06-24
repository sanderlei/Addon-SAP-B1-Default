using System;
using System.Collections.Generic;

namespace Common.Util
{
    /// <summary>
    /// Complex dynamic property implementation.
    /// </summary>
    public class ComplexProperty<T> : DynamicProperty<T>
    {
        Dictionary<string, object> _properties;

        // constructors...
        public ComplexProperty()
        {
        }

        // private properties...
        Dictionary<string, object> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new Dictionary<string, object>();
                return _properties;
            }
        }

        // public methods...
        public override TValue GetValue<TValue>(string name)
        {
            DynamicProperty<TValue> property = GetProperty<TValue>(name);
            if (property != null)
                return property.Value;
            return default(TValue);
        }
        public override void SetValue<TValue>(string name, TValue value)
        {
            DynamicProperty<TValue> property = GetProperty<TValue>(name);
            if (property != null)
                property.Value = value;
        }
        public override bool HasProperty(string name)
        {
            return _properties == null ? false : Properties.ContainsKey(name);
        }
        public override void AddProperty<TValue>(string name,
               DynamicProperty<TValue> property)
        {
            if (HasProperty(name))
                throw new InvalidOperationException("Can't add property.");

            Properties.Add(name, property);
        }
        public override void RemoveProperty(string name)
        {
            Properties.Remove(name);
        }
        public override DynamicProperty<TValue> GetProperty<TValue>(string name)
        {
            if (!HasProperty(name))
                throw new InvalidOperationException("Can't get property.");

            DynamicProperty<TValue> property = Properties[name] as DynamicProperty<TValue>;
            if (property == null)
                throw new InvalidOperationException("Invalid type specified.");

            return property;
        }

        // public properties...
        public override T Value
        {
            get { return default(T); }
            set { }
        }
    }
}
