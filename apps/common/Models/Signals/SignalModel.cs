using System;
using System.Collections.Generic;

namespace Amphora.Common.Models.Signals
{
    public class SignalModel : Entity
    {
        public static string Numeric => nameof(Numeric);
        public static string String => nameof(String);
        public static string DateTime => nameof(DateTime);
        public static List<string> Options => new List<string>{Numeric, String};

        public SignalModel()
        {
        }

        public SignalModel(string keyName, string valueType)
        {
            KeyName = keyName ?? throw new System.ArgumentNullException(nameof(keyName));

            if(string.Equals(valueType, Numeric) || string.Equals(valueType, String) || string.Equals(valueType, DateTime))
            {
                ValueType = valueType;
            }
            else
            {
                throw new ArgumentException("ValueType must be Numeric, String, or DateTime");
            }
            CreatedDate = System.DateTime.UtcNow;

            this.Id = $"{KeyName}-{ValueType}";
        }

        public string KeyName { get; set; }
        public string ValueType {get;set;}

        public bool IsNumeric => this.ValueType == Numeric;
        public bool IsString => this.ValueType == String;
        
    }
}