
async function tsi(id, signals) {

    var scheme = new ColorScheme;
    var tsiClient = new TsiClient();
    var startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    endDate = new Date();
    endDate.setDate(endDate.getDate() + 7);
    var linechartTsqExpressions = [];

    scheme.from_hue(signals.length)
        .scheme('triade')
        .variation('soft');


    var colors = scheme.colors();
    let count = 0;

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
            { from: startDate, to: endDate, bucketSize: '6h' },
            '#' + colors[count++], // color
            sig.Property);

        linechartTsqExpressions.push(x);
    });

    const url = window.location.host + "/api"
    console.log(url)
    var result = await tsiClient.server.getTsqResults("token", url, linechartTsqExpressions.map(function (ae) { 
        return ae.toTsq() 
    }))
    var transformedResult = tsiClient.ux.transformTsqResultsForVisualization(result, linechartTsqExpressions);

    console.log("linechartTsqExpressions");
    console.log(linechartTsqExpressions);

    var lineChart = new tsiClient.ux.LineChart(document.getElementById('chart'));
    const offset = new Date().getTimezoneOffset() * -1; // unsure why TSI needs this flipped...
    lineChart.render(transformedResult, { theme: 'light', grid: true, tooltip: true, legend: 'compact', yAxisState: 'stacked', offset }, linechartTsqExpressions);
}