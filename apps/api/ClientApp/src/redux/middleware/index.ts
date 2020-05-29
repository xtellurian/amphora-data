import { amphoraMiddleware } from './api/amphorae';
import { termsMiddleware } from './api/terms';
import { axiosUpdater } from './axiosUpdater';
import { logger, crashReporter } from './logger';
import { controlMenu } from './ui/burgerMenu';
import { toastsListener } from './ui/toaster';

export default [logger, crashReporter, axiosUpdater, ...amphoraMiddleware, ...termsMiddleware, controlMenu, toastsListener]