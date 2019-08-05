using System;
using Amphora.Common.Models;

namespace Amphora.Tests.Helpers
{
    public static class AmphoraLibrary
    {
        private static Random rnd = new Random();
        public static Amphora.Common.Models.Amphora GetValidAmphora()
        {
            return new Amphora.Common.Models.Amphora()
            {
                Bounded = true,
                Description = "Valid Amphora - description",
                Price = rnd.Next(0, 99),
                SchemaId = null,
                Title = "Valid Amphora - title"
            };
        }
    }
}