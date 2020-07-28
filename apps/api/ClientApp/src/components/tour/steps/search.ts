import { ExtendedStep } from "../tourSteps";

const SearchButton: ExtendedStep = {
    target: ".tour-search-button",
    title: "The Search Button",
    content: "Click here to search for new Amphora.",
    navigateOnClick: "/search?term=weather",
};

const SearchBar: ExtendedStep = {
    target: "#search-bar",
    title: "The Search Bar",
    content:
        "The search bar lets you search for existing data across organisations. To view an Amphora, click on the name below.",
};

export const Steps = [SearchButton, SearchBar];
