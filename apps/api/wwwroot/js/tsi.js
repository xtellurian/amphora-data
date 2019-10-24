function tsi(signals, response) {
    // signals should be a list of SignalMOdel
//     CreatedDate: "2019-10-03T23:59:54.469267Z"
// Id: "key|Numeric"
// IsNumeric: true
// IsString: false
// Property: "key"
// ValueType: "Numeric"
// ttl: -1
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
        y[sig.Property] = {};
        const x = new tsiClient.ux.TsqExpression(
            {},
            y,
            {
                from: startDate,
                to: endDate
            },
            '#' + colors[count++], // color
            sig.Property); 
        linechartTsqExpressions.push(x);
    });

    console.log("linechartTsqExpressions");
    console.log(linechartTsqExpressions);
    console.log("response");
    console.log(response);
    var transformedResult = tsiClient.ux.transformTsqResultsForVisualization(response, linechartTsqExpressions);
    console.log("transformedResult");
    console.log(transformedResult);
    var lineChart = new tsiClient.ux.LineChart(document.getElementById('chart'));
    const offset = new Date().getTimezoneOffset() * -1; // unsure why TSI needs this flipped...
    lineChart.render(transformedResult, { theme: 'light', grid: true, tooltip: true, legend: 'compact', yAxisState: 'stacked', offset }, linechartTsqExpressions);
}