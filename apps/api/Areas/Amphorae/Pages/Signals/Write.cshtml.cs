using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae.Signals;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [CommonAuthorize]
    public class WriteModel : AmphoraPageModel
    {
        private readonly ISignalService signalService;

        public WriteModel(IAmphoraeService amphoraeService, ISignalService signalService) : base(amphoraeService)
        {
            this.signalService = signalService;
        }

        [BindProperty]
        public List<SignalValue> Values { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            if (Amphora != null)
            {
                var signals = Amphora.V2Signals;
                this.Values = new List<SignalValue>();
                foreach (var s in signals)
                {
                    this.Values.Add(new SignalValue(s.Property, s.ValueType));
                }

                this.Values.Add(new SignalValue("TimeStamp", SignalV2.DateTime) { DateTimeValue = System.DateTime.UtcNow });
            }

            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await LoadAmphoraAsync(id);
            if (Amphora != null)
            {
                var values = new Dictionary<string, object>();
                foreach (var s in Amphora.V2Signals)
                {
                    var dto = Values.FirstOrDefault(d => d.Property == s.Property); // d.Property is null (not bound?)
                    if (dto == null) { continue; }
                    if (dto.IsNumeric) { values.Add(s.Property, dto.NumericValue); }
                    else if (dto.IsString) { values.Add(s.Property, dto.StringValue); }
                }

                if (Values.FirstOrDefault(v => v.IsDateTime)?.DateTimeValue.HasValue ?? false)
                {
                    values[SpecialProperties.Timestamp] = Values.FirstOrDefault(v => v.IsDateTime)?.DateTimeValue.Value;
                }

                await signalService.WriteSignalAsync(User, this.Amphora, values);
            }

            return RedirectToPage("/Detail/Signals", new { id = id });
        }
    }
}
