import * as React from 'react';
import { MessageBase } from './MessageBase';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class SuccessMessage extends MessageBase {
    render() {
        if (this.props.alert) {
            return (
                <div className={`message-success ${this.outerDivClass}`}>
                    {this.renderIconColumn("check")}

                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div>);
        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}