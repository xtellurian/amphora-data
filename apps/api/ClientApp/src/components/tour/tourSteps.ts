import { Step } from "react-joyride";

export interface ExtendedStep extends Step {
    navigateOnClick?: string;
}

export const Welcome: ExtendedStep = {
    target: "#welcome",
    content: "Hi there :) We're going to take a short tour of Amphora Data.",
};

export const Search: ExtendedStep = {
    target: "#search-button",
    content: "This is the search button",
    navigateOnClick: "/search"
};

export const SearchBar: ExtendedStep = {
    target: "#search-bar",
    content: "This is the search bar, where you can find data you need",
};

export const steps: ExtendedStep[] = [Welcome, Search, SearchBar];
