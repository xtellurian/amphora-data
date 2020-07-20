import * as React from "react";
import Joyride, { CallBackProps } from "react-joyride";
import Modal from "react-modal";
import * as toast from "../molecules/toasts";

const customModalStyles: Modal.Styles = {
    overlay: {
        zIndex: 1500, // this should be on top of the hamburger menu.
        border: "none",
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

const withTour = (WrappedComponent: any) => {
    const tourItem = localStorage.getItem("tour");
    const Tour: React.FunctionComponent = (props) => {
        const initialState = {
            modalOpen: tourItem !== "dismiss",
            doTour: typeof tourItem !== "undefined" && tourItem !== "dismiss",
            steps: [
                {
                    target: "#howdy",
                    content: "This is my awesome feature!",
                },
                {
                    target: "#main-search-button",
                    content: "This another awesome feature!",
                },
            ],
        };
        const [state, setState] = React.useState(initialState);

        const dismissTour = () => {
            setState({ ...state, modalOpen: false, doTour: false });
            localStorage.setItem("tour", "dismiss");
        };

        const acceptTour = () => {
            setState({ ...state, modalOpen: false, doTour: true });
            localStorage.setItem("tour", "accept");
        };

        if (!tourItem) {
            // then we never run this before
            return (
                <React.Fragment>
                    <Modal
                        isOpen={state.modalOpen}
                        shouldCloseOnEsc={true}
                        shouldCloseOnOverlayClick={true}
                        onRequestClose={(e) => dismissTour()}
                        style={customModalStyles}
                        contentLabel="Tour Request Modal"
                    >
                        <div className="jumbotron">
                            <h1 className="display-4">Hello, and Welcome!</h1>
                            <p className="lead">
                                If this is your first time, we reccommend taking
                                a quick tour.
                            </p>

                            <div className="lead btn-group">
                                <a
                                    className="btn btn-primary btn-lg"
                                    href="#"
                                    role="button"
                                    onClick={() => acceptTour()}
                                >
                                    Take the Tour
                                </a>
                                <a
                                    className="btn btn-secondary btn-lg"
                                    href="#"
                                    role="button"
                                    onClick={() => dismissTour()}
                                >
                                    No thanks, maybe later
                                </a>
                            </div>
                        </div>
                    </Modal>
                </React.Fragment>
            );
        }
        const tourCallback = (c: CallBackProps) => {
            if (c.action === "reset") {
                // tour is done
                localStorage.setItem("tour", "dismiss");
                setState({ ...state, doTour: false });
            }
        };

        if (state.doTour) {
            toast.success({text: "Starting a tour."})
            return (
                <React.Fragment>
                    <Joyride
                        run={true}
                        debug={true}
                        steps={state.steps}
                        callback={(c) => tourCallback(c)}
                        styles={{
                            options: {
                                arrowColor: "#e3ffeb",
                                backgroundColor: "#e3ffeb",
                                overlayColor: "rgba(79, 26, 0, 0.4)",
                                primaryColor: "var(--amphora-red)",
                                textColor: "#004a14",
                                width: 900,
                                zIndex: 5000,
                            },
                        }}
                    />
                    <WrappedComponent {...props} />
                </React.Fragment>
            );
        } else {
            return <WrappedComponent {...props} />;
        }
    };

    return Tour;
};

export default withTour;
