using System;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
    {
        private static Random rnd = new Random();
        public static Amphora.Common.Models.Amphora GetValidAmphora(string id = null)
        {
            return new Amphora.Common.Models.Amphora()
            {
                Id = id,
                Description = "Valid Amphora - description",
                Price = rnd.Next(0, 99),
                Title = "Valid Amphora - title"
            };
        }

        public static Amphora.Common.Models.Amphora GetInalidAmphora(string id = null)
        {
            return new Amphora.Common.Models.Amphora()
            {
                Id = id,
                Description = null,
                Price =  -1 * rnd.Next(0, 99),
                Title = ""
            };
        }
    }
}