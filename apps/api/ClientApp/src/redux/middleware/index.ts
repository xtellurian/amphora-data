import { amphoraMiddleware } from "./api/amphorae";
import { termsMiddleware } from "./api/terms";
import { signalsMiddleware } from "./api/signals";
import { selfMiddleware } from "./api/self";
import { searchMiddleware } from "./api/search";
import { mapsMiddleware } from "./api/maps";
import { permissionsMiddleware } from "./api/permissions";
import { reactors } from "./reactors";
import { logger, crashReporter } from "./logger";
import { controlMenu } from "./ui/burgerMenu";
import { toastsListener } from "./ui/toaster";

export default [
    logger,
    crashReporter,
    ...reactors,
    ...selfMiddleware,
    ...permissionsMiddleware,
    ...amphoraMiddleware,
    ...termsMiddleware,
    ...signalsMiddleware,
    ...searchMiddleware,
    ...mapsMiddleware,
    controlMenu,
    toastsListener,
];
