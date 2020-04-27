import { IAction } from "./action";
import { AmphoraModel } from "../AmphoraState";

export const GET_MY_CREATED_AMPHORAE = '[amphorae] GET MY CREATED';
export const GET_MY_CREATED_AMPHORAE_SUCCESS = `${GET_MY_CREATED_AMPHORAE} SUCCESS`;
export const GET_MY_CREATED_AMPHORAE_ERROR = `${GET_MY_CREATED_AMPHORAE} ERROR`;

export const UPDATE_AMPHORAE_LIST = '[amphorae] Update List';


interface GetMyCreatedAmphoraeAction extends IAction {
    type: typeof GET_MY_CREATED_AMPHORAE,
    startDateIndex: number
}

interface RecieveMyCreatedAmphoraAction extends IAction {
    type: typeof GET_MY_CREATED_AMPHORAE_SUCCESS,
    startDateIndex: number,
    payload: AmphoraModel[]
}

export const actionCreators = {

    getMyCreatedAmphorae: (startDateIndex: number): GetMyCreatedAmphoraeAction => ({
        type: GET_MY_CREATED_AMPHORAE,
        startDateIndex
    }),

    updateAmphoraeList: (data: string[]) => ({
        type: UPDATE_AMPHORAE_LIST,
        payload: data
    })
}

export type KnownAction = GetMyCreatedAmphoraeAction | RecieveMyCreatedAmphoraAction | RecieveMyCreatedAmphoraAction;