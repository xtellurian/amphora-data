import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { CreateApplication, AppLocation } from "amphoradata";
import { ModalWrapper, ModalContents, ModalHeading } from "../molecules/modal";
import { success, info, error } from "../molecules/toasts";
import { parseServerError } from "../../utilities";
import { TextInput } from "../molecules/inputs";
import { EditApplicationLocationSection } from "./ApplicationLocationSection";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { ModalFooter, Spinner } from "reactstrap";
import { PrimaryButton } from "../molecules/buttons";
import { useLocation } from "react-router";
import { LoadingState } from "../molecules/empty";

interface CreateApplicationModalState {
    loading?: boolean;
    error?: any;
    applicationId?: string | null | undefined;
    application: CreateApplication;
}
export const CreateOrUpdateApplicationModal: React.FC = () => {
    const clients = useAmphoraClients();
    const location = useLocation();
    const searchParams = new URLSearchParams(location.search);

    const [isOpen, setIsOpen] = React.useState<boolean>(true);
    const [state, setState] = React.useState<CreateApplicationModalState>({
        applicationId: searchParams.get("id"),
        loading: true,
        application: { locations: [{}] },
    });

    React.useEffect(() => {
        if (state.applicationId) {
            // set the state with the application information
            clients.applicationsApi
                .applicationsReadApplication(state.applicationId)
                .then((r) => {
                    info({ text: "Loaded Application" }, { autoClose: 1000 });
                    setState({
                        loading: false,
                        application: r.data,
                        applicationId: r.data.id,
                    });
                })
                .catch((e) => {
                    error({
                        text: parseServerError(e, "Failed to load application"),
                    });
                    // close the modal in this case
                    setIsOpen(false);
                });
        } else {
            setState({
                ...state,
                loading: false,
            });
        }
    }, []);

    const save = () => {
        setState({
            ...state,
            loading: true,
        });

        if (state.applicationId) {
            console.log(`Updating application ${state.applicationId}`);
            clients.applicationsApi
                .applicationsUpdateApplication(state.applicationId, {
                    ...state.application,
                })
                .then((r) => {
                    success(
                        { text: "Application Updated" },
                        { autoClose: 1000 }
                    );
                    setIsOpen(false);
                })
                .catch((e) => {
                    const text = parseServerError(
                        e,
                        "The server could not update the application"
                    );
                    error({ text }, { autoClose: 1000 });
                    setState({
                        loading: false,
                        application: state.application,
                        error: e,
                    });
                });
        } else {
            console.log("Creating application");
            clients.applicationsApi
                .applicationsCreateApplication(state.application)
                .then((r) => {
                    success(
                        { text: "Application Created" },
                        { autoClose: 2000 }
                    );
                    setIsOpen(false);
                })
                .catch((e) => {
                    const text = parseServerError(
                        e,
                        "The server did not create the application"
                    );
                    error({ text }, { autoClose: 1000 });
                    setState({
                        loading: false,
                        application: state.application,
                        error: e,
                    });
                });
        }
    };

    const setName = (name?: string) => {
        if (name) {
            const application = state.application;
            application.name = name;
            setState({ ...state, application });
        }
    };

    const setLogoutUrl = (logoutUrl?: string) => {
        const application = state.application;
        application.logoutUrl = logoutUrl;
        setState({ ...state, application });
    };
    const addNewLocation = () => {
        const application = state.application;
        if (application.locations) {
            application.locations = [...application.locations, {}];
            setState({ ...state, application });
        }
    };

    const updateLocation = (location: AppLocation, index: number) => {
        const application = state.application;
        if (application.locations) {
            application.locations[index] = location;
        }
        setState({ ...state, application });
    };
    const removeLocation = (location: AppLocation, index: number) => {
        const application = state.application;
        if (application.locations) {
            application.locations.splice(index, 1);
        }
        setState({ ...state, application });
    };

    const MainContents: React.FC = () => {
        return (
            <React.Fragment>
                <ModalHeading>
                    Create a new Application
                    {state.loading && <Spinner />}
                </ModalHeading>
                <TextInput
                    label="Application Name"
                    value={state.application.name || undefined}
                    onComplete={setName}
                    validator={(v) =>
                        !v ? { isValid: false } : { isValid: true }
                    }
                />
                <TextInput
                    label="Logout URL"
                    value={state.application.logoutUrl || undefined}
                    onComplete={setLogoutUrl}
                />

                <hr />
                <h3>Locations</h3>
                {state.application.locations &&
                    state.application.locations.map((l, index) => (
                        <EditApplicationLocationSection
                            key={index}
                            location={l}
                            onUpdated={(k) => updateLocation(k, index)}
                            onRemoved={(k) => removeLocation(k, index)}
                        />
                    ))}
                <div
                    className="m-3 cursor-pointer"
                    onClick={() => addNewLocation()}
                >
                    <FontAwesomeIcon className="mr-2" icon="plus-square" />
                    Add Location
                </div>
            </React.Fragment>
        );
    };

    return (
        <React.Fragment>
            <ModalWrapper onCloseRedirectTo="/applications" isOpen={isOpen}>
                <ModalContents>
                    {state.loading ? <LoadingState /> : <MainContents />}
                </ModalContents>

                <ModalFooter>
                    <PrimaryButton onClick={() => save()}>Save</PrimaryButton>
                </ModalFooter>
            </ModalWrapper>
        </React.Fragment>
    );
};
