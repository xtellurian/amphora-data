import { ExtendedStep } from "../tourSteps";

const MenuButton: ExtendedStep = {
    target: ".tour-create-button",
    navigateOnClick: "/create",
    content: "Click here to create your own Amphora",
};

const Name: ExtendedStep = {
    target: ".tour-create-name",
    content: "Choose name for your data container.",
};
const Description: ExtendedStep = {
    target: ".tour-create-description",
    content: "In human terms, describe the data you are packaging.",
};
const Price: ExtendedStep = {
    target: ".tour-create-price",
    content: "Choose a monthly subscription price. It can be 0.",
};

const CreateAmphora: ExtendedStep = {
    target: ".tour-create",
    content:
        "An Amphora is a data container. By putting data in a container, you can apply permissions, charge an access fee, control terms of use, and more.",
};

export const Steps = [MenuButton, CreateAmphora, Name, Description, Price];
