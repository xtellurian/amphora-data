import * as React from 'react';
import { MessageBase } from './MessageBase';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class InfoMessage extends MessageBase {
    render() {
        if (this.props.alert) {
            return (
                <div className={`message-info ${this.outerDivClass}`}>
                    {this.renderIconColumn("info-circle")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div>);
        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}