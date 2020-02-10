using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Amphora.Api.Areas.Amphorae.Pages.Files
{
    [Authorize]
    public class EditMetadataPageModel : AmphoraPageModel
    {
        public EditMetadataPageModel(IAmphoraeService amphoraeService) : base(amphoraeService)
        {
        }

        public Dictionary<string, KeyValuePair<string, string>> Meta { get; set; } = new Dictionary<string, KeyValuePair<string, string>>();

        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            await LoadAmphoraAsync(id);
            this.Name = name;
            if (Amphora != null)
            {
                Amphora.FileAttributes ??= new Dictionary<string, Common.Models.Amphorae.AttributeStore>();

                if (Amphora.FileAttributes.TryGetValue(name, out var meta))
                {
                    this.Meta = new Dictionary<string, KeyValuePair<string, string>>();
                    var index = 0;
                    foreach (var m in meta.Attributes)
                    {
                        this.Meta.Add(index++.ToString(), new KeyValuePair<string, string>(m.Key, m.Value));
                    }
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
                        Amphora.FileAttributes ??= new Dictionary<string, Common.Models.Amphorae.AttributeStore>();
                        Amphora.FileAttributes[name] = new Common.Models.Amphorae.AttributeStore(dic);

                        var res = await amphoraeService.UpdateAsync(User, Amphora);
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