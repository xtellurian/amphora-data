import * as React from 'react';
import { connect } from 'react-redux';
import { Spinner } from 'reactstrap';
import { AmphoraDetailProps, mapStateToProps } from './props';
import { EmptyState } from '../../molecules/empty/EmptyState';
import { amphoraApiClient } from '../../../clients/amphoraApiClient';

interface FilesState {
    files: string[];
}

class Files extends React.PureComponent<AmphoraDetailProps, FilesState> {

    /**
     *
     */
    constructor(props: AmphoraDetailProps) {
        super(props);
        this.state = { files: [] };
    }
    public componentDidMount() {
        amphoraApiClient.amphoraeFilesListFiles(this.props.match.params.id)
            .then(f => this.setState({ files: f.data }))
            .catch(e => console.log(e));
    }

    renderFileList() {
        if (this.state.files && this.state.files.length) {
            // there are files
            return <p> THERE ARE FILES HERE TO SEE</p>
        } else {
            return (
            <EmptyState>
                <strong>You haven't uploaded a file</strong>
            </EmptyState>)
        }
    }

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.cache[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <h5>Files</h5>
                    <hr/>
                    {this.renderFileList()}
                </React.Fragment>

            )
        } else {
            return <Spinner></Spinner>
        }

    }
}

export default connect(
    mapStateToProps,
    null,
)(Files);