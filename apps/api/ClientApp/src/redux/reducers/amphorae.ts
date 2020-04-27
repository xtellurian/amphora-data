import { UPDATE_AMPHORAE_LIST } from "../actions/amphorae";
import { IAction } from "../actions/action";

export function amphoraeReducer(amphorae = [], action: IAction) {

    switch (action.type) {

        case UPDATE_AMPHORAE_LIST:
            // console.log(action)
            if(action.payload){
                return action.payload;
            } else
            {
                return []
            }

        default:
            return amphorae;
    }
}