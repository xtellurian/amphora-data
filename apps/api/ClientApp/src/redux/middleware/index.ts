import { reactors } from "./reactors";
import { mapsMiddleware } from "./api/maps";
import { logger, crashReporter } from "./logger";
import { controlMenu } from "./ui/burgerMenu";
import { toastsListener } from "./ui/toaster";

export default [
    logger,
    crashReporter,
    ...reactors,
    ...mapsMiddleware,
    controlMenu,
    toastsListener,
];
