import { Actions } from "react-amphora";
import * as toast from "../components/molecules/toasts";

export const toastOnActionResult = (actionResult: Actions.ActionResult) => {
    toast.info({
        text: actionResult.type,
    });
};
