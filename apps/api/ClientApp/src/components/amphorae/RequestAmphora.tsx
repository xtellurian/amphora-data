import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Button, Spinner } from 'reactstrap';
import { ApplicationState } from '../../redux/state';
import { AmphoraState } from '../../redux/state/amphora';
// import { actionCreators } from '../../redux/actions/amphorae';
import { actionCreators as listActions } from "../../redux/actions/amphora/list";
import { actionCreators as modalActions } from "../../redux/actions/ui";

// At runtime, Redux will merge together...
type RequestAmphoraProps =
  AmphoraState // ... state we've requested from the Redux store
  & (typeof listActions & typeof modalActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class RequestAmphora extends React.PureComponent<RequestAmphoraProps> {

  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.setMyView();
  }

  // This method is called when the route parameters change
  public componentDidUpdate(prevProps: RequestAmphoraProps) {
    // this.ensureDataFetched();
  }

  public render() {
    return (
      <React.Fragment>
        <h1>Request Amphora</h1>
        
      </React.Fragment>
    );
  }

  private setMyView(): void {
    this.props.listMyCreatedAmphora();
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
)(RequestAmphora);
