import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../redux/state';
import { TermsOfUseState } from '../../redux/state/terms';
import { LoadingState } from '../molecules/empty/LoadingState';
import { actionCreators as listActions } from '../../redux/actions/terms/list';

import { PrimaryButton } from '../molecules/buttons'
import { emptyCache } from '../../redux/state/common';
import { TermsOfUse } from 'amphoradata';
import { EmptyState } from '../molecules/empty/EmptyState';

// At runtime, Redux will merge together...
type TermsOfUseProps =
    TermsOfUseState
    & typeof listActions
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class TermsOfUseComponent extends React.Component<TermsOfUseProps> {

    componentDidMount() {
        this.loadList();
    }

    private loadList(): void {
        this.props.listTerms();
    }

    // rendering methods
    public render() {
        return (
            <React.Fragment>
                <div className="row">
                    <div className="col-lg-5">
                        <div className="txt-xxl">Terms of Use</div>
                    </div>
                    <div className="col-lg-7 text-right">
                        <PrimaryButton>
                            Add Terms of Use
                        </PrimaryButton>
                    </div>
                </div>
                <hr />
                {this.renderList()}

                {/* <Route path='/amphora/detail/:id' component={ConnectedAmphoraModal} /> */}
            </React.Fragment>

        );
    }

    private renderList() {
        if (this.props.isLoading) { return (<LoadingState />) }
        else if (this.props.termIds && this.props.termIds.length > 0) {
            return (
                <div>Some terms in temrids  {this.props.termIds.length}</div>
            )
        } else {
            return (
                <EmptyState>
                    There are no terms of use here yet.
                </EmptyState>
            )
        }
    }
}

function mapStateToProps(state: ApplicationState): TermsOfUseState {
    if (state.terms) {
        return state.terms;
    } else {
        return {
            cache: emptyCache<TermsOfUse>(),
            isLoading: true,
            termIds: []
        }
    }
}

export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    listActions // Selects which action creators are merged into the component's props
)(TermsOfUseComponent);
