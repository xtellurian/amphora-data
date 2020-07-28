import { ExtendedStep } from "../tourSteps";

const MyAmphoraButton: ExtendedStep = {
    target: ".tour-my-amphora-button",
    navigateOnClick: "/amphora",
    content: "Click here to view your data.",
};

const MyAmphora: ExtendedStep = {
    title: "My Data",
    target: ".tour-my-amphora",
    content:
        "Every user and organisation can access their own data. Here you can toggle between viewing data you've created or purchased, either by you or others in your organisation",
};

export const Steps = [MyAmphoraButton, MyAmphora];
