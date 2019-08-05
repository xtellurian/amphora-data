using System;
using Amphora.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Amphora.Tests.Helpers
{
    public static class BadJsonLibrary
    {
        public const string DiverseTypesKey = "DiverseTypes";
        private static Random rnd = new Random();
        public static string GetJson(string key)
        {
            if (string.Equals(key, DiverseTypesKey)) return JsonConvert.SerializeObject(new DiverseTypes());

            return "";
        }

        private class DiverseTypes
        {
            public string A {get;set;} = "akjsbfjksd";
            public bool B {get;set;} = true;
            public double C {get;set;} = 12442.24;

        }
    }

    
}