import * as React from "react";
import { Table } from "../molecules/tables/Table";
import { RouteComponentProps } from "react-router";
import { EmptyState } from "../molecules/empty/EmptyState";
import { MyAmphoraContext } from "react-amphora";

const columns = [
    { key: "name", name: "Amphora Name" },
    // { key: 'createdDate', name: 'Date Created' },
    { key: "price", name: "Price per Month" },
];

class ConnectedAmphoraTable extends React.PureComponent<
    MyAmphoraContext.MyAmphoraState & RouteComponentProps
> {
    render() {
        if (this.props.results && this.props.results.length > 0) {
            return (
                <div>
                    <Table
                        onRowClicked={(r) =>
                            this.props.history.push(`amphora/detail/${r.id}`)
                        }
                        columns={columns}
                        rowGetter={(i: number) => this.props.results[i]}
                        rowCount={Math.min(10, this.props.results.length)}
                    />
                </div>
            );
        } else {
            return <EmptyState>There are no Amphora here yet.</EmptyState>;
        }
    }
}

export default MyAmphoraContext.withMyAmphoraState(ConnectedAmphoraTable);
