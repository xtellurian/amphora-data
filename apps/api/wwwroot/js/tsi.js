
async function tsi(id, signals) {


    var tsiClient = new TsiClient();
    var startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    endDate = new Date();
    endDate.setDate(endDate.getDate() + 7);

    var linechartTsqExpressions = await getLineChartExpressions(tsiClient, signals, startDate, endDate);


    const url = window.location.host + "/api"
    console.log(url)
    var result = await tsiClient.server.getTsqResults("token", url, linechartTsqExpressions.map(function (ae) {
        return ae.toTsq()
    }))
    var transformedResult = tsiClient.ux.transformTsqResultsForVisualization(result, linechartTsqExpressions);

    var lineChart = new tsiClient.ux.LineChart(document.getElementById('chart'));
    var availability = new tsiClient.ux.AvailabilityChart(document.getElementById('availability'));

    var brushMoveEndAction = async (from, to, timezone) => {
        var linechartTsqExpressions = await getLineChartExpressions(tsiClient, signals, from, to);

        const url = window.location.host + "/api"
        var result = await tsiClient.server.getTsqResults("token", url, linechartTsqExpressions.map(function (ae) {
            return ae.toTsq()
        }))
        var transformedResult = tsiClient.ux.transformTsqResultsForVisualization(result, linechartTsqExpressions);

        var today = (new Date()).toISOString();
        var statesOrEvents = [
            {"Component States" : // This key is shown in the legend
                [
                    {
                        [today] : {
                            'color': '#D869CB',
                            'description' : 'Now'
                        }
                    }
                ]
            }
        ];

        //transformedResult.push(statesOrEvents)
        // linechartTsqExpressions.push({dataType: 'events', height: 50, eventElementType: 'teardrop'},)
        lineChart.render(transformedResult,
            {
                theme: 'light', grid: true, tooltip: true, legend: 'compact', yAxisState: 'stacked',
                noAnimate: true, includeDots: true, offset: timezone,
                includeEnvelope: true, dateLocale: 'en-AU', //events: statesOrEvents
            },
            linechartTsqExpressions);

    }

    availability.render([{ availabilityCount: { "": {} } }],
        { legend: 'hidden', theme: 'light', color: 'red', brushMoveEndAction: brushMoveEndAction, offset: 'Local', isCompact: true },
        {
            range:
            {
                from: startDate.toISOString(),
                to: endDate.toISOString()
            },
            intervalSize: '1h'
        }
    );

    // render empty
    lineChart.render(transformedResult,
        { theme: 'light', grid: true, tooltip: true, legend: 'compact', yAxisState: 'stacked', offset: 'Local' });
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