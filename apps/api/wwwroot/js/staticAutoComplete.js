function staticAutocomplete(inputSelector, content, key, callback) {
    const auto = new autoComplete({
        data: {                              // Data src [Array, Function, Async] | (REQUIRED)
          src: content,
          key: [key],
          cache: true
        },
        placeHolder: "Search...",                    // Place Holder text                 | (Optional)
        selector: inputSelector,           // Input field selector              | (Optional)
        threshold: 3,                        // Min. Chars length to start Engine | (Optional)
        debounce: 300,                       // Post duration for engine to start | (Optional)
        searchEngine: "loose",           // Search Engine type/mode           | (Optional)
        // customEngine: (query, record) => {
        //     return record;
        // },             
        resultsList: {                       // Rendered results list object      | (Optional)
            render: true,
            container: source => {
                source.setAttribute("id", "autoComplete_org_list");
                const resultsListID = "food_List";
                // return resultsListID;
            },
            destination: document.querySelector(inputSelector),
            position: "afterend",
            element: "ul"
        },
        maxResults: 5,                         // Max. number of rendered results | (Optional)
        highlight: true,                       // Highlight matching results      | (Optional)
        resultItem: {                          // Rendered result item            | (Optional)
            content: (data, source) => {
                console.log(source)
                console.log(data)
                source.innerHTML = data.match;
            },
            element: "li"
        },
        noResults: () => {                     // Action script on noResults      | (Optional)
            const result = document.createElement("li");
            result.setAttribute("class", "no_result");
            result.setAttribute("tabindex", "1");
            result.innerHTML = "No Results";
            document.querySelector("#autoComplete_org_list").appendChild(result);
        },
        onSelection: callback
    });
}