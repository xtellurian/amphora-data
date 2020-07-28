import { Step } from "react-joyride";
import Steps from "./steps";
export interface ExtendedStep extends Step {
    navigateOnClick?: string;
    finalStep?: boolean;
}

const Welcome: ExtendedStep = {
    target: ".welcome-anchor",
    title: "Welcome to Amphora Data",
    navigateOnClick: "/",
    content:
        "We're going to take a quick your around Amphora Data. Just click on these icons to view the tips and advance the tour. Let's go!",
};
const RedoTour: ExtendedStep = {
    target: ".tour-tour-button",
    title: "Redo this tour",
    navigateOnClick: "/",
    content: "You can redo this tour at any time",
};
const Final: ExtendedStep = {
    target: ".welcome-anchor",
    navigateOnClick: "/",
    content: "That's it!",
    finalStep: true,
};

export const steps: ExtendedStep[] = [Welcome, ...Steps, RedoTour, Final];
