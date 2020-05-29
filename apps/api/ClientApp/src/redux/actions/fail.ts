import { Action } from 'redux';

const FAILED = true;
export interface OnFailedAction extends Action {
    failed: typeof FAILED;
    message: string;
}