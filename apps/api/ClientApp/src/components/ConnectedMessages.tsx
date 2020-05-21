import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../redux/state';
import { UiState } from '../redux/state/ui';
import { actionCreators } from '../redux/actions/ui';

import { Message } from './molecules/messages/Message';

type MessagesProps =
    UiState
    & typeof actionCreators;

class ConnectedMessages extends React.PureComponent<MessagesProps> {
    private renderMessages() {
        if (this.props.alerts) {
            const x = this.props.alerts.map(a =>
                <Message dismiss={(id) => this.dismissMessage(id)} alert={a} key={a.id} />
            );

            console.log(x)
            return x;
        }
    }

    private dismissMessage(id: string) {
        this.props.popAlert(id);
    }

    public render(): React.ReactElement {
        return (
            <React.Fragment>
                <div className="w-75">
                    {this.renderMessages()}

                </div>
            </React.Fragment>
        );
    }
}

function mapStateToProps(state: ApplicationState) {
    return state.ui
}

export default connect(
    mapStateToProps,
    actionCreators
)(ConnectedMessages);
