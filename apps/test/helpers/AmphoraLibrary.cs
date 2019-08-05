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
                Description = "Valid Amphora - description",
                Price = rnd.Next(0, 99),
                Title = "Valid Amphora - title"
            };
        }
    }
}