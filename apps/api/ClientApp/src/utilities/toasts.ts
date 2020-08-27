import { Actions } from "react-amphora";
import * as toast from "../components/molecules/toasts";
import { Settings } from "../redux/state/Settings";
import { User } from "oidc-client";

export const toastOnSignIn = (user: User) => {
    toast.success(
        {
            text: `Welcome ${user.profile.name}`,
        },
        {
            autoClose: 1000,
        }
    );
};
export const toastOnSignInError = (e: any) => {
    toast.warning(
        {
            text: `Failed to sign in. Retrying...`,
        },
        {
            autoClose: 1000,
        }
    );
};

export const toastOnAction = (action: Actions.Action, settings: Settings) => {
    if (settings.showDebuggingNotifications) {
        toast.info(action.type);
    }
};
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
