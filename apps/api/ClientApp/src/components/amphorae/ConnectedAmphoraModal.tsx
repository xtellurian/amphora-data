import * as React from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../../redux/state";
import { actionCreators } from "../../redux/actions/amphora/fetch";
import { ModalWrapper } from "../molecules/modal/ModalWrapper";
import { RouteComponentProps, Route } from "react-router";
import { Description } from "./detail/Description";
import Files from "./detail/Files";
import Signals from "./detail/signals/Signals";
import AddSignal from "./detail/signals/AddSignal";
import Integrate from "./detail/Integrate";
import TermsOfUse from "./detail/TermsOfUse";
import Location from "./detail/Location";
import Quality from "./detail/Quality";
import { Cache } from "../../redux/state/common";
import { AmphoraDetailMenu } from "./detail/AmphoraDetailMenu";
import { DetailedAmphora } from "amphoradata";
import { MagicLabel } from "../molecules/magic-inputs/MagicLabel";
import { LoadingState } from "../molecules/empty/LoadingState";
import { PermissionsState } from "../../redux/state/permissions";
import { PurchaseButton } from "./PurchaseButton";
import "./amphoraModal.css";

export function baseLink(pathname: string): string {
    const split = pathname.split("/");
    if (split.length > 2) {
        return `${split[0]}/${split[1]}/${split[2]}`;
    } else {
        return "/amphora/detail";
    }
}

interface ConnectedAmphoraModalState {
    isOpen: boolean;
}

type ConnectedAmphoraModalProps = {
    cache: Cache<DetailedAmphora>;
    permissions: PermissionsState;
} & typeof actionCreators &
    RouteComponentProps<{ id: string }>;

class ConnectedAmphoraModal extends React.PureComponent<
    ConnectedAmphoraModalProps,
    ConnectedAmphoraModalState
> {
    /**
     *
     */
    constructor(props: ConnectedAmphoraModalProps) {
        super(props);
        this.state = { isOpen: true };
    }
    public componentDidMount() {
        if (this.props.match.params.id) {
            this.props.fetchAmphora(this.props.match.params.id);
        }
    }

    private toggleMenu(isOpen: boolean) {
        this.setState({ isOpen });
    }

    private redirectToPath(): string {
        if (this.props.location.pathname) {
            const split = this.props.location.pathname.split("/");
            if (split.length > 1) {
                return `${split[0]}/${split[1]}`;
            }
        }
        // default to this:
        return "/amphora";
    }

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.cache.store[id];
        const openClose = this.state.isOpen ? "menu-open" : "menu-closed";
        if (amphora) {
            return (
                <ModalWrapper
                    isOpen={true}
                    onCloseRedirectTo={this.redirectToPath()}
                >
                    <div className={openClose}>
                        {/* this renders the master menu */}
                        <AmphoraDetailMenu
                            // permissions={this.props.permissions}
                            {...this.props}
                            id={this.props.match.params.id}
                            toggleMenu={(o) => this.toggleMenu(o)}
                            isOpen={this.state.isOpen}
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
                                path={`${baseLink(
                                    this.props.location.pathname
                                )}/:id/`}
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
                                    this.props.location.pathname
                                )}/:id/files`}
                                component={Files}
                            />

                            <Route
                                exact
                                path={`${baseLink(
                                    this.props.location.pathname
                                )}/:id/signals`}
                                component={Signals}
                            />
                            <Route
                                exact
                                path={`${baseLink(
                                    this.props.location.pathname
                                )}/:id/signals/add`}
                                component={AddSignal}
                            />

                            <Route
                                exact
                                path={`${baseLink(
                                    this.props.location.pathname
                                )}/:id/integrate`}
                                component={Integrate}
                            />
                            <Route
                                exact
                                path={`${baseLink(
                                    this.props.location.pathname
                                )}/:id/terms`}
                                component={TermsOfUse}
                            />
                            <Route
                                exact
                                path={`${baseLink(
                                    this.props.location.pathname
                                )}/:id/location`}
                                component={Location}
                            />
                            <Route
                                exact
                                path={`${baseLink(
                                    this.props.location.pathname
                                )}/:id/quality`}
                                component={Quality}
                            />
                            <div className="purchase-button-row">
                                <PurchaseButton
                                    price={amphora.price || 0}
                                    id={id}
                                />
                            </div>
                        </div>
                    </div>
                </ModalWrapper>
            );
        } else {
            return <LoadingState />;
        }
    }
}

function mapStateToProps(state: ApplicationState) {
    return {
        isLoading: state.amphora.isLoading,
        cache: state.amphora.metadata,
        onCloseRedirectTo: null,
        permissions: state.permissions,
    };
}

export default connect(
    mapStateToProps,
    actionCreators
)(ConnectedAmphoraModal as any);
