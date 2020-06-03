import { searchClient } from "../../../clients/amphoraApiClient";
import * as searchAmphoraActions from '../../actions/search/searchAmphora'

const searchAmphora = (store: any) => (next: any) => (
  action: searchAmphoraActions.AllSearchActions
) => {
  next(action);
  if (action.type === searchAmphoraActions.SEARCH_AMPHORA) {
    const searchAction = action as searchAmphoraActions.SearchAmphoraAction;
    searchClient
      .searchSearchAmphorae(searchAction.query.term)
      .then((r) => store.dispatch(searchAmphoraActions.actionCreators.recieveSearchResults(searchAction.query, r.data)))
      .catch((e) => store.dispatch(searchAmphoraActions.actionCreators.fail(e)));
  }
};

export const searchMiddleware = [searchAmphora];
