import { Action } from 'redux';

export const OPEN_BURGER_MENU = '[ui] OPEN BURGER MENU';
export const CLOSE_BURGER_MENU = '[ui] CLOSE BURGER MENU';

export const actionCreators = {
    open: (): Action => ({ type: OPEN_BURGER_MENU }),
    close: (): Action => ({ type: CLOSE_BURGER_MENU }),
}
