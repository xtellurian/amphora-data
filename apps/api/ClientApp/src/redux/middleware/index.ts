import { reactors } from "./reactors";
import { mapsMiddleware } from "./api/maps";
import { logger, crashReporter } from "./logger";
import { controlMenu } from "./ui/burgerMenu";

export default [
    logger,
    crashReporter,
    ...reactors,
    ...mapsMiddleware,
    controlMenu,
];
