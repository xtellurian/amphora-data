import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../../../redux/state';
import { Table } from '../Table';
import { DetailedAmphora } from 'amphoradata';
import { RouteComponentProps } from 'react-router';
import { StringToEntityMap } from '../../../../redux/state/common';
import { AccessType, Scope } from '../../../../redux/actions/amphora/list';
import { EmptyState } from '../../empty/EmptyState';

interface ConnectedAmphoraTableProps {
    accessType: AccessType;
    scope: Scope;
    collections: {
        self?: {
            created?: string[];
            purchased?: string[];
        };
        organisation?: {
            created?: string[];
            purchased?: string[];
        };
    };
    cache: StringToEntityMap<DetailedAmphora>;
}

const columns = [
    { key: 'name', name: 'Amphora Name' },
    // { key: 'createdDate', name: 'Date Created' },
    { key: 'price', name: 'Price per Month' },
];

class ConnectedAmphoraTable extends React.PureComponent<ConnectedAmphoraTableProps & RouteComponentProps> {

    getIdsList(): string[] {
        if (this.props.scope === "self") {
            if (this.props.accessType === "created") {
                return this.props.collections.self ? this.props.collections.self.created || [] : [];
            }
            else if (this.props.accessType === "purchased") {
                return this.props.collections.self ? this.props.collections.self.purchased || [] : [];
            } else {
                return [];
            }
        } else if (this.props.scope === "organisation") {
            if (this.props.accessType === "created") {
                return this.props.collections.organisation ? this.props.collections.organisation.created || [] : [];
            }
            else if (this.props.accessType === "purchased") {
                return this.props.collections.organisation ? this.props.collections.organisation.purchased || [] : [];
            } else {
                return [];
            }
        } else {
            return [];
        }
    }

    render() {
        const ids = this.getIdsList();
        if (ids && ids.length > 0) {
            return (<div>
                <Table
                    onRowClicked={(r) => this.props.history.push(`amphora/detail/${r.id}`)}
                    columns={columns}
                    rowGetter={(i: number) => this.props.cache[ids[i]]}
                    rowCount={Math.min(10, ids.length)} />
            </div>)
        } else {
            return <EmptyState >
                There are no Amphora here yet.
            </EmptyState>
        }
    }
}

function mapStateToProps(state: ApplicationState) {
    if (state && state.amphora && state.amphora.collections) {
        return {
            collections: state.amphora.collections,
            cache: state.amphora.cache
        };
    } else {
        return {
            collections: {
                self: { created: [], purchased: [] },
                organisation: { created: [], purchased: [] }
            },
            cache: state.amphora.cache
        };
    }
}

export default connect(mapStateToProps)(ConnectedAmphoraTable);