function organisationsAutocomplete(elementId, resultsId, destinationId, url, onSelectionCallback) {
    if (!destinationId) {
        destinationId = elementId;
    }

    const autoCompletejs = new autoComplete({
        data: {
            src: async function () {
                // Loading placeholder text
                // document.querySelector(`#${elementId}`).setAttribute("placeholder", "Loading...");
                // Fetch External Data Source

                let term = document.querySelector(`#${elementId}`).value;
                const source = await fetch(`${url}?term=${term}`, {});
                const data = await source.json();
                // Returns Fetched data
                return data;
            },
            key: ["name"],
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
        placeHolder: "Organisation Name",
        selector: `#${elementId}`,
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
                source.setAttribute("id", resultsId);
            },
            destination: document.querySelector(`#${destinationId}`),
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
            result.innerHTML = "We can't find any existing organisations matching that name.";
            document.querySelector(`#${resultsId}`).appendChild(result);
        },
        onSelection: onSelectionCallback,
        // onSelection: function (feedback) {
        //     const orgId = feedback.selection.value.id;
        //     window.location.href = `/Organisations/RequestToJoin?id=${orgId}`
        // },
    });
}