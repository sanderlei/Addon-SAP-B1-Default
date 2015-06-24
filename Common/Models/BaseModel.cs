using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    public class BaseModel : Attribute
    {
        public BaseModel()
        { }

        private Type fieldType;

        public Type FieldType { get { return fieldType; } }



        public BaseModel(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }
}
