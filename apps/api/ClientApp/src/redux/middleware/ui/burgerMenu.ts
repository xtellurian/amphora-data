import { CLOSE_BURGER_MENU, OPEN_BURGER_MENU } from '../../actions/plugins/burgerMenu';
import { action as toggleMenu } from 'redux-burger-menu';
import { Action } from 'redux';

export const controlMenu = (store: any) => (next: any) => (action: Action) => {
    next(action);
    if (action.type === OPEN_BURGER_MENU) {
        store.dispatch(toggleMenu(true))
    } else if (action.type === CLOSE_BURGER_MENU) {
        store.dispatch(toggleMenu(false))
    }
};