import * as React from "react";
import { BasicAmphora } from "amphoradata";
import { SearchContext } from "react-amphora";
import { useLocation, useHistory } from "react-router";
import { TextInput } from "../../molecules/inputs";
import { LoadingState, EmptyState } from "../../molecules/empty";
import { Table } from "../../molecules/tables/Table";

const getSearchParams = (queryString: string): URLSearchParams => {
    return new URLSearchParams(queryString);
};
const getSearchTerm = (queryString: string): string => {
    return getSearchParams(queryString).get("term") || "";
};

const columns = [
    { key: "name", name: "Amphora Name" },
    // { key: 'createdDate', name: 'Date Created' },
    { key: "price", name: "Price per Month" },
];

interface SearchPageState {
    loading: boolean;
    term: string;
    results: BasicAmphora[];
}
export const SearchSection: React.FC = (props) => {
    const actions = SearchContext.useSearchDispatch();
    const context = SearchContext.useSearchState();
    const location = useLocation();
    const history = useHistory();

    const [state, setState] = React.useState<SearchPageState>({
        loading: true,
        results: [],
        term: getSearchTerm(location.search),
    });

    const setTerm = (term?: string) => {
        const searchParams = getSearchParams(location.search);
        searchParams.set("term", term || "");
        history.replace({
            pathname: location.pathname,
            search: searchParams.toString(),
        });
        setState({
            term: term || "",
            loading: true,
            results: state.results,
        });
    };

    React.useEffect(() => {
        if (context.isAuthenticated) {
            setState({
                loading: true,
                term: state.term,
                results: state.results,
            });
            actions.dispatch({
                type: "search:execute",
                payload: { term: state.term },
            });
        }
    }, [context.isAuthenticated, state.term]);

    // react to changes in results
    React.useEffect(() => {
        console.log("results change triggered");
        setState({
            loading: context.isLoading || false,
            term: state.term,
            results: context.results,
        });
    }, [context.results]);

    return (
        <React.Fragment>
            <h2>Search</h2>
            <TextInput
                id="search-bar"
                value={getSearchParams(location.search).get("term") || ""}
                onComplete={(t) => setTerm(t)}
                label=""
                placeholder="Search for some data"
            />

            {state.loading && <LoadingState />}
            {!state.loading && state.results.length === 0 && (
                <EmptyState>There were no results matching search.</EmptyState>
            )}
            {!state.loading && state.results.length > 0 && (
                <Table
                    onRowClicked={(r) => history.push(`/search/detail/${r.id}?term=${state.term}`)}
                    columns={columns}
                    rowGetter={(i: number) => state.results[i]}
                    rowCount={Math.min(12, state.results.length)}
                />
            )}
        </React.Fragment>
    );
};
