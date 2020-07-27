import * as React from "react";
import { SearchContext } from "react-amphora";
import { RouteComponentProps, Route } from "react-router";
import { TextInput } from "../molecules/inputs";
import { LoadingState } from "../molecules/empty/LoadingState";
import { ConnectedAmphoraModal } from "./ConnectedAmphoraModal";
import { Table } from "../molecules/tables/Table";

// At runtime, Redux will merge together...
type SearchComponentProps = SearchContext.SearchState &
    SearchContext.SearchDispatch &
    RouteComponentProps<{}>; // ... plus incoming routing parameters

const columns = [
    { key: "name", name: "Amphora Name" },
    // { key: 'createdDate', name: 'Date Created' },
    { key: "price", name: "Price per Month" },
];

class SearchComponent extends React.PureComponent<SearchComponentProps> {
    // This method is called when the route parameters change
    public componentDidMount() {
        const term = this.searchParams().get("term");
        // const term = find(this.props.location.search, "term");
        if (term) {
            this.doSearch(term);
        }
    }

    private searchParams(): URLSearchParams {
        return new URLSearchParams(this.props.location.search);
    }

    private doSearch(term?: string) {
        if (term) {
            this.props.dispatch({ type: "search:execute", payload: { term } });
        }
    }

    private renderLoader(): JSX.Element | undefined {
        if (this.props.isLoading) {
            return <LoadingState />;
        } else {
            return <React.Fragment />;
        }
    }

    private renderTable(): JSX.Element | undefined {
        if (this.props.results) {
            return (
                <Table
                    onRowClicked={(r) =>
                        this.props.history.push(`/search/detail/${r.id}`)
                    }
                    columns={columns}
                    rowGetter={(i: number) => this.props.results[i]}
                    rowCount={Math.min(12, this.props.results.length)}
                />
            );
        }
    }

    public render() {
        return (
            <React.Fragment>
                <h2>Search</h2>
                <TextInput
                    id="search-bar"
                    value={this.searchParams().get("term") || ""}
                    onComplete={(t) => this.doSearch(t)}
                    label=""
                    placeholder="Enter a term..."
                />
                {this.renderLoader()}
                {this.renderTable()}

                <Route
                    path="/search/detail/:id"
                    component={ConnectedAmphoraModal}
                />
            </React.Fragment>
        );
    }
}

export default SearchContext.withSearch(SearchComponent);
