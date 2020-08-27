import * as React from "react";
import { BasicAmphora } from "amphoradata";
import { SearchContext } from "react-amphora";
import { useLocation, useHistory } from "react-router";
import { TextInput } from "../../molecules/inputs";
import { LoadingState, EmptyState } from "../../molecules/empty";
import { Table } from "../../molecules/tables/Table";
import { PaginationComponent } from "../../molecules/pagination/Pagination";

const perPage = 32;
const getSearchParams = (queryString: string): URLSearchParams => {
    return new URLSearchParams(queryString);
};
const getSearchTerm = (queryString: string): string => {
    return getSearchParams(queryString).get("term") || "";
};
const getPage = (queryString: string): number => {
    const p = getSearchParams(queryString).get("page") || "1";
    const np = parseInt(p);
    if (isNaN(np)) {
        return 1;
    } else {
        return np;
    }
};

const columns = [
    { key: "name", name: "Amphora Name" },
    // { key: 'createdDate', name: 'Date Created' },
    { key: "price", name: "Price per Month" },
];

interface SearchPageState {
    loading: boolean;
    term: string;
    page: number;
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
        page: getPage(location.search),
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
            page: state.page,
        });
    };

    // page effect
    React.useEffect(() => {
        const page = getPage(location.search);
        if (state.page !== page) {
            setState({
                ...state,
                page,
            });
        }
    });

    React.useEffect(() => {
        if (context.isAuthenticated) {
            setState({
                loading: true,
                term: state.term,
                results: state.results,
                page: state.page,
            });
            actions.dispatch({
                type: "search:execute",
                payload: {
                    term: state.term,
                    skip: Math.max(state.page - 1, 0) * perPage,
                    take: perPage,
                },
            });
        }
    }, [context.isAuthenticated, state.term, state.page]);

    // react to changes in results
    React.useEffect(() => {
        setState({
            loading: context.isLoading || false,
            term: state.term,
            results: context.results,
            page: state.page,
        });
    }, [context.results]);

    let nPages = state.page;
    if (state.results.length === perPage) {
        nPages = nPages + 1;
    }

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
                <React.Fragment>
                    <Table
                        onRowClicked={(r) =>
                            history.push(
                                `/search/detail/${r.id}?term=${state.term}`
                            )
                        }
                        columns={columns}
                        rowGetter={(i: number) => state.results[i]}
                        rowCount={Math.min(perPage, state.results.length)}
                    />

                    <PaginationComponent
                        qs={location.search}
                        className="mt-5"
                        baseTo="/search"
                        nPages={nPages}
                        page={state.page}
                    />
                </React.Fragment>
            )}
        </React.Fragment>
    );
};
