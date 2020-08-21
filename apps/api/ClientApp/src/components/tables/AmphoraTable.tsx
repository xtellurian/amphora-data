import * as React from "react";
import { Table } from "../molecules/tables/Table";
import { EmptyState } from "../molecules/empty/EmptyState";
import { DetailedAmphora } from "amphoradata";
import { useHistory } from "react-router-dom";

const columns = [
    { key: "name", name: "Amphora Name" },
    // { key: 'createdDate', name: 'Date Created' },
    { key: "price", name: "Price per Month" },
];

interface AmphoraTableProps {
    amphoras: DetailedAmphora[];
}

export const AmphoraTable: React.FC<AmphoraTableProps> = ({ amphoras }) => {
    const history = useHistory();

    if (amphoras && amphoras.length > 0) {
        return (
            <div>
                <Table
                    onRowClicked={(r) => history.push(`amphora/detail/${r.id}`)}
                    columns={columns}
                    rowGetter={(i: number) => amphoras[i]}
                    rowCount={Math.min(10, amphoras.length)}
                />
            </div>
        );
    } else {
        return <EmptyState>There are no Amphora here yet.</EmptyState>;
    }
};
