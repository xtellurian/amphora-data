using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
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

        public Dictionary<string, CustomKVP> Meta { get; set; } = new Dictionary<string, CustomKVP>();

        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            await LoadAmphoraAsync(id);
            this.Name = name;
            if (Amphora != null)
            {
                Amphora.FilesMetaData ??= new Dictionary<string, Common.Models.Amphorae.MetaDataStore>();

                if (Amphora.FilesMetaData.TryGetValue(name, out var meta))
                {
                    this.Meta = new Dictionary<string, CustomKVP>();
                    var index = 0;
                    foreach (var m in meta.MetaData)
                    {
                        this.Meta.Add(index++.ToString(), new CustomKVP(m.Key, m.Value));
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
                        this.Meta = JsonConvert.DeserializeObject<Dictionary<string, CustomKVP>>(meta);
                        var dic = ToDictionary(this.Meta);
                        Amphora.FilesMetaData ??= new Dictionary<string, Common.Models.Amphorae.MetaDataStore>();
                        Amphora.FilesMetaData[name] = new Common.Models.Amphorae.MetaDataStore(dic);

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

        private Dictionary<string, string> ToDictionary(Dictionary<string, CustomKVP> kvps)
        {
            var dic = new Dictionary<string, string>();
            foreach (var kvp in kvps)
            {
                if (!dic.ContainsKey(kvp.Value.Key))
                {
                    dic.Add(kvp.Value.Key, kvp.Value.Value);
                }
                else
                {
                    throw new ArgumentException("Duplicate key");
                }
            }

            return dic;
        }

        public class CustomKVP
        {
            public CustomKVP()
            {
            }

            public CustomKVP(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}