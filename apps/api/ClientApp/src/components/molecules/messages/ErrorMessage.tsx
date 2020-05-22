import * as React from 'react';
import { MessageBase } from './MessageBase';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class ErrorMessage extends MessageBase {
    render() {
        if (this.props.alert) {
            const alertId = this.props.alert.id;
            return (
                <div className={`message-error ${this.outerDivClass}`}>
                    {this.renderIconColumn("exclamation-circle")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                    
                </div>);
        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}