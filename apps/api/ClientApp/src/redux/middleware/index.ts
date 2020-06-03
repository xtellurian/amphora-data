import { amphoraMiddleware } from "./api/amphorae";
import { termsMiddleware } from "./api/terms";
import { signalsMiddleware } from "./api/signals";
import { selfMiddleware } from "./api/self";
import { searchMiddleware } from "./api/search";
import { axiosUpdater } from "./reactors/axiosUpdater";
import { onLogin } from "./reactors/onLogin";
import { logger, crashReporter } from "./logger";
import { controlMenu } from "./ui/burgerMenu";
import { toastsListener } from "./ui/toaster";

export default [
  logger,
  crashReporter,
  axiosUpdater,
  onLogin,
  ...selfMiddleware,
  ...amphoraMiddleware,
  ...termsMiddleware,
  ...signalsMiddleware,
  ...searchMiddleware,
  controlMenu,
  toastsListener,
];
