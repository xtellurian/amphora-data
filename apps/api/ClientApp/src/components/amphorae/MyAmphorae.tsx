import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Route } from 'react-router-dom';
import { Button, Spinner } from 'reactstrap';
import { ApplicationState } from '../../redux/state';
import { AmphoraState } from '../../redux/state/amphora';
import AmphoraTable from '../molecules/tables/ConnectedAmphoraTable';
import { actionCreators as listActions } from "../../redux/actions/amphora/list";
import { actionCreators as modalActions } from "../../redux/actions/ui";
import ConnectedAmphoraModal from './ConnectedAmphoraModal';
import { Tabs, activeTab } from '../molecules/tabs';
import { Toggle } from '../molecules/toggles/Toggle';
import { Scope } from './scope';

// At runtime, Redux will merge together...
type MyAmphoraeProps =
  AmphoraState // ... state we've requested from the Redux store
  & (typeof listActions & typeof modalActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ accessType: string }>; // ... plus incoming routing parameters


class MyAmphorae extends React.PureComponent<MyAmphoraeProps> {

  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.setScopeSelf();
  }

  private toggleScope(scope: string | Scope) {
    switch (scope as Scope) {
      case "organisation":
        this.setScopeOrg();
        break;
      case "self":
        this.setScopeSelf();
        break;
    }
  }

  public render() {
    return (
      <React.Fragment>
        <div className="row">
          <div className="col-5">
            <div className="txt-xxl">My Amphora</div>
          </div>
          <div className="col-7">
            <Toggle
              options={[{ text: "My List", id: "self" }, { text: "My Organisation", id: "organisation" }]}
              onOptionSelected={(v) => this.toggleScope(v)} />
          </div>
        </div>
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
        <Tabs default="created" tabs={tabs} />
      </React.Fragment>
    )
  }

  private renderList() {
    if (this.props.isLoading) {
      return (
        <Spinner color="light" />
      )
    }

    return (
      <div>
        {this.renderTabs()}
        {activeTab(this.props.location.search)}
        <AmphoraTable />
      </div>
    )
  }

  private setScopeSelf(): void {
    this.props.listMyCreatedAmphora();
  }

  private setScopeOrg(): void {
    this.props.listOrganisationCreatedAmphora();
  }
}

function mapStateToProps(state: ApplicationState): AmphoraState {
  if (state.amphora) {
    return state.amphora;
  } else {
    return {
      isLoading: true,
      list: [],
      cache: {}
    }
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  { ...listActions, ...modalActions } // Selects which action creators are merged into the component's props
)(MyAmphorae as any);
