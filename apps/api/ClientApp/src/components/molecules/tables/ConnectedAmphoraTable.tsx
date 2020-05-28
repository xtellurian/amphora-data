import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/state';
import { Table } from './Table';
import { DetailedAmphora } from 'amphoradata';

interface ConnectedAmphoraTableProps {
    list: DetailedAmphora[];
}

const columns = [
    { key: 'name', name: 'Amphora Name' },
    // { key: 'createdDate', name: 'Date Created' },
    { key: 'price', name: 'Price per Month' },
];

const rows = [
    { id: 0, title: 'Example' },
    { id: 1, title: 'Demo' }
];

class ConnectedAmphoraTable extends React.PureComponent<ConnectedAmphoraTableProps> {

    render() {

        return (<div>
            <Table columns={columns} rowGetter={(i: number) => this.props.list[i]} rowCount={Math.min(10, this.props.list.length)}/>
        </div>)
    }
}

function mapStateToProps(state: ApplicationState) {
    if (state && state.amphora && state.amphora.list) {
        return {
            list: state.amphora.list
        };
    } else {
        return { list: [] };
    }
}

export default connect(
    mapStateToProps
)(ConnectedAmphoraTable);