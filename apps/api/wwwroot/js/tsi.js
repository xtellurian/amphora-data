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
    var renderLineGraph = async (from, to, timezone, bucketSize) => {

        if (!from) from = start;
        else start = from; // cache
        if (!to) to = end;
        else end = to; // cache
        if (!timezone) timezone = 'Local'
        if(!bucketSize) bucketSize = '2h';

        console.log(bucketSize)

        var linechartTsqExpressions = await getLineChartExpressions(tsiClient, id, signals, filters, from, to, bucketSize);

        const url = window.location.host + "/api"
        var result = await tsiClient.server.getTsqResults("token", url, linechartTsqExpressions.map(function (ae) {
            return ae.toTsq()
        }))
        var data = tsiClient.ux.transformTsqResultsForVisualization(result, linechartTsqExpressions);
        var todayDate = new Date();

        if(todayDate > from && todayDate < to ) {
            // now is between from and to
            var today = todayDate.toISOString();
            // adds today
            events = {};
            data.push({[`Events`]: events});
            var values = {};
            events[`Days`] = values;
    
            let measures = {};
            measures['Today'] = 'Now';
            values[today] = measures

            var eventValueMapping = {
                Today: {
                    color: '#BC312A'
                }
            }
            
            linechartTsqExpressions.push({dataType: 'events', valueMapping: eventValueMapping, height: 10, eventElementType: 'diamond', onElementClick: null})
        }

        lineChart.render(data,
            {
                theme: 'light', grid: true, tooltip: true, legend: 'compact', yAxisState: 'stacked',
                noAnimate: true, includeDots: false, offset: timezone, includeEnvelope: true, dateLocale: 'en-AU',
                strings: {'Display Grid': 'Show Table'},
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

async function getLineChartExpressions(tsiClient, id, signals, filters, from, to, bucketSize) {
    var scheme = new ColorScheme;

    scheme.from_hue(Math.max(signals.length, 10))
        .scheme('tetrade')
        .variation('pastel');
    var colors = shuffle(scheme.colors()).slice(0, signals.length);
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
        var kind = 'numeric';
        var alias = sig.Property;
        if(sig.Attributes && sig.Attributes.Attributes)
        {
            var a = sig.Attributes.Attributes;
            if(a.Units)
            {
                // check units uppercase
                alias += ` (${a.Units})`
            }
            if(a.units)
            {
                // check units lowercase
                alias += ` (${a.units})`
            }
        }
        y[sig.Property] = {
            kind,
            value: { tsx: `$event.${sig.Property}` },
            filter,
            aggregation: { tsx: 'avg($value)' }
        };
        const x = new tsiClient.ux.TsqExpression(
            { timeSeriesId: [id] },
            y,
            { from, to, bucketSize },
            '#' + colors[colourCount++], // color
            alias);

        linechartTsqExpressions.push(x);
    });
    return linechartTsqExpressions;
}

/**
 * Shuffles array in place.
 * @param {Array} a items An array containing the items.
 */
function shuffle(a) {
    var j, x, i;
    for (i = a.length - 1; i > 0; i--) {
        j = Math.floor(Math.random() * (i + 1));
        x = a[i];
        a[i] = a[j];
        a[j] = x;
    }
    return a;
}

