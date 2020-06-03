import { Action } from "redux";
import { BasicAmphora } from "amphoradata";
import { OnFailedAction, toMessage } from "../fail";
import { SearchQuery } from "../../state/search";

export const SEARCH_AMPHORA = "[search] AMPHORA";
export const SEARCH_AMPHORA_SUCCESS = `${SEARCH_AMPHORA} SUCCESS`;
export const SEARCH_AMPHORA_FAIL = `${SEARCH_AMPHORA} FAIL`;

export interface SearchAmphoraAction extends Action {
  type: typeof SEARCH_AMPHORA;
  query: SearchQuery;
}

export interface SearchAmphoraSuccessAction extends Action {
  type: typeof SEARCH_AMPHORA_SUCCESS;
  query: SearchQuery;
  payload: BasicAmphora[];
}

export const actionCreators = {
  // listing amphora
  searchAmphora: (query: SearchQuery): SearchAmphoraAction => ({
    type: SEARCH_AMPHORA,
    query,
  }),
  recieveSearchResults: (
    query: SearchQuery,
    payload: BasicAmphora[]
  ): SearchAmphoraSuccessAction => ({
    type: SEARCH_AMPHORA_SUCCESS,
    query,
    payload,
  }),

  fail: (e: any): OnFailedAction => ({
    type: SEARCH_AMPHORA_FAIL,
    message: `Search failed, ${toMessage(e)}`,
    failed: true,
  }),
};

export type AllSearchActions =
  | SearchAmphoraAction
  | SearchAmphoraSuccessAction
  | OnFailedAction;
