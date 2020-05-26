import * as React from 'react';
import { connect } from 'react-redux';
import { Spinner } from 'reactstrap';
import { AmphoraDetailProps, mapStateToProps } from './props';
import { EmptyState } from '../../molecules/empty/EmptyState';
import { amphoraApiClient } from '../../../clients/amphoraApiClient';
import { PrimaryButton } from '../../molecules/buttons';

interface FilesState {
    files: string[];
}

const hiddenInputId = "select-file-input";

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
                    <strong>There are no files.</strong>
                </EmptyState>)
        }
    }

    private onFileChangedHandler(e: React.ChangeEvent<HTMLInputElement>) {
        console.log(e.target.files)
    }

    private triggerUpload() {
        const x = document.getElementById(hiddenInputId);
        if (x) {
            x.click()
        }
    }

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.cache[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <div className="row">
                        <div className="col-6">
                            <h5>Files</h5>
                        </div>
                        <div className="col-6 pr-5 text-right">

                            <input id={hiddenInputId} hidden type="file" name="file" onChange={(e) => this.onFileChangedHandler(e)} />
                            <PrimaryButton onClick={e => this.triggerUpload()}> Upload File </PrimaryButton>
                        </div>
                    </div>
                    <hr />
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