import * as React from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../../../../redux/state";
import { Table } from "../Table";
import { TermsOfUse } from "amphoradata";
import { RouteComponentProps } from "react-router";
import { Cache, emptyCache } from "../../../../redux/state/common";
import { EmptyState } from "../../empty/EmptyState";

interface ConnectedTermsTableProps {
  termIds: string[];
  cache: Cache<TermsOfUse>;
}

const columns = [
  { key: "name", name: "Terms of Use Name" },
];

class ConnectedTermsTable extends React.PureComponent<
  ConnectedTermsTableProps & RouteComponentProps
> {
  render() {
    const ids = this.props.termIds;
    if (ids && ids.length > 0) {
      return (
        <div>
          <Table
            onRowClicked={(r) =>
              this.props.history.push(`/terms/detail/${r.id}`)
            }
            columns={columns}
            rowGetter={(i: number) => this.props.cache.store[ids[i]]}
            rowCount={Math.min(10, ids.length)}
          />
        </div>
      );
    } else {
      return <EmptyState>There are no Terms here yet.</EmptyState>;
    }
  }
}

function mapStateToProps(state: ApplicationState): ConnectedTermsTableProps {
  if (state && state.terms && state.terms) {
    return {
      cache: state.terms.cache,
      termIds: state.terms.termIds,
    };
  } else {
    return {
      termIds: [],
      cache: emptyCache<TermsOfUse>(),
    };
  }
}

export default connect(mapStateToProps)(ConnectedTermsTable);
