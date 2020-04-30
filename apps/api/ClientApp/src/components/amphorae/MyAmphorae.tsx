import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Button, Spinner } from 'reactstrap';
import { ApplicationState } from '../../redux/state';
import { AmphoraState } from '../../redux/state/amphora';
// import { actionCreators } from '../../redux/actions/amphorae';
import { actionCreators as listActions } from "../../redux/actions/amphora/list";
import { actionCreators as modalActions } from "../../redux/actions/ui";
import { AmphoraListItem } from './AmphoraListItem';

// At runtime, Redux will merge together...
type MyAmphoraeProps =
  AmphoraState // ... state we've requested from the Redux store
  & (typeof listActions & typeof modalActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class MyAmphorae extends React.PureComponent<MyAmphoraeProps> {

  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.setMyView();
  }

  // This method is called when the route parameters change
  public componentDidUpdate(prevProps: MyAmphoraeProps) {
    // this.ensureDataFetched();
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
        <h1 id="tabelLabel">My Amphora</h1>
        <Button color="primary" onClick={() => this.setMyView()}>My List</Button>
        <Button color="secondary" onClick={() => this.setOrganisationView()}>My Organisation</Button>
        {this.renderList()}
      </React.Fragment>
    );
  }

  private renderList() {
    if (this.props.isLoading) {
      return (
        <Spinner color="light" />
      )
    }

    return (
      <div>
        <p>There are {this.countAmphora()} amphoras </p>
        {
          this.props.list.map(a => (
            <AmphoraListItem openModal={() => this.props.open(a)} amphora={a} key={a.id || ""} />
          ))
        }
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
      list: []
    }
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  { ...listActions, ...modalActions } // Selects which action creators are merged into the component's props
)(MyAmphorae as any);
