import { ExtendedStep } from "../tourSteps";

const MenuButton: ExtendedStep = {
    title: "Create an Amphora",
    target: ".tour-create-button",
    navigateOnClick: "/create",
    content: "Click here to create your own Amphora",
};
const CreateAmphora: ExtendedStep = {
    target: ".tour-create",
    content:
        "An Amphora is a data container. By putting data in a container, you can apply permissions, charge an access fee, control terms of use, and more.",
};
const Name: ExtendedStep = {
    title: "Amphora Name",
    target: ".tour-create-name",
    content: "Choose a meaningful name for your new Amphora.",
};
const Description: ExtendedStep = {
    title: "Description",
    target: ".tour-create-description",
    content: "In human terms, describe the data you are packaging. Markdown is supported.",
};
const Price: ExtendedStep = {
    title: "Price",
    target: ".tour-create-price",
    content: "Choose a monthly subscription price. It can be $0.",
};


export const Steps = [MenuButton, CreateAmphora, Name, Description, Price];
