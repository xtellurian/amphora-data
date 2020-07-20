import { Actions } from "react-amphora";
import * as toast from "../components/molecules/toasts";

export const toastOnActionResult = (actionResult: Actions.ActionResult) => {
    if (actionResult.error) {
        toast.error({ text: `${actionResult.error}` });
    } else if (actionResult.payload) {
        toast.success({
            text: actionResult.type,
        });
    } else {
        toast.info({
            text: actionResult.type,
        });
    }
};
