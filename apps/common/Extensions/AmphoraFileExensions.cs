using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Common.Extensions
{
    public static class AmphoraFileExtensions
    {
        public static Task LoadAttributesAsync(this IEnumerable<IAmphoraFileReference> files)
        {
            return Task.CompletedTask;
            // var tasks = new List<Task>();
            // foreach (var f in files)
            // {
            //     tasks.Add(f.LoadAttributesAsync());
            // }

            // await Task.WhenAll(tasks);
        }
    }
}