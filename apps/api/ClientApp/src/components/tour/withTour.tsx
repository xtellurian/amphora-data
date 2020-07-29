import * as React from "react";
import Joyride, {
    CallBackProps,
    ACTIONS,
    LIFECYCLE,
    Locale,
} from "react-joyride";
import Modal from "react-modal";
import * as toast from "../molecules/toasts";
import { steps, ExtendedStep } from "./tourSteps";
import { useHistory } from "react-router";

interface CustomCallbackProps extends CallBackProps {
    step: ExtendedStep;
}

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

const tourLocale: Locale = {
    close: "Next",
};

const withTour = (WrappedComponent: any) => {
    const tourItem = localStorage.getItem("tour");
    const Tour: React.FunctionComponent = (props) => {
        const history = useHistory();
        const initialState = {
            modalOpen: tourItem !== "dismiss",
            promptForTour: !tourItem,
            doTour: tourItem === "accept",
            steps,
            toasted: false,
        };
        const [state, setState] = React.useState(initialState);

        const dismissTour = () => {
            setState({
                steps,
                promptForTour: false,
                modalOpen: false,
                doTour: false,
                toasted: state.toasted,
            });
            localStorage.setItem("tour", "dismiss");
        };

        const acceptTour = () => {
            localStorage.setItem("tour", "accept");
            setState({
                steps,
                promptForTour: false,
                modalOpen: false,
                doTour: true,
                toasted: state.toasted,
            });
        };

        if (state.promptForTour) {
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
                                If this is your first time, we recommend taking
                                a quick tour.
                            </p>

                            <div className="lead btn-group">
                                <a
                                    href="#"
                                    className="btn btn-primary btn-lg text-white"
                                    role="button"
                                    onClick={() => acceptTour()}
                                >
                                    Take the Tour
                                </a>
                                <a
                                    className="btn btn-secondary btn-lg"
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
        const tourCallback = (c: CustomCallbackProps) => {
            console.log(c);
            if (
                c.step.navigateOnClick &&
                c.step.navigateOnClick.length > 0 &&
                c.action === ACTIONS.UPDATE &&
                c.lifecycle == LIFECYCLE.TOOLTIP
            ) {
                history.push(c.step.navigateOnClick);
            }
            if (c.step.finalStep && c.action === ACTIONS.CLOSE) {
                // tour is done
                localStorage.setItem("tour", "dismiss");
                setState({ ...state, doTour: false });
            }
        };

        if (state.doTour) {
            if (!state.toasted) {
                setState({
                    steps: state.steps,
                    doTour: state.doTour,
                    modalOpen: state.modalOpen,
                    promptForTour: state.promptForTour,
                    toasted: true,
                });
                toast.success(
                    { text: "Starting the Tour" },
                    { autoClose: 1000 }
                );
            }
            return (
                <React.Fragment>
                    <Joyride
                        locale={tourLocale}
                        run={true}
                        debug={true}
                        steps={state.steps}
                        callback={(c) => tourCallback(c)}
                        styles={{
                            options: {
                                beaconSize: 48,
                                arrowColor: "#e3ffeb",
                                backgroundColor: "var(--amphora-white)",
                                overlayColor: "rgba(179, 173, 176, 0.4)",
                                spotlightShadow: '0 0 15px rgba(0, 0, 0, 0.5)',
                                primaryColor: "var(--berry)",
                                textColor: "var(--amphora-black)",
                                width: 500,
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
