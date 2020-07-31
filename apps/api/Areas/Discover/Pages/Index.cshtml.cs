using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Maths;
using Amphora.Common.Models.Amphorae;
using Amphora.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Discover.Pages
{
    [CommonAuthorize]
    public class IndexPageModel : PageModel
    {
        private readonly ISearchService searchService;
        private readonly IAuthenticateService authenticateService;
        private readonly IMapService mapService;

        public string MapKey { get; }
        public GeoLocation MapCenter { get; private set; }
        public int Zoom { get; private set; } = 3;

        public IndexPageModel(ISearchService searchService,
                              IAuthenticateService authenticateService,
                              IMapService mapService,
                              IOptionsMonitor<AzureMapsOptions> mapsOptions)
        {
            this.searchService = searchService;
            this.authenticateService = authenticateService;
            this.mapService = mapService;
            this.MapKey = mapsOptions.CurrentValue?.Key;
        }

        [BindProperty(SupportsGet = true)]
        public AmphoraSearchQueryOptions Q { get; set; } = new AmphoraSearchQueryOptions();
        public long Count { get; set; }
        public string CenterReason { get; set; }

        public IEnumerable<AmphoraModel> Entities { get; set; }
        public IList<FacetResult> LabelFacets { get; private set; } = new List<FacetResult>();
        public bool MapView { get; private set; }
        public string Handler { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Handler ??= "ListView";
            await RunSearch();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Handler ??= "ListView";
            await RunSearch();
            return Page();
        }

        public async Task<IActionResult> OnGetListViewAsync()
        {
            this.MapView = false;
            this.Handler = "ListView";
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnPostListViewAsync()
        {
            this.MapView = false;
            this.Handler = "ListView";
            return await OnPostAsync();
        }

        public async Task<IActionResult> OnGetMapViewAsync()
        {
            this.MapView = true;
            this.Handler = "MapView";
            var response = await OnGetAsync();
            await CheckCenterAsync();
            return response;
        }

        public async Task<IActionResult> OnPostMapViewAsync()
        {
            this.MapView = true;
            this.Handler = "MapView";
            var response = await OnPostAsync();
            await CheckCenterAsync();
            return response;
        }

        private async Task CheckCenterAsync()
        {
            if (Entities != null && Entities.Any())
            {
                await LoadCenterFromEntities();
            }
            else if (!TryLoadCenterFromSearchDefinition())
            {
                await LoadCenterFromIp();
            }
            else if (MapCenter == null)
            {
                // last default
                MapCenter = new GeoLocation(133.77, -25.27); // australia center
                Zoom = 3;
                CenterReason = "Defaulted to Australia";
            }
        }

        private bool TryLoadCenterFromSearchDefinition()
        {
            if (Q?.Lat != null && Q.Lon != null)
            {
                CenterReason = "Map Location based on search parameters";
                MapCenter = new GeoLocation(Q.Lon.Value, Q.Lat.Value);
                Zoom = 8;
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task LoadCenterFromIp()
        {
            // load the map position based on the default position in the IP address
            if (HttpContext.Request.TryGetForwardedSourceIpAddress(out var ip))
            {
                CenterReason = $"Map Location based on IP Address {ip}";
                MapCenter = await mapService.GetPositionFromIp(ip);
                Zoom = 3;
            }
            else
            {
                CenterReason = $"Map Location based on IP Address {HttpContext.Connection.RemoteIpAddress}";
                MapCenter = await mapService.GetPositionFromIp(HttpContext.Connection.RemoteIpAddress);
                Zoom = 3;
            }
        }

        private async Task LoadCenterFromEntities()
        {
            // get the center based on the entities in the list
            var withPosition = Entities.Where(e => e.GeoLocation != null && e.GeoLocation.HasValue()).ToList();
            if (withPosition.Count > 0)
            {
                var maxLat = withPosition.Max(a => a.GeoLocation.Lat());
                var minLat = withPosition.Min(a => a.GeoLocation.Lat());
                var maxLon = withPosition.Max(a => a.GeoLocation.Lon());
                var minLon = withPosition.Min(a => a.GeoLocation.Lon());
                var maxRange = 30;
                var reasonableSize =
                    System.Math.Abs(maxLat - minLat) < maxRange
                    && System.Math.Abs(maxLon - minLon) < maxRange;
                if (reasonableSize)
                {
                    // use the average for the center
                    MapCenter = new GeoLocation((maxLon + minLon) / 2, (maxLat + minLat) / 2);
                    CenterReason = "Map Location based on Amphora positions";
                    var maxDistance = Haversine.Distance(new GeoLocation(maxLon, maxLat), new GeoLocation(minLon, minLat));
                    if (maxDistance > 10)
                    {
                        Zoom = DistanceToZoomLevel.DistanceInMetersToZoomLevel(maxDistance * 1000);
                    }
                    else
                    {
                        Zoom = 8;
                    }
                }
                else
                {
                    await LoadCenterFromIp();
                }
            }
            else
            {
                await LoadCenterFromIp();
            }
        }

        private ParallelQuery<string> GetLabels()
        {
            var labels = this.Q?.Labels?.Split(',').Where(_ => !string.IsNullOrWhiteSpace(_)).AsParallel();
            labels?.ForAll(_ => _.Trim()); // trim all labels
            this.Q.Labels = labels != null ? string.Join(',', labels) : null;
            return labels;
        }

        private async Task RunSearch()
        {
            var geo = GetGeo();
            var labels = GetLabels();
            var searchParameters = Q.ToSearchParameters();
            var res = await searchService.SearchAsync<AmphoraModel>(Q.Term, searchParameters);
            this.Count = res.Count.HasValue ? res.Count.Value : 0;
            this.Entities = res.Results.Select(_ => _.Entity);
            if (res.Facets.TryGetValue($"{nameof(AmphoraModel.Labels)}/{nameof(Label.Name)}", out var labelFacets))
            {
                this.LabelFacets = labelFacets;
            }
        }

        private GeoLocation GetGeo()
        {
            GeoLocation geo = null;
            if (Q.Lat.HasValue && Q.Lon.HasValue)
            {
                geo = new GeoLocation(Q.Lon.Value, Q.Lat.Value);
            }

            return geo;
        }
    }
}