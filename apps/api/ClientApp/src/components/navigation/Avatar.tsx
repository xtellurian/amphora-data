import * as React from 'react';
import { connect } from 'react-redux';
import { OidcState } from '../../redux/state/plugins/oidc';
import { ApplicationState } from '../../redux/state'
import { Spinner } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faUser } from '@fortawesome/free-solid-svg-icons'

type AvatarState =
    {className: string}
    & OidcState;// ... state we've requested from the Redux store

class Avatar extends React.PureComponent<AvatarState> {

    public render() {
        if (this.props.isLoadingUser) {
            return (
                <Spinner className={this.props.className} color="light" />
            );
        } else if (this.props.user && this.props.user.profile && this.props.user.profile.name) {
            return (
                <div className={`${this.props.className} ml-2 d-flex align-items-center`}>
                    <FontAwesomeIcon className="align-middle mr-2" size="sm" icon={faUser} />
                    {`Hi, ${this.props.user.profile.name}`}
                </div>
            )
        } else {
            return (
                <div className={`${this.props.className} ml-2 d-flex align-items-center`}>
                    <FontAwesomeIcon className="align-middle" size="lg" icon={faUser} />
                </div>
            );
        }
    }
}

function mapStateToProps(state: ApplicationState) {
    return state.oidc;
}

export default connect(
    mapStateToProps,
    null
)(Avatar as any);