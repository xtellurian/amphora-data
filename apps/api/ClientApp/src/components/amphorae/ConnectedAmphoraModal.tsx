import * as React from "react";
import { ModalWrapper } from "../molecules/modal/ModalWrapper";
import { RouteComponentProps, Route } from "react-router";
import { AmphoraOperationsContext } from "react-amphora";
import { Description } from "./detail/Description";
import { FilesPage } from "./detail/Files";
import { Signals } from "./detail/signals/Signals";
import AddSignal from "./detail/signals/AddSignalComponent";
import { Integrate } from "./detail/Integrate";
import { TermsOfUsePage } from "./detail/TermsOfUse";
import { Location } from "./detail/Location";
import { QualityPage } from "./detail/Quality";
import { AmphoraDetailMenu } from "./detail/AmphoraDetailMenu";
import { MagicLabel } from "../molecules/magic-inputs/MagicLabel";
import { LoadingState } from "../molecules/empty/LoadingState";
import { EmptyState } from "../molecules/empty/EmptyState";
import { PurchaseButtonComponent } from "./PurchaseButton";
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
                type: "amphora-operation:read",
                payload: { id: props.match.params.id },
            });
            setState({
                isOpen: state.isOpen,
                loadAttempts: state.loadAttempts + 1,
            });
        }
        console.log(context);
    }, [context, props.match.params.id, state, actions]);

    const toggleMenu = (isOpen: boolean) => {
        setState({ isOpen, loadAttempts: state.loadAttempts });
    };

    if (context.isLoading) {
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

        const handleUpdateName = (name: string) => {
            if (name !== amphora.name) {
                actions.dispatch({
                    type: "amphora-operation:update",
                    payload: {
                        id: amphora.id || "",
                        model: {
                            ...amphora,
                            name,
                        },
                    },
                });
            }
        };

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
                        <div className="mb-1">
                            <div>
                                <MagicLabel
                                    initialValue={amphora.name}
                                    onSave={(v) => handleUpdateName(v)}
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
                                    maxPermissionLevel={
                                        context.maxPermissionLevel
                                    }
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
                            render={(props) => (
                                <Signals {...props} amphora={amphora} />
                            )}
                        />
                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/signals/add`}
                            render={(props) => (
                                <AddSignal {...props} amphora={amphora} />
                            )}
                        />

                        <Route
                            exact
                            path={`${baseLink(
                                props.location.pathname
                            )}/:id/integrate`}
                            render={(props) => (
                                <Integrate
                                    {...props}
                                    amphora={amphora}
                                    name={"<user name>"}
                                />
                            )}
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
                            render={(props) => (
                                <Location {...props} amphora={amphora} />
                            )}
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
                            <PurchaseButtonComponent amphora={amphora} />
                        </div>
                    </div>
                </div>
            </ModalWrapper>
        );
    } else {
        return (
            <ModalWrapper isOpen={true} onCloseRedirectTo={redirectToPath()}>
                <EmptyState>Unknown Amphora...</EmptyState>;
            </ModalWrapper>
        );
    }
};
