import * as React from 'react';
import { MessageBase } from './MessageBase';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class WarningMessage extends MessageBase {
    render() {
        if (this.props.alert) {
            return (
                <div className={`message-warn ${this.outerDivClass}`} >
                    {this.renderIconColumn("exclamation-triangle")}

                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div >);
        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}