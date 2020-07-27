import { ExtendedStep } from "../tourSteps";

const SearchButton: ExtendedStep = {
    target: ".tour-search-button",
    content: "Click here to discover Amphora.",
    navigateOnClick: "/search?term=weather",
};

const SearchBar: ExtendedStep = {
    target: "#search-bar",
    content:
        "The search bar lets you search for existing data across organisations. To view an Amphora, click on the name below.",
};

export const Steps = [SearchButton, SearchBar];
