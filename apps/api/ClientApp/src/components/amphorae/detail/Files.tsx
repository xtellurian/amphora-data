import * as React from "react";
import { AxiosInstance } from "axios";
import Modal, { Styles } from "react-modal";
import { Link } from "react-router-dom";
import * as axios from "axios";
import { useAmphoraClients } from "react-amphora";
import { OneAmphora } from "./props";
import { EmptyState } from "../../molecules/empty/EmptyState";
import { success, error } from "../../molecules/toasts";
import { PrimaryButton } from "../../molecules/buttons";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Table } from "../../molecules/tables/Table";
import { Header } from "./Header";
import { UploadResponse, DetailedAmphora } from "amphoradata";

interface FilesState {
    isLoading: boolean;
    requiresReload?: boolean;
    downloadFileName?: string | null;
    fileNames: string[];
    newFileName?: string;
}
const modalStyle: Styles = {
    overlay: {
        zIndex: 3000, // this should be on top of the hamburger menu.
    },
    content: {
        minWidth: "20rem",
        top: "50%",
        left: "50%",
        right: "auto",
        bottom: "auto",
        marginRight: "-50%",
        transform: "translate(-50%, -50%)",
    },
};
const columns = [{ key: "name", name: "File Name" }];

// backup upload url
const backupUploadUrl = (id: string, name: string) =>
    `/api/amphorae/${id}/files/${name}`;
// helper
function completeUpload(
    amphora: DetailedAmphora,
    axios: AxiosInstance,
    name: string,
    file: File,
    res?: UploadResponse | null | undefined,
    callback?: (error?: any | null | undefined) => void
) {
    const data = new FormData();

    data.append("name", name);
    data.append("content", file);
    const useBlobEndpoint = false;
    console.log(`starting upload, useElobEndpoint: ${useBlobEndpoint}`);
    if (res && res.url && useBlobEndpoint) {
        axios
            .put(res.url, data)
            .then((k) => callback && callback())
            .catch((e) => callback && callback(e));
    } else if (amphora.id) {
        // try backup
        axios
            .put(backupUploadUrl(amphora.id, name), data)
            .then((k) => callback && callback())
            .catch((e) => callback && callback(e));
    } else {
        callback && callback(true);
    }
}

const hiddenInputId = "select-file-input";

export const FilesPage: React.FunctionComponent<OneAmphora> = (props) => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<FilesState>({
        requiresReload: false,
        fileNames: [],
        isLoading: false,
        downloadFileName: null,
    });

    const cancelToken = axios.default.CancelToken;
    const source = cancelToken.source();

    React.useEffect(() => {
        console.log("using files effect");
        if (props.amphora && props.amphora.id) {
            const amphoraId = props.amphora.id;
            if (amphoraId && !state.isLoading) {
                setState({ ...state, isLoading: true });
                clients.amphoraeApi
                    .amphoraeFilesListFiles(
                        amphoraId,
                        "Alphabetical",
                        "",
                        25,
                        0,
                        {
                            cancelToken: source.token,
                        }
                    )
                    .then((r) =>
                        setState({ fileNames: r.data, isLoading: false })
                    )
                    .catch((e) => {
                        console.log(e);
                        setState({ fileNames: [], isLoading: false });
                    });
            }
            return () => source.cancel("The files component unmounted");
        }
    }, [props.amphora, state.requiresReload]);

    const triggerUpload = () => {
        const x = document.getElementById(hiddenInputId);
        if (x) {
            x.click();
        }
    };

    const onFileChangedHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files && e.target.files[0];
        const localFileName = file && file.name;
        const fileName = state.newFileName || localFileName;
        if (
            props.amphora.id &&
            file &&
            fileName &&
            e.target.files &&
            e.target.files.length > 0
        ) {
            const amphoraId = props.amphora.id;
            clients.amphoraeApi
                .amphoraeFilesCreateFileRequest(amphoraId, fileName)
                .then((r) =>
                    completeUpload(
                        props.amphora,
                        clients.axios,
                        fileName,
                        file,
                        r.data,
                        (e) => {
                            if (!e) {
                                success(
                                    { text: "File Uploaded" },
                                    { autoClose: 1000 }
                                );
                                setState({ ...state, requiresReload: true });
                            } else {
                                error(
                                    { text: "Error Uploading File" },
                                    { autoClose: 2000 }
                                );
                            }
                        }
                    )
                )
                .catch((e) => console.log(e));
        }
    };

    if (state.isLoading) {
        return <LoadingState />;
    }

    const downloadFile = (fileName: string) => {
        setState({
            isLoading: state.isLoading,
            fileNames: state.fileNames,
            newFileName: state.newFileName,
            downloadFileName: fileName,
        });
    };

    const closeModal = () => {
        setState({
            fileNames: state.fileNames,
            isLoading: state.isLoading,
            newFileName: state.newFileName,
            downloadFileName: null,
        });
    };
    const downloadPath = `/api/amphorae/${props.amphora.id}/files/${state.downloadFileName}`;
    console.log(state);
    return (
        <React.Fragment>
            <Modal
                isOpen={!!state.downloadFileName}
                shouldCloseOnEsc={true}
                shouldCloseOnOverlayClick={true}
                // onAfterOpen={onAfterOpen}
                onRequestClose={(e) => closeModal()}
                style={modalStyle}
                contentLabel="Example Modal"
            >
                <div className="text-center">
                    <Link target="_blank" to={downloadPath}>
                        <PrimaryButton
                            className="w-75"
                            onClick={() => setTimeout(closeModal, 250)}
                        >
                            {`Download ${state.downloadFileName}`}
                        </PrimaryButton>
                    </Link>
                </div>
            </Modal>

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
            {state.fileNames.length === 0 ? (
                <EmptyState> There are no files in this Amphora.</EmptyState>
            ) : (
                <Table
                    columns={columns}
                    onRowClicked={(r) => downloadFile(r.name as string)}
                    rowGetter={(i: number) => {
                        return { name: state.fileNames[i] };
                    }}
                    rowCount={Math.min(25, state.fileNames.length)}
                />
            )}
        </React.Fragment>
    );
};
