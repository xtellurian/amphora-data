function autocompleteGeoSearch(token, url, latSelector, lonSelector) {

    const autoCompletejs = new autoComplete({
        data: {
            src: async function () {
                // Loading placeholder text
                document.querySelector("#autoComplete").setAttribute("placeholder", "Loading...");
                // Fetch External Data Source

                let query = document.querySelector("#autoComplete").value;
                const source = await fetch(`${url}?query=${query}`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                        // 'Content-Type': 'application/x-www-form-urlencoded',
                    }
                });
                const data = await source.json();
                // Returns Fetched data
                data.results.forEach(function (element) {
                    // flatten a bit
                    element.freeformAddress = element.address.freeformAddress
                });
                //console.log(data.results);
                return data.results;
            },
            key: ["freeformAddress"],
            cache: false
        },
        sort: function (a, b) {
            if (a.match < b.match) {
                return -1;
            }
            if (a.match > b.match) {
                return 1;
            }
            return 0;
        },
        query: {
            manipulate: function (query) {
                return query.replace("pizza", "burger");
            }
        },
        placeHolder: "Geo Search",
        selector: "#autoComplete",
        threshold: 3,
        debounce: 250,
        //searchEngine: "loose",
        customEngine: (query, record) => {
            return record;
        },
        highlight: true,
        maxResults: 10,
        resultsList: {
            render: true,
            container: function (source) {
                source.setAttribute("id", "autoComplete_results_list");
            },
            destination: document.querySelector("#autoComplete"),
            position: "afterend",
            element: "ul",
        },
        resultItem: {
            content: function (data, source) {
                source.innerHTML = data.match;
            },
            element: "li",
        },
        noResults: function () {
            const result = document.createElement("li");
            result.setAttribute("class", "no_result");
            result.setAttribute("tabindex", "1");
            result.innerHTML = "No Results";
            document.querySelector("#autoComplete_results_list").appendChild(result);
        },
        onSelection: function (feedback) {
            const selection = feedback.selection.value.freeformAddress;
            // Render selected choice to selection div
            document.querySelector(".selection").innerHTML = selection;
            // Clear Input
            document.querySelector("#autoComplete").value = "";
            // Change placeholder with the selected value
            document.querySelector("#autoComplete").setAttribute("placeholder", selection);
            // set lat and lon
            if (latSelector && lonSelector) {
                document.querySelector(latSelector).setAttribute("value", feedback.selection.value.position.lat);
                document.querySelector(lonSelector).setAttribute("value", feedback.selection.value.position.lon);
            }
            // Concole log autoComplete data feedback
        },
    });
}