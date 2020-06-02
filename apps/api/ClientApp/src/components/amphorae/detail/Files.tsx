import * as React from 'react';
import { connect } from 'react-redux';
import { actionCreators } from '../../../redux/actions/amphora/files';
import { AmphoraDetailProps, mapStateToProps } from './props';
import { EmptyState } from '../../molecules/empty/EmptyState';
import { amphoraApiClient } from '../../../clients/amphoraApiClient';
import { PrimaryButton } from '../../molecules/buttons';
import * as toast from '../../molecules/toasts';
import { LoadingState } from '../../molecules/empty/LoadingState';

interface FilesState {
    isLoading: boolean;
    files: string[];
    newFileName?: string;
}

type AmphoraDetailFilesProps =
    AmphoraDetailProps
    & typeof actionCreators;

const hiddenInputId = "select-file-input";

class Files extends React.PureComponent<AmphoraDetailFilesProps, FilesState> {

    /**
     *
     */
    constructor(props: AmphoraDetailFilesProps) {
        super(props);
        this.state = { isLoading: true, files: [] };
    }
    public componentDidMount() {
        this.loadFiles();
    }

    private loadFiles() {
        amphoraApiClient.amphoraeFilesListFiles(this.props.match.params.id)
            .then((f) => this.setState({ isLoading: false, files: f.data }))
            .catch((e) => this.handleFileLoadError(e));
    }

    private handleFileLoadError(e: any) {
        this.setState({ isLoading: false, files: [] });
        toast.error({ text: "Error getting files" })
    }

    renderFileList() {
        if (this.state.isLoading) {
            return <LoadingState />
        }
        else if (this.state.files && this.state.files.length > 0) {
            // there are files
            return this.state.files.map(f => <p key={f}> {f}</p>)
        } else {
            return (
                <EmptyState>
                    <strong>There are no files.</strong>
                </EmptyState>)
        }
    }

    private onFileChangedHandler(e: React.ChangeEvent<HTMLInputElement>) {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora && e.target.files && e.target.files.length > 0) {
            const name = this.state.newFileName || e.target.files[0].name;
            this.props.uploadFile(amphora, e.target.files[0], name);
            setTimeout(() => this.loadFiles(), 2000); //TODO: Improve this
        }
    }

    private triggerUpload() {
        const x = document.getElementById(hiddenInputId);
        if (x) {
            x.click()
        }
    }

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
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
            return <LoadingState />
        }

    }
}

export default connect(
    mapStateToProps,
    actionCreators,
)(Files as any);