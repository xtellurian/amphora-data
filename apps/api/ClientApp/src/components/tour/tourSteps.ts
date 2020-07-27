import { Step } from "react-joyride";
import Steps from "./steps";
export interface ExtendedStep extends Step {
    navigateOnClick?: string;
    finalStep?: boolean;
}

const Welcome: ExtendedStep = {
    target: ".welcome-anchor",
    title: "hello",
    navigateOnClick: "/",
    content:
        "We're going to take a quick your around Amphora Data. Just click on these icons to view the tips and advance the tour. Let's go!",
};

const FinalStep: ExtendedStep = {
    target: ".welcome-anchor",
    navigateOnClick: "/",
    content: "That's the tour, thanks for playing.",
    finalStep: true,
};

export const steps: ExtendedStep[] = [Welcome, ...Steps, FinalStep];
