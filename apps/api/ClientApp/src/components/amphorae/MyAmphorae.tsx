import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../../redux/store';
import * as AmphoraStore from '../../redux/AmphoraState';
import { actionCreators } from '../../redux/actions/amphorae';

// At runtime, Redux will merge together...
type AmphoraeProps =
  AmphoraStore.AmphoraState // ... state we've requested from the Redux store
  & typeof actionCreators  // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class MyAmphorae extends React.PureComponent<AmphoraeProps> {
  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.ensureDataFetched();
  }

  // This method is called when the route parameters change
  public componentDidUpdate(prevProps: AmphoraeProps) {
    // this.ensureDataFetched();
  }

  public render() {
    return (
      <React.Fragment>
        <h1 id="tabelLabel">Weather forecast</h1>
        <p>This component demonstrates fetching amphora from the server and working with URL parameters.</p>
        <p>There are {this.props.amphoras.length} amphoras </p>
        {this.renderForecastsTable()}
        {this.renderPagination()}
      </React.Fragment>
    );
  }

  private ensureDataFetched() {
    const startDateIndex = parseInt(this.props.match.params.startDateIndex, 10) || 0;

    this.props.getMyCreatedAmphorae(startDateIndex);

  }

  private renderForecastsTable() {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Name</th>
            <th>Description</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {this.props.amphoras.map((amphora: AmphoraStore.AmphoraModel) =>
            <tr key={amphora.id}>
              <td>{amphora.name}</td>
              <td>{amphora.description}</td>
              <td>{amphora.price}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  private renderPagination() {
    const prevStartDateIndex = (this.props.startDateIndex || 0) - 5;
    const nextStartDateIndex = (this.props.startDateIndex || 0) + 5;

    return (
      <div className="d-flex justify-content-between">
        <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${prevStartDateIndex}`}>Previous</Link>
        {this.props.isLoading && <span>Loading...</span>}
        <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${nextStartDateIndex}`}>Next</Link>
      </div>
    );
  }
}

const mapStateToProps = (state: ApplicationState) => ({
  amphoras: state.amphoras
});

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  actionCreators // Selects which action creators are merged into the component's props
)(MyAmphorae as any);
