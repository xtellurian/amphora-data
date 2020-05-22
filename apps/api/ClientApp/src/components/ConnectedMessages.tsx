import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../redux/state';
import { UiState, Alert, SUCCESS, INFO, WARNING, ERROR } from '../redux/state/ui';
import { actionCreators } from '../redux/actions/ui';

import { SuccessMessage, InfoMessage, WarningMessage, ErrorMessage } from './molecules/messages';

type MessagesProps =
    UiState
    & typeof actionCreators;

class ConnectedMessages extends React.PureComponent<MessagesProps> {
    private renderMessages() {
        if (this.props.alerts) {
            return this.props.alerts.map(a => this.renderMessage(a));
        }
    }

    private renderMessage(alert: Alert) {
        switch (alert.level) {
            case SUCCESS:
                return <SuccessMessage key={alert.id} alert={alert} dismiss={(id) => this.dismissMessage(id)} />
            case INFO:
                return <InfoMessage key={alert.id} alert={alert} dismiss={(id) => this.dismissMessage(id)} />
            case WARNING:
                return <WarningMessage key={alert.id} alert={alert} dismiss={(id) => this.dismissMessage(id)} />
            case ERROR:
                return <ErrorMessage key={alert.id} alert={alert} dismiss={(id) => this.dismissMessage(id)} />
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
