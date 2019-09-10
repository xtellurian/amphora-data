using System;
using Amphora.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Amphora.Tests.Helpers
{
    public static class BadJsonLibrary
    {
        public const string DiverseTypesKey = "DiverseTypes";
        public const string BadlyFormedAmphoraKey = "BadlyFormedAmphora";
        private static Random rnd = new Random();
        public static string GetJson(string key)
        {
            if (string.Equals(key, DiverseTypesKey)) return JsonConvert.SerializeObject(new DiverseTypes());
            if (string.Equals(key, BadlyFormedAmphoraKey)) return BadlyFormedAmphora();
            return "";
        }

        private static string BadlyFormedAmphora()
        {
            var entity = new Amphora.Common.Models.Amphora
            {
                Price = -1,
                Name = null,
                Description = null,
            };
            return JsonConvert.SerializeObject(entity);
        }

        private class DiverseTypes
        {
            public string A {get;set;} = "akjsbfjksd";
            public bool B {get;set;} = true;
            public double C {get;set;} = 12442.24;

        }
    }

    
}