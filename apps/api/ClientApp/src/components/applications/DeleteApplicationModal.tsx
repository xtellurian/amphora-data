import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { Application } from "amphoradata";
import { ModalWrapper, ModalContents, ModalHeading } from "../molecules/modal";
import { success, info, error, warning } from "../molecules/toasts";
import { parseServerError } from "../../utilities";
import { ModalFooter, Spinner, Row, Col } from "reactstrap";
import { PrimaryButton, SecondaryButton } from "../molecules/buttons";
import { useLocation } from "react-router";
import { LoadingState } from "../molecules/empty";
import { Link } from "react-router-dom";

interface DeleteApplicationModalState {
    loading?: boolean;
    error?: any;
    applicationId?: string | null | undefined;
    application: Application;
}
export const DeleteApplicationModal: React.FC = () => {
    const clients = useAmphoraClients();
    const location = useLocation();
    const searchParams = new URLSearchParams(location.search);

    const [isOpen, setIsOpen] = React.useState<boolean>(true);
    const [state, setState] = React.useState<DeleteApplicationModalState>({
        applicationId: searchParams.get("id"),
        loading: true,
        application: { locations: [{}] },
    });

    const deleteApp = () => {
        if (state.application && state.application.id) {
            clients.applicationsApi
                .applicationsDeleteApplication(state.application.id)
                .then((r) => {
                    success({
                        text: `Application ${state.application.name} was deleted`,
                    });
                    setIsOpen(false);
                })
                .catch((e) => {
                    error({
                        text: parseServerError(
                            e,
                            "Application was not deleted."
                        ),
                    });
                });
        } else {
            warning({ text: "Unable to delete application. Id was unknown." });
        }
    };

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

    const MainContents: React.FC = () => {
        return (
            <React.Fragment>
                <ModalHeading>
                    Delete an Application
                    {state.loading && <Spinner />}
                </ModalHeading>
                <Row>
                    <Col>
                        <h5>
                            Are you sure you want to delete this Application?
                        </h5>
                    </Col>
                </Row>

                <div className="card w-50 p-5 m-auto">
                    <div>Name: {state.application.name}</div>
                    <div>
                        Origins:{" "}
                        {state.application.locations?.map(
                            (l) => `${l.origin},`
                        )}
                    </div>
                </div>
                <ModalFooter>
                    <PrimaryButton onClick={() => deleteApp()}>
                        Delete
                    </PrimaryButton>
                    <Link to="/applications">
                        <SecondaryButton>Cancel</SecondaryButton>
                    </Link>
                </ModalFooter>
            </React.Fragment>
        );
    };

    return (
        <React.Fragment>
            <ModalWrapper onCloseRedirectTo="/applications" isOpen={isOpen}>
                <ModalContents>
                    {state.loading ? <LoadingState /> : <MainContents />}
                </ModalContents>
            </ModalWrapper>
        </React.Fragment>
    );
};
