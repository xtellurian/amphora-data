import * as React from "react";
import { connect } from "react-redux";
import { useAmphoraClients } from "react-amphora";
import { actionCreators } from "../../../redux/actions/amphora/files";
import { AmphoraDetailProps, mapStateToProps, OneAmphora } from "./props";
import { EmptyState } from "../../molecules/empty/EmptyState";
import { amphoraApiClient } from "../../../clients/amphoraApiClient";
import { PrimaryButton } from "../../molecules/buttons";
import * as toast from "../../molecules/toasts";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";
import { UploadResponse, DetailedAmphora } from "amphoradata";
import { AxiosInstance } from "axios";

interface FilesState {
    loaded: boolean;
    fileNames: string[];
    newFileName?: string;
}

// backup upload url
const backupUploadUrl = (id: string, name: string) =>
    `/api/amphorae/${id}/files/${name}`;
// helper
function completeUpload(
    amphora: DetailedAmphora,
    axios: AxiosInstance,
    name: string,
    file: File,
    res?: UploadResponse | null | undefined
) {
    const data = new FormData();

    data.append("name", name);
    data.append("content", file);

    console.log("starting upload");
    if (res && res.url) {
        axios
            .put(res.url, data)
            .then((k) => console.log("uploaded file"))
            .catch((e) => console.log("ERROR Uploading file"));
    } else if (amphora.id) {
        // try backup
        axios
            .put(backupUploadUrl(amphora.id, name), data)
            .then((k) => console.log("uploaded file backup"))
            .catch((e) => console.log("uploaded file backup error"));
    } else {
        console.log("uh oh");
    }
}

type AmphoraDetailFilesProps = AmphoraDetailProps & typeof actionCreators;

const hiddenInputId = "select-file-input";

export const FilesPage: React.FunctionComponent<OneAmphora> = (props) => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<FilesState>({
        fileNames: [],
        loaded: false,
    });
    React.useEffect(() => {
        if (props.amphora.id && !state.loaded) {
            clients.amphoraeApi
                .amphoraeFilesListFiles(props.amphora.id)
                .then((r) => setState({ fileNames: r.data, loaded: true }))
                .catch((e) => setState({ fileNames: [], loaded: true }));
        }
    });

    const triggerUpload = () => {
        const x = document.getElementById(hiddenInputId);
        if (x) {
            x.click();
        } else {
            console.log("no xxx");
        }
    };

    const onFileChangedHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
        console.log("running handler");
        const file = e.target.files && e.target.files[0];
        const localFileName = file && file.name;
        const fileName = state.newFileName || localFileName;
        console.log(clients.axios)
        if (
            props.amphora.id &&
            file &&
            fileName &&
            e.target.files &&
            e.target.files.length > 0
        ) {
            clients.amphoraeApi
                .amphoraeFilesCreateFileRequest(
                    props.amphora.id,
                    fileName
                )
                .then((r) =>
                    completeUpload(
                        props.amphora,
                        clients.axios,
                        fileName,
                        file,
                        r.data
                    )
                )
                .catch((e) => console.log(e));
        }
    };

    return (
        <React.Fragment>
            <Header title="Files">
                <input
                    id={hiddenInputId}
                    hidden
                    type="file"
                    name="file"
                    onChange={(e) => onFileChangedHandler(e)}
                />
                <PrimaryButton onClick={(e) => triggerUpload()}>
                    {" "}
                    Upload File{" "}
                </PrimaryButton>
            </Header>
            {state.fileNames.map((f) => (
                <p key={f}> {f}</p>
            ))}
        </React.Fragment>
    );
};

// class FilesClassy extends React.PureComponent<
//     AmphoraDetailFilesProps,
//     FilesState
// > {
//     /**
//      *
//      */
//     constructor(props: AmphoraDetailFilesProps) {
//         super(props);
//         this.state = { isLoading: true, files: [] };
//     }
//     public componentDidMount() {
//         this.loadFiles();
//     }

//     private loadFiles() {
//         amphoraApiClient
//             .amphoraeFilesListFiles(this.props.match.params.id)
//             .then((f) => this.setState({ isLoading: false, files: f.data }))
//             .catch((e) => this.handleFileLoadError(e));
//     }

//     private handleFileLoadError(e: any) {
//         this.setState({ isLoading: false, files: [] });
//         toast.error({ text: "Error getting files" });
//     }

//     renderFileList() {
//         if (this.state.isLoading) {
//             return <LoadingState />;
//         } else if (this.state.files && this.state.files.length > 0) {
//             // there are files
//             return this.state.files.map((f) => <p key={f}> {f}</p>);
//         } else {
//             return <EmptyState>There are no files.</EmptyState>;
//         }
//     }

//     private onFileChangedHandler(e: React.ChangeEvent<HTMLInputElement>) {
//         const id = this.props.match.params.id;
//         const amphora = this.props.amphora.metadata.store[id];
//         if (amphora && e.target.files && e.target.files.length > 0) {
//             const name = this.state.newFileName || e.target.files[0].name;
//             this.props.uploadFile(amphora, e.target.files[0], name);
//             setTimeout(() => this.loadFiles(), 2000); //TODO: Improve this
//         }
//     }

//     private triggerUpload() {
//         const x = document.getElementById(hiddenInputId);
//         if (x) {
//             x.click();
//         }
//     }

//     public render() {
//         const id = this.props.match.params.id;
//         const amphora = this.props.amphora.metadata.store[id];
//         if (amphora) {
//             return (
//                 <React.Fragment>
//                     <Header title="Files">
//                         <input
//                             id={hiddenInputId}
//                             hidden
//                             type="file"
//                             name="file"
//                             onChange={(e) => this.onFileChangedHandler(e)}
//                         />
//                         <PrimaryButton onClick={(e) => this.triggerUpload()}>
//                             {" "}
//                             Upload File{" "}
//                         </PrimaryButton>
//                     </Header>
//                     {this.renderFileList()}
//                 </React.Fragment>
//             );
//         } else {
//             return <LoadingState />;
//         }
//     }
// }

// export default connect(mapStateToProps, actionCreators)(FilesClassy as any);
