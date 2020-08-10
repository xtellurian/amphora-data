import * as React from "react";
import { AxiosInstance } from "axios";
import { useAmphoraClients } from "react-amphora";
import { OneAmphora } from "./props";
import { EmptyState } from "../../molecules/empty/EmptyState";
import * as axios from "axios";
import { PrimaryButton } from "../../molecules/buttons";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Table } from "../../molecules/tables/Table";
import { Header } from "./Header";
import { UploadResponse, DetailedAmphora } from "amphoradata";
import Modal, { Styles } from "react-modal";
import { Link } from "react-router-dom";

interface FilesState {
    isLoading: boolean;
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

    console.log("starting upload");
    if (res && res.url) {
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
        fileNames: [],
        isLoading: false,
        downloadFileName: null,
    });

    const cancelToken = axios.default.CancelToken;
    const source = cancelToken.source();

    const tryLoadFiles = () => {
        if (props.amphora.id && !state.isLoading) {
            setState({ isLoading: true, fileNames: state.fileNames });
            clients.amphoraeApi
                .amphoraeFilesListFiles(
                    props.amphora.id,
                    "Alphabetical",
                    '',
                    25,
                    0,
                    {
                        cancelToken: source.token,
                    }
                )
                .then((r) => setState({ fileNames: r.data, isLoading: false }))
                .catch((e) => setState({ fileNames: [], isLoading: false }));
        }
    };

    React.useEffect(() => {
        tryLoadFiles();
        return () => source.cancel("The files component unmounted");
    }, []);

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
            clients.amphoraeApi
                .amphoraeFilesCreateFileRequest(props.amphora.id, fileName)
                .then((r) =>
                    completeUpload(
                        props.amphora,
                        clients.axios,
                        fileName,
                        file,
                        r.data,
                        (e) => !e && tryLoadFiles()
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
                        <PrimaryButton className="w-75" onClick={() => setTimeout(closeModal, 250)}>
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
            {state.fileNames.length == 0 ? (
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
