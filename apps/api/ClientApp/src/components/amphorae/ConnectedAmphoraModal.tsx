import * as React from "react";
import { ModalWrapper } from "../molecules/modal/ModalWrapper";
import { RouteComponentProps, Route } from "react-router";
import { AmphoraOperationsContext } from "react-amphora";
import { Description } from "./detail/Description";
import { FilesPage } from "./detail/Files";
import Signals from "./detail/signals/Signals";
import AddSignal from "./detail/signals/AddSignal";
import Integrate from "./detail/Integrate";
import { TermsOfUsePage } from "./detail/TermsOfUse";
import Location from "./detail/Location";
import { QualityPage } from "./detail/Quality";
import { AmphoraDetailMenu } from "./detail/AmphoraDetailMenu";
import { MagicLabel } from "../molecules/magic-inputs/MagicLabel";
import { LoadingState } from "../molecules/empty/LoadingState";
import { EmptyState } from "../molecules/empty/EmptyState";
import { PurchaseButton } from "./PurchaseButton";
import "./amphoraModal.css";

type ConnectedAmphoraModalProps = RouteComponentProps<{ id: string }>;

export function baseLink(pathname: string): string {
    const split = pathname.split("/");
    if (split.length > 2) {
        return `${split[0]}/${split[1]}/${split[2]}`;
    } else {
        return "/amphora/detail";
    }
}

export const ConnectedAmphoraModal: React.FunctionComponent<ConnectedAmphoraModalProps> = (
    props
) => {
    const redirectToPath = (): string => {
        if (props.location.pathname) {
            const split = props.location.pathname.split("/");
            if (split.length > 1) {
                return `${split[0]}/${split[1]}`;
            }
        }
        // default to this:
        return "/amphora";
    };

    const context = AmphoraOperationsContext.useAmphoraOperationsState();
    const actions = AmphoraOperationsContext.useAmphoraOperationsDispatch();

    const [state, setState] = React.useState({
        isOpen: true,
        loadAttempts: 0,
    });

    React.useEffect(() => {
        const isCorrectId =
            context.current && context.current.id === props.match.params.id;
        if (!context.isLoading && !isCorrectId && state.loadAttempts < 4) {
            actions.dispatch({
                type: "amphora-operation-read",
                payload: { id: props.match.params.id },
            });
            setState({
                isOpen: state.isOpen,
                loadAttempts: state.loadAttempts + 1,
            });
        }
        console.log(context);
    });

    const toggleMenu = (isOpen: boolean) => {
        setState({ isOpen, loadAttempts: state.loadAttempts });
    };

    if (context.isLoading) {
        console.log("RENDERING loading...");
        return (
            <ModalWrapper isOpen={true} onCloseRedirectTo={redirectToPath()}>
                <LoadingState />;
            </ModalWrapper>
        );
    } else if (context.current) {
        console.log("RENDERING current");
        const amphora = context.current;
        const terms = context.terms;
        const openClose = state.isOpen ? "menu-open" : "menu-closed";
        return (
            <ModalWrapper isOpen={true} onCloseRedirectTo={redirectToPath()}>
                <div className={openClose}>
                    {/* this renders the master menu */}
                    <AmphoraDetailMenu
                        {...props}
                        id={props.match.params.id}
                        toggleMenu={(o) => toggleMenu(o)}
                        isOpen={state.isOpen}
                        maxPermissionLevel={context.maxPermissionLevel}
                    />
                    <div className="modal-inner">
                        <div className="mb-2">
                            <div>
                                <MagicLabel
                                    initialValue={amphora.name}
                                    onSave={(v) => alert(v)}
                                >
                                    <span className="txt-lg">
                                        {amphora.name}
                                    </span>
                                </MagicLabel>
                            </div>
                            <small>
                                Created{" "}
                                {amphora.createdDate
                                    ? "on " +
                                      amphora.createdDate.toLocaleString()
                                    : "earlier"}
                            </small>
                        </div>
                        {/* these render the detail views */}
                        <Route
                            exact
                            path={`${baseLink(props.location.pathname)}/:id/`}
                            render={(props) => (
                                <Description
                                    {...props}
                                    amphora={amphora}
                                    isLoading={false}
                                />
                            )}
                        />
                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/files`}
                            render={(props) => (
                                <FilesPage {...props} amphora={amphora} />
                            )}
                        />

                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/signals`}
                            component={Signals}
                        />
                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/signals/add`}
                            component={AddSignal}
                        />

                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/integrate`}
                            component={Integrate}
                        />
                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/terms`}
                            render={(props) => (
                                <TermsOfUsePage
                                    {...props}
                                    amphora={amphora}
                                    terms={terms}
                                />
                            )}
                        />
                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/location`}
                            component={Location}
                        />
                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/quality`}
                            render={(props) => (
                                <QualityPage {...props} amphora={amphora} />
                            )}
                        />
                        <div className="purchase-button-row">
                            <PurchaseButton
                                price={amphora.price || 0}
                                id={amphora.id || ""}
                            />
                        </div>
                    </div>
                </div>
            </ModalWrapper>
        );
    } else {
        console.log("RENDERING unknown empty");
        return (
            <ModalWrapper isOpen={true} onCloseRedirectTo={redirectToPath()}>
                <EmptyState>Unknown Amphora...</EmptyState>;
            </ModalWrapper>
        );
    }
};
