import * as React from "react";
import { connect } from "react-redux";
import { RouteComponentProps, Route } from "react-router";
import ConnectedSearchTable from '../tables/ConnectedSearchTable';
import { actionCreators as actions } from "../../redux/actions/search/searchAmphora";
import { TextInput } from "../molecules/inputs";
import { ApplicationState } from "../../redux/state";
import { SearchState } from "../../redux/state/search";
import { LoadingState } from "../molecules/empty/LoadingState";
import ConnectedAmphoraModal from "./ConnectedAmphoraModal";

// At runtime, Redux will merge together...
type FindAmphoraProps = {
  state: SearchState;
} &
  typeof actions & // ... plus action creators we've requested
  RouteComponentProps<{ term: string }>; // ... plus incoming routing parameters

class FindAmphora extends React.PureComponent<FindAmphoraProps> {
  // This method is called when the route parameters change
  public componentDidUpdate(prevProps: FindAmphoraProps) {
    // this.ensureDataFetched();
  }

  private doSearch(term?: string) {
    if(term) {
      this.props.searchAmphora({term, page: 0})
    }
  }

  private renderLoader(): JSX.Element | undefined {
    if(this.props.state.isLoading) {
      return <LoadingState/>
    }
  }

  private renderTable(): JSX.Element | undefined {
    if(!this.props.state.isLoading) {
      return <ConnectedSearchTable {...this.props} />
    }
  }

  public render() {
    return (
      <React.Fragment>
        <h2>Search</h2>
        <TextInput onComplete={(t) => this.doSearch(t)} label="" placeholder="Enter a term..." />
        {this.renderLoader()}
        {this.renderTable()}

        <Route path='/search/detail/:id' component={ConnectedAmphoraModal} />
      </React.Fragment>
    );
  }
}

function mapStateToProps(state: ApplicationState) {
  return {
    state: state.search,
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  { ...actions } // Selects which action creators are merged into the component's props
)(FindAmphora);
