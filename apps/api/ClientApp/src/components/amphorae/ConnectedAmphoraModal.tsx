import * as React from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../../redux/state";
import { actionCreators } from "../../redux/actions/amphora/fetch";
import { ModalWrapper } from "../molecules/modal/ModalWrapper";
import { RouteComponentProps, Route } from "react-router";
import Description from "./detail/Description";
import Files from "./detail/Files";
import Signals from "./detail/signals/Signals";
import AddSignal from "./detail/signals/AddSignal";
import Integrate from "./detail/Integrate";
import TermsOfUse from "./detail/TermsOfUse";
import Location from "./detail/Location";
import Quality from "./detail/Quality";
import { Cache } from "../../redux/state/common";
import DetailMenu from "./detail/AmphoraDetailMenu";
import { DetailedAmphora } from "amphoradata";
import { LoadingState } from "../molecules/empty/LoadingState";

interface ConnectedAmphoraModalState {
  isOpen: boolean;
}

type ConnectedAmphoraModalProps = {
  cache: Cache<DetailedAmphora>;
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

  public render() {
    const id = this.props.match.params.id;
    const amphora = this.props.cache.store[id];
    const openClose = this.state.isOpen ? "menu-open" : "menu-closed";
    if (amphora) {
      return (
        <ModalWrapper isOpen={true} onCloseRedirectTo="/amphora">
          <div className={openClose}>
            {/* this renders the master menu */}
            <DetailMenu
              {...this.props}
              id={this.props.match.params.id}
              toggleMenu={(o) => this.toggleMenu(o)}
              isOpen={this.state.isOpen}
            />
            <div className="modal-inner">
              <div className="mb-2">
                <h4>{amphora.name}</h4>
                <small>
                  Created{" "}
                  {amphora.createdDate
                    ? "on " + amphora.createdDate.toLocaleString()
                    : "earlier"}
                </small>
              </div>
              {/* these render the detail views */}
              <Route
                exact
                path="/amphora/detail/:id/"
                component={Description}
              />
              <Route exact path="/amphora/detail/:id/files" component={Files} />

              <Route
                exact
                path="/amphora/detail/:id/signals"
                component={Signals}
              />
              <Route
                exact
                path="/amphora/detail/:id/signals/add"
                component={AddSignal}
              />

              <Route
                exact
                path="/amphora/detail/:id/integrate"
                component={Integrate}
              />
              <Route
                exact
                path="/amphora/detail/:id/terms"
                component={TermsOfUse}
              />
              <Route
                exact
                path="/amphora/detail/:id/location"
                component={Location}
              />
              <Route
                exact
                path="/amphora/detail/:id/quality"
                component={Quality}
              />
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
  };
}

export default connect(
  mapStateToProps,
  actionCreators
)(ConnectedAmphoraModal as any);
