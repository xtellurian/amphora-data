import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Button, Spinner } from 'reactstrap';
import { ApplicationState } from '../../redux/state';
import { AmphoraState } from '../../redux/state/amphora';
import { Table } from '../molecules/tables/Table';
import { actionCreators as listActions } from "../../redux/actions/amphora/list";
import { actionCreators as modalActions } from "../../redux/actions/ui";
import { Route } from 'react-router-dom';
import qs from 'qs';

import ConnectedAmphoraModal from './ConnectedAmphoraModal';
import { Tabs, Tab } from '../molecules/tabs/Tabs';

const CREATED = "created";
const PURCHASED = "purchased";
type AccessType = typeof CREATED | typeof PURCHASED;

// At runtime, Redux will merge together...
type MyAmphoraeProps =
  AmphoraState // ... state we've requested from the Redux store
  & (typeof listActions & typeof modalActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ accessType: string }>; // ... plus incoming routing parameters


class MyAmphorae extends React.PureComponent<MyAmphoraeProps> {

  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.setMyView();
  }

  private countAmphora(): number {
    if (this.props.list) {
      return this.props.list.length;
    } else { }
    return 0;
  }

  public render() {
    return (
      <React.Fragment>
        <div className="txt-xxl">My Amphora</div>
        <Button color="primary" onClick={() => this.setMyView()}>My List</Button>
        <Button color="secondary" onClick={() => this.setOrganisationView()}>My Organisation</Button>
        {this.renderList()}

        <Route path='/amphora/detail/:id' component={ConnectedAmphoraModal} />
      </React.Fragment>
    );
  }

  private getAccessType(): AccessType {
    const search = qs.parse(this.props.location.search, { ignoreQueryPrefix: true });
    console.log(search)
    return search.accessType as AccessType || "created";
  }

  private renderTabs() {
    const accessType = this.getAccessType();
    const tabs: Tab[] = [
      { isActive: accessType==CREATED, text: "Created", to: `${this.props.match.path}?accessType=created` },
      {isActive: accessType==PURCHASED, text: "Purchased", to: `${this.props.match.path}?accessType=purchased` }
    ]
    return (
      <React.Fragment>
        {this.getAccessType()}
        < Tabs tabs={tabs} />
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
        <Table />
        {/* <p>There are {this.countAmphora()} amphoras </p>
        {
          this.props.list.map(a => (
            <AmphoraListItem openModal={() => this.props.open(a)} amphora={a} key={a.id || ""} />
          ))
        } */}
      </div>
    )
  }

  private setMyView(): void {
    this.props.listMyCreatedAmphora();
  }

  private setOrganisationView(): void {
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
