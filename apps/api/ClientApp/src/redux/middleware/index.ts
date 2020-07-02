import { termsMiddleware } from "./api/terms";
import { signalsMiddleware } from "./api/signals";
import { selfMiddleware } from "./api/self";
import { searchMiddleware } from "./api/search";
import { mapsMiddleware } from "./api/maps";
import { reactors } from "./reactors";
import { logger, crashReporter } from "./logger";
import { controlMenu } from "./ui/burgerMenu";
import { toastsListener } from "./ui/toaster";

export default [
    logger,
    crashReporter,
    ...reactors,
    ...selfMiddleware,
    ...termsMiddleware,
    ...signalsMiddleware,
    ...searchMiddleware,
    ...mapsMiddleware,
    controlMenu,
    toastsListener,
];
