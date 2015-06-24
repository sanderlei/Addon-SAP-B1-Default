using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Util
{
    public class DuplicateObject
    {
        public DynamicProperty<object> Duplicate(object obj)
        {
            DynamicProperty<object> complexProperty = new ComplexProperty<object>();
            SimpleProperty<object> simpleProperty;
            foreach (System.Reflection.PropertyInfo property in obj.GetType().GetProperties())
            {
                object value = property.GetValue(obj, null);
                simpleProperty = new SimpleProperty<object>(value);
                
                complexProperty.AddProperty(property.Name, simpleProperty); 
            }
            return complexProperty;
        }
    }
}
