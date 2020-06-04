import * as React from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../../redux/state";
import { Table } from "../molecules/tables/Table";
import { BasicAmphora } from "amphoradata";
import { RouteComponentProps } from "react-router";
import { EmptyState } from "../molecules/empty/EmptyState";

interface ConnectedSearchTableProps {
    results: BasicAmphora[];
}

const columns = [
    { key: "name", name: "Amphora Name" },
    // { key: 'createdDate', name: 'Date Created' },
    { key: "price", name: "Price per Month" },
];
class ConnectedSearchTable extends React.PureComponent<
    ConnectedSearchTableProps & RouteComponentProps
> {
    render() {
        if (this.props.results && this.props.results.length > 0) {
            return (
                <div>
                    <Table
                        onRowClicked={(r) =>
                            this.props.history.push(`/search/detail/${r.id}`)
                        }
                        columns={columns}
                        rowGetter={(i: number) => this.props.results[i]}
                        rowCount={Math.min(12, this.props.results.length)}
                    />
                </div>
            );
        } else {
            return (
                <EmptyState>
                    There are no Amphora found that match your search.
                </EmptyState>
            );
        }
    }
}

function mapStateToProps(state: ApplicationState): ConnectedSearchTableProps {
    if (state && state.search && state.search.results) {
        return {
            results: state.search.results,
        };
    } else {
        return {
            results: [],
        };
    }
}

export default connect(mapStateToProps)(ConnectedSearchTable);
