import { getAmphoraFlow, processAmphoraeCollection } from './amphorae';
import {axiosUpdater} from './axiosUpdater';
import { apiMiddleware } from './api';
import { logger, crashReporter } from './logger';

export default [ logger, crashReporter, axiosUpdater, getAmphoraFlow, processAmphoraeCollection, apiMiddleware]