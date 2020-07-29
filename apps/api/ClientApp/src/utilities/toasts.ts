import { Actions } from "react-amphora";
import * as toast from "../components/molecules/toasts";

export const toastOnActionResult = (actionResult: Actions.ActionResult) => {
    if (actionResult.error) {
        toast.error(
            { text: `${actionResult.error}` },
            {
                autoClose: 1000,
            }
        );
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
