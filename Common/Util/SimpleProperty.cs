using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Util
{
    /// <summary />
    /// Simple dynamic property.
    /// </summary />
    public class SimpleProperty<T> : DynamicProperty<T>
    {
        T _value;

        // constructors...
        public SimpleProperty(T value)
        {
            _value = value;
        }

        // public methods...
        public override TValue GetValue<TValue>(string name)
        {
            throw new InvalidOperationException("Can't get property value.");
        }
        public override void SetValue<TValue>(string name, TValue value)
        {
            throw new InvalidOperationException("Can't set property value.");
        }
        public override bool HasProperty(string name)
        {
            return false;
        }
        public override void AddProperty<TValue>(string name,
               DynamicProperty<TValue> property)
        {
            throw new InvalidOperationException("Can't add child properties.");
        }
        public override void RemoveProperty(string name)
        {
            throw new InvalidOperationException("Can't remove child properties.");
        }
        public override DynamicProperty<TValue> GetProperty<TValue>(string name)
        {
            throw new InvalidOperationException("Can't get child properties.");
        }

        // public properties...
        public override T Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
