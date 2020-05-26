import { CLOSE_BURGER_MENU, OPEN_BURGER_MENU, MenuAction } from '../../actions/plugins/burgerMenu';
import { action as toggleMenu } from 'redux-burger-menu';

export const controlMenu = (store: any) => (next: any) => (action: MenuAction) => {
    next(action);
    if (action.type === OPEN_BURGER_MENU) {
        store.dispatch(toggleMenu(true, action.menu));
    } else if (action.type === CLOSE_BURGER_MENU) {
        store.dispatch(toggleMenu(false, action.menu));
    }
};