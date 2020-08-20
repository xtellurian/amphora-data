import { Actions } from "react-amphora";
import * as toast from "../components/molecules/toasts";
import { Settings } from "../redux/state/Settings";

export const toastOnActionResult = (
    actionResult: Actions.ActionResult,
    settings: Settings
) => {
    if (actionResult.error) {
        toast.error(
            { text: `${actionResult.error}` },
            {
                autoClose: 1000,
            }
        );
    } else if (!settings.showDebuggingNotifications) {
        return;
    } else if (actionResult.payload) {
        toast.success(
            {
                text: actionResult.type,
            },
            {
                autoClose: 1000,
            }
        );
    } else {
        toast.info(
            {
                text: actionResult.type,
            },
            {
                autoClose: 1000,
            }
        );
    }
};
