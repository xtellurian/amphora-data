import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
// import { actionCreators } from '../../redux/actions/amphorae';
import { actionCreators as listActions } from "../../redux/actions/amphora/list";
import { actionCreators as modalActions } from "../../redux/actions/ui";

// At runtime, Redux will merge together...
type FindAmphoraProps =
  (typeof listActions & typeof modalActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class FindAmphora extends React.PureComponent<FindAmphoraProps> {

  // This method is called when the route parameters change
  public componentDidUpdate(prevProps: FindAmphoraProps) {
    // this.ensureDataFetched();
  }

  public render() {
    return (
      <React.Fragment>
        <h1>Find Amphora</h1>
        
      </React.Fragment>
    );
  }
}

export default connect(
  null, // Selects which state properties are merged into the component's props
  { ...listActions, ...modalActions } // Selects which action creators are merged into the component's props
)(FindAmphora);
