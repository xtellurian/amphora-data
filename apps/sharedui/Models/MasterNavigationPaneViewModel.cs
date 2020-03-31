using System.Collections.Generic;
using System.Linq;

namespace Amphora.SharedUI.Models
{
    public class MasterNavigationPaneViewModel
    {
        public MasterNavigationPaneViewModel(string index, params string[] paths)
        {
            this.Index = new Page(index);
            this.Pages = new List<Page>();
            foreach (var p in paths)
            {
                this.Pages.Add(new Page(p));
            }
        }

        public MasterNavigationPaneViewModel(Page index, IEnumerable<Page> pages)
        {
            this.Index = index;
            this.Pages = pages.ToList();
        }

        public Page Index { get; set; }
        public List<Page> Pages { get; }

        public class Page
        {
            public Page(string path, Page parent = null, string name = null, IDictionary<string, string> query = null)
            {
                Path = path;
                Parent = parent;
                if (name == null)
                {
                    name = Path.Split('/').LastOrDefault();
                }

                Name = name;
                if (query != null && query.Count > 0)
                {
                    Query = query;
                }
            }

            public string Path { get; }
            public Page Parent { get; }
            public string Name { get; }
            public bool IsActive { get; set; } = false;
            public IDictionary<string, string> Query { get; set; } = new Dictionary<string, string>();
        }
    }
}
