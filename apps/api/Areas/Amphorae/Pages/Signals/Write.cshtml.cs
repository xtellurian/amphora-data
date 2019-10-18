using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    public class WriteModel : AmphoraPageModel
    {
        private readonly ISignalService signalService;

        public WriteModel(IAmphoraeService amphoraeService, ISignalService signalService) : base(amphoraeService)
        {
            this.signalService = signalService;
        }

        [BindProperty]
        public List<SignalValueDto> Values { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            if (Amphora != null)
            {
                var signals = this.Amphora.Signals.Select(s => s.Signal).ToList();
                this.Values = new List<SignalValueDto>();
                foreach (var s in signals)
                {
                    this.Values.Add(new SignalValueDto(s.KeyName, s.ValueType));
                }
                this.Values.Add(new SignalValueDto("TimeStamp", SignalModel.DateTime) { DateTimeValue = System.DateTime.UtcNow });
            }

            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            if (Amphora != null)
            {
                var values = new Dictionary<string, object>();
                foreach (var s in Amphora.Signals)
                {
                    var dto = Values.FirstOrDefault(d => d.KeyName == s.Signal.KeyName); // d.keyname is null (not bound?)
                    if (dto == null) continue;
                    if (dto.IsNumeric) values.Add(s.Signal.KeyName, dto.NumericValue);
                    else if (dto.IsString) values.Add(s.Signal.KeyName, dto.StringValue);
                }
                if(Values.FirstOrDefault(v => v.IsDateTime)?.DateTimeValue.HasValue ?? false)
                {
                    values["t"] = Values.FirstOrDefault(v => v.IsDateTime)?.DateTimeValue.Value;
                }
                await signalService.WriteSignalAsync(User, this.Amphora, values);
            }
            return RedirectToPage("./Index", new { id = id });
        }
    }
}