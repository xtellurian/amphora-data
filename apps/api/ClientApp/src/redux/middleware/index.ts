import { amphoraMiddleware } from './amphorae';
import { axiosUpdater } from './axiosUpdater';
import { logger, crashReporter } from './logger';
import { controlMenu } from './ui/burgerMenu';
import { toastsListener } from './ui/toaster';

export default [logger, crashReporter, axiosUpdater, ...amphoraMiddleware, controlMenu, toastsListener]