import { Action } from 'redux';

export const OPEN_BURGER_MENU = '[ui] OPEN BURGER MENU';
export const CLOSE_BURGER_MENU = '[ui] CLOSE BURGER MENU';

export const PRIMARY_MENU = "primary";

type MenuType = typeof PRIMARY_MENU;

export interface MenuAction extends Action {
    menu: MenuType;
}

export const actionCreators = {
    open: (menu: MenuType): MenuAction => ({ type: OPEN_BURGER_MENU, menu }),
    close: (menu: MenuType): MenuAction => ({ type: CLOSE_BURGER_MENU, menu }),
}
