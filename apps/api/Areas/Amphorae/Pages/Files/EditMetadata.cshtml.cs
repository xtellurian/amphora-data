using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Amphora.Api.Areas.Amphorae.Pages.Files
{
    [CommonAuthorize]
    public class EditMetadataPageModel : AmphoraPageModel
    {
        private readonly IAmphoraFileService amphoraFileService;

        public EditMetadataPageModel(IAmphoraeService amphoraeService, IAmphoraFileService amphoraFileService) : base(amphoraeService)
        {
            this.amphoraFileService = amphoraFileService;
        }

        public Dictionary<string, KeyValuePair<string, string>> Meta { get; set; } = new Dictionary<string, KeyValuePair<string, string>>();

        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            await LoadAmphoraAsync(id);
            this.Name = name;
            if (Amphora != null)
            {
                var meta = await amphoraFileService.Store.ReadAttributes(Amphora, name);

                this.Meta = new Dictionary<string, KeyValuePair<string, string>>();
                var index = 0;
                foreach (var m in meta)
                {
                    this.Meta.Add(index++.ToString(), new KeyValuePair<string, string>(m.Key, m.Value));
                }
            }
            else
            {
                return RedirectToPage(PageMap.Index);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string name, [FromForm] string meta)
        {
            await LoadAmphoraAsync(id);
            this.Name = name;
            if (Amphora != null)
            {
                if (!string.IsNullOrEmpty(meta))
                {
                    try
                    {
                        this.Meta = JsonConvert.DeserializeObject<Dictionary<string, KeyValuePair<string, string>>>(meta);
                        var dic = this.Meta?.ToChildDictionary();
                        var res = await amphoraFileService.WriteAttributesAsync(User, Amphora, dic, name);

                        if (res.Succeeded)
                        {
                            return RedirectToPage(PageMap.Files, new { id = id });
                        }
                        else
                        {
                            this.ModelState.AddModelError(string.Empty, res.Message);
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    catch (JsonException)
                    {
                        // do something maybe?
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "MetaData can't be empty");
                }
            }

            return Page();
        }
    }
}