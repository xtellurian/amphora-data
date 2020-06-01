import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Route } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { ApplicationState } from '../../redux/state';
import { AmphoraState } from '../../redux/state/amphora';
import AmphoraTable from '../molecules/tables/connected/ConnectedAmphoraTable';
import { actionCreators as listActions, Scope, AccessType } from "../../redux/actions/amphora/list";
import { actionCreators as modalActions } from "../../redux/actions/ui";
import ConnectedAmphoraModal from './ConnectedAmphoraModal';
import { Tabs, activeTab } from '../molecules/tabs';
import { Toggle } from '../molecules/toggles/Toggle';
import { LoadingState } from '../molecules/empty/LoadingState';

// At runtime, Redux will merge together...
type MyAmphoraeProps =
  AmphoraState // ... state we've requested from the Redux store
  & (typeof listActions & typeof modalActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ accessType: string; scope: string }>; // ... plus incoming routing parameters

interface MyAmphoraeState {
  scope: Scope;
}

class MyAmphorae extends React.Component<MyAmphoraeProps, MyAmphoraeState> {

  /**
   *
   */
  constructor(props: MyAmphoraeProps) {
    super(props);
    this.state = {
      scope: "self"
    }
  }
  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.loadList();
  }

  public componentDidUpdate(prevProps: MyAmphoraeProps) {
    if (this.getAccessType(prevProps.location.search) !== this.getAccessType()) {
      // check if the old access type is not the same as this one.
      this.loadList();
    }
  }

  private getAccessType(queryString?: string | undefined): AccessType {
    const accessType = activeTab(queryString || this.props.location.search, "accessType") as AccessType;
    return accessType || "created";
  }

  private scopeOptionSelected(v: string) {
    this.setState({ scope: v as Scope })
    this.loadList();
  }


  private loadList(): void {
    const scope = this.state.scope;
    const accessType = this.getAccessType();
    this.props.listMyCreatedAmphora(scope, accessType);
  }

  // rendering methods
  public render() {
    return (
      <React.Fragment>
        <div className="row">
          <div className="col-lg-5">
            <div className="txt-xxl">My Amphora</div>
          </div>
          <div className="col-lg-7">
            <Toggle
              options={[{ text: "My List", id: "self" }, { text: "My Organisation", id: "organisation" }]}
              onOptionSelected={(v) => this.scopeOptionSelected(v)} />
          </div>
        </div>
        <hr />
        {this.renderList()}

        <Route path='/amphora/detail/:id' component={ConnectedAmphoraModal} />
      </React.Fragment>
    );
  }

  private renderTabs() {
    const tabs = [
      { id: "created" },
      { id: "purchased" }
    ]
    return (
      <React.Fragment>
        <Tabs name="accessType" default="created" tabs={tabs} />
      </React.Fragment>
    )
  }

  private renderList() {
    if (this.props.isLoading) { return (<LoadingState />) }
    return (
      <div>
        {this.renderTabs()}
        <AmphoraTable scope={this.state.scope} accessType={this.getAccessType()} {...this.props} />
      </div>
    )
  }
}

function mapStateToProps(state: ApplicationState): AmphoraState {
  if (state.amphora) {
    return state.amphora;
  } else {
    return {
      isLoading: true,
      collections: {
        organisation: {
          created: [],
          purchased: []
        },
        self: {
          created: [],
          purchased: []
        }
      },
      cache: {}
    }
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  { ...listActions, ...modalActions } // Selects which action creators are merged into the component's props
)(MyAmphorae as any);
