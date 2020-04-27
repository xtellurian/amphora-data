import { getAmphoraFlow, processAmphoraeCollection } from './amphorae';
import { apiMiddleware } from './api';
import { logger, crashReporter } from './logger';

export default [getAmphoraFlow, processAmphoraeCollection, apiMiddleware, logger, crashReporter]