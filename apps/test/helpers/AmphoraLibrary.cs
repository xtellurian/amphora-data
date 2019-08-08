using System;
using Amphora.Common.Models;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
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

        public static Amphora.Common.Models.Tempora GetValidTempora()
        {
            return new Amphora.Common.Models.Tempora()
            {
                Description = "Valid Amphora - description",
                Price = rnd.Next(0, 99),
                Title = "Valid Amphora - title"
            };
        }

        public static Amphora.Common.Models.Tempora GetInvalidTempora()
        {
            return new Amphora.Common.Models.Tempora()
            {
                Description = null,
                Price = -1 * rnd.Next(0, 99),
                Title = null
            };
        }
    }
}