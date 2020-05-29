import * as React from 'react';
import { connect } from 'react-redux';
import { Spinner } from 'reactstrap';
import { AmphoraDetailProps, mapStateToProps } from './props';
import ReactMarkdown from 'react-markdown';
import { DetailedAmphora } from 'amphoradata';
import { LoadingState } from '../../molecules/empty/LoadingState';


class TermsOfUse extends React.PureComponent<AmphoraDetailProps> {

    private renderTerms(amphora: DetailedAmphora) {
        const termsId = amphora.termsOfUseId;
        if (termsId) {
            return { termsId }
        }
        else {
            return (
                <React.Fragment>
                    There are no terms
                </React.Fragment>
            )
        }
    }

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.cache[id];
        if (amphora) {

            return (
                <React.Fragment>
                    {id}
                    <h3>{amphora.name}</h3>

                    {this.renderTerms(amphora)}
                </React.Fragment>
            )

        } else {
            return <LoadingState />
        }

    }
}

export default connect(
    mapStateToProps,
    null,
)(TermsOfUse);