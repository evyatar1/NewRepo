using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DBMoviesManager
{
    [Serializable]
    public class ValidationException : Exception
    {
        private string fieldName;
        private string wrongInput;
        public ValidationException()
        {
        }
        public ValidationException(string message) : base(message)
        {
        }
        public ValidationException(string fieldName, string wrongInput) : base("Please enter a valid " + fieldName)
        {
            this.fieldName = fieldName;
            this.wrongInput = wrongInput;
        }
        public string FieldName { get => fieldName; set => fieldName = value; }
    }
}
