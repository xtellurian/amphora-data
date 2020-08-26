import { reactors } from "./reactors";
import { logger, crashReporter } from "./logger";
import { controlMenu } from "./ui/burgerMenu";

export default [
    logger,
    crashReporter,
    ...reactors,
    controlMenu,
];
