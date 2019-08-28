function tsi(cols, response) {

    var scheme = new ColorScheme;
    var tsiClient = new TsiClient();
    var startDate = new Date();
    startDate.setDate(startDate.getDate() - 7);
    endDate = new Date();
    var linechartTsqExpressions = [];

    //var cols = @Html.Raw(@Newtonsoft.Json.JsonConvert.SerializeObject(Model.Domain.GetDatumMembers()));
    //var response = @Html.Raw(@Model.QueryResponse);

    scheme.from_hue(cols.length)
        .scheme('triade')
        .variation('soft');


    var colors = scheme.colors();
    let count = 0;
    cols.forEach((col) => {
        linechartTsqExpressions.push(new tsiClient.ux.TsqExpression(
            {},
            { Avg: {} },
            {
                from: startDate,
                to: endDate
            },
            '#' + colors[count++], // color
            col.Name)); // 
    });

    console.log("linechartTsqExpressions");
    console.log(linechartTsqExpressions);
    console.log("response");
    console.log(response);
    var transformedResult = tsiClient.ux.transformTsqResultsForVisualization(response, linechartTsqExpressions);
    console.log("transformedResult");
    console.log(transformedResult);
    var lineChart = new tsiClient.ux.LineChart(document.getElementById('chart'));
    lineChart.render(transformedResult, { theme: 'light', grid: true, tooltip: true, legend: 'shown', yAxisState: 'shared' }, linechartTsqExpressions);
}