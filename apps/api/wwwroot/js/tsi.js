// returns a function to render
async function tsi(id, signals, filters) {

    // filters should be a dictionary with propertyName: tsx value, and object {value, operator, type}
    var tsiClient = new TsiClient();
    var rangeStart = new Date();
    rangeStart.setDate(rangeStart.getDate() - 30);
    var rangeEnd = new Date();
    rangeEnd.setDate(rangeEnd.getDate() + 7);

    var lineChart = new tsiClient.ux.LineChart(document.getElementById('chart'));
    var availability = new tsiClient.ux.AvailabilityChart(document.getElementById('availability'));

    // defaults
    var start = new Date();
    start.setDate(start.getDate() - 5);
    var end = new Date();
    end.setDate(end.getDate() + 2);

    // ON RENDER
    var renderLineGraph = async (from, to, timezone) => {

        if (!from) from = start;
        else start = from; // cache
        if (!to) to = end;
        else end = to; // cache
        if (!timezone) timezone = 'Local'

        var linechartTsqExpressions = await getLineChartExpressions(tsiClient, id, signals, filters, from, to);

        const url = window.location.host + "/api"
        var result = await tsiClient.server.getTsqResults("token", url, linechartTsqExpressions.map(function (ae) {
            return ae.toTsq()
        }))
        var transformedResult = tsiClient.ux.transformTsqResultsForVisualization(result, linechartTsqExpressions);
        var today = (new Date()).toISOString();

        // copied
        var events = [
            {
                "Today": [
                    {
                        [today]: {
                            'color': '#08172e',
                            'description': 'Now'
                        }
                    }
                ]
            }
        ];

        lineChart.render(transformedResult,
            {
                theme: 'light', grid: true, tooltip: true, legend: 'compact', yAxisState: 'stacked',
                noAnimate: true, includeDots: true, offset: timezone,
                includeEnvelope: true, dateLocale: 'en-AU', events: events,
            },
            linechartTsqExpressions);

    }

    availability.render([{ availabilityCount: { "": {} } }],
        { legend: 'hidden', theme: 'light', color: 'red', brushMoveEndAction: renderLineGraph, offset: 'Local', isCompact: true },
        {
            range:
            {
                from: rangeStart.toISOString(),
                to: rangeEnd.toISOString()
            },
            intervalSize: '1h'
        }
    );

    renderLineGraph(start, end, 'Local')
    availability.setBrush(start.valueOf(), end.valueOf(), true)

    return renderLineGraph;
}

async function getLineChartExpressions(tsiClient, id, signals, filters, from, to) {
    var scheme = new ColorScheme;
    scheme.from_hue(signals.length)
        .scheme('triade')
        .variation('soft');
    var colors = scheme.colors();
    let colourCount = 0;

    var tsx = "";
    if(filters) {
        Object.keys(filters).forEach(key => {
            if(filters[key]){
                if (tsx) {
                    tsx += " AND "
                }
                tsx += `($event.${key}.${filters[key].type} ${filters[key].operator} ${filters[key].value})`;
            } 
            else console.log("Ignoring null value")
        });
    }
    if(tsx) console.log(tsx)
    var filter = null;
    if(tsx.length > 0) {
        filter = { tsx }
    }

    var linechartTsqExpressions = [];
    signals.forEach((sig) => {
        const y = {};
        y[sig.Property] = {
            kind: 'numeric',
            value: { tsx: `$event.${sig.Property}` },
            filter,
            aggregation: { tsx: 'avg($value)' }
        };
        const x = new tsiClient.ux.TsqExpression(
            { timeSeriesId: [id] },
            y,
            { from, to, bucketSize: '6h' },
            '#' + colors[colourCount++], // color
            sig.Property);

        linechartTsqExpressions.push(x);
    });
    return linechartTsqExpressions;
}