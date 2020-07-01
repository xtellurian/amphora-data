import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { OneAmphora } from "./props";
import { EmptyState } from "../../molecules/empty/EmptyState";
import * as axios from "axios";
import { PrimaryButton } from "../../molecules/buttons";
import * as toast from "../../molecules/toasts";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";
import { UploadResponse, DetailedAmphora } from "amphoradata";
import { AxiosInstance } from "axios";

interface FilesState {
    isLoading: boolean;
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

const hiddenInputId = "select-file-input";

export const FilesPage: React.FunctionComponent<OneAmphora> = (props) => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<FilesState>({
        fileNames: [],
        isLoading: false,
    });

    const cancelToken = axios.default.CancelToken;
    const source = cancelToken.source();

    React.useEffect(() => {
        if (props.amphora.id && !state.isLoading) {
            setState({ isLoading: true, fileNames: state.fileNames });
            clients.amphoraeApi
                .amphoraeFilesListFiles(
                    props.amphora.id,
                    64,
                    0,
                    "Alphabetical",
                    undefined,
                    undefined,
                    {
                        cancelToken: source.token,
                    }
                )
                .then((r) => setState({ fileNames: r.data, isLoading: false }))
                .catch((e) => setState({ fileNames: [], isLoading: false }));

            return () => source.cancel("The files component unmounted");
        }
    }, []);

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
        console.log(clients.axios);
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
                        r.data
                    )
                )
                .catch((e) => console.log(e));
        }
    };

    if (state.isLoading) {
        return <LoadingState />;
    }

    if (state.fileNames.length === 0) {
        return <EmptyState> There are no files in this Amphora.</EmptyState>;
    }

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
