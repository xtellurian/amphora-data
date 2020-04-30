import { listAmphora } from './amphorae';
import { axiosUpdater } from './axiosUpdater';
import { logger, crashReporter } from './logger';
import { controlMenu } from './ui/burgerMenu';

export default [logger, crashReporter, axiosUpdater, listAmphora, controlMenu]