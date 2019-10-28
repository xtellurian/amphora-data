
async function tsi(id, signals) {


    var tsiClient = new TsiClient();
    var rangeStart = new Date();
    rangeStart.setDate(rangeStart.getDate() - 30);
    var rangeEnd = new Date();
    rangeEnd.setDate(rangeEnd.getDate() + 7);

    var lineChart = new tsiClient.ux.LineChart(document.getElementById('chart'));
    var availability = new tsiClient.ux.AvailabilityChart(document.getElementById('availability'));

    // ON RENDER
    var renderLineGraph = async (from, to, timezone) => {

        var linechartTsqExpressions = await getLineChartExpressions(tsiClient, signals, from, to);

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

    var initialStart = new Date();
    initialStart.setDate(initialStart.getDate() - 5);
    var initialEnd = new Date();
    initialEnd.setDate(initialEnd.getDate() + 2);
    renderLineGraph(initialStart, initialEnd, 'Local')
    availability.setBrush(initialStart.valueOf(), initialEnd.valueOf(), true)
}

async function getLineChartExpressions(tsiClient, signals, from, to) {
    var scheme = new ColorScheme;
    scheme.from_hue(signals.length)
        .scheme('triade')
        .variation('soft');
    var colors = scheme.colors();
    let count = 0;

    var linechartTsqExpressions = [];
    signals.forEach((sig) => {
        const y = {};
        y[sig.Property] = {
            kind: 'numeric',
            value: { tsx: `$event.${sig.Property}` },
            filter: null,
            aggregation: { tsx: 'avg($value)' }
        };
        const x = new tsiClient.ux.TsqExpression(
            { timeSeriesId: [id] },
            y,
            { from, to, bucketSize: '6h' },
            '#' + colors[count++], // color
            sig.Property);

        linechartTsqExpressions.push(x);
    });
    return linechartTsqExpressions;
}