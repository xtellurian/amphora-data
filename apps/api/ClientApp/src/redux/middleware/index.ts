import { amphoraMiddleware } from './amphorae';
import { axiosUpdater } from './axiosUpdater';
import { logger, crashReporter } from './logger';
import { controlMenu } from './ui/burgerMenu';
import { alertsListener } from './ui/alertsListener';

export default [logger, crashReporter, axiosUpdater, ...amphoraMiddleware, controlMenu, alertsListener]