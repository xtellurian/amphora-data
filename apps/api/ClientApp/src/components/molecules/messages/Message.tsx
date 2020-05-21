import * as React from 'react';
import { Alert, SUCCESS, INFO, WARNING, ERROR } from '../../../redux/state/ui';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import './messages.css';

interface MessageProps {
    alert?: Alert;
    dismiss: (id: string) => void;
}
export class Message extends React.PureComponent<MessageProps> {

    public render() {
        if (this.props.alert) {
            return this.renderAlert(this.props.alert);
        } else {
            return (
                <React.Fragment></React.Fragment>
            )
        }
    }

    private renderAlert(alert: Alert) {
        const commonClass = "message row";
        switch (alert.level) {
            case SUCCESS:
                return (
                    <div className={`message-success ${commonClass}`}>
                        <div className="icon">
                            <FontAwesomeIcon size="lg" icon="check" />
                        </div>
                        <div className="message-content">
                            {alert.content}
                        </div>
                    </div>);
            case INFO:
                return (
                    <div className={`message-info ${commonClass}`}>
                        <div className="icon">
                            <FontAwesomeIcon size="lg" icon="info-circle" />
                        </div>
                        <div className="content">
                            {alert.content}
                        </div>
                    </div>);
            case WARNING:
                return (
                    < div className={`message-warn ${commonClass}`} >
                        <div className="icon">
                            <FontAwesomeIcon size="lg" icon="exclamation-triangle" />
                        </div>
                        <div className="content">
                            {alert.content}
                        </div>
                    </div >);
            case ERROR:
                return (
                    <div className={`message-error ${commonClass}`}>
                        <div className="icon">
                            <FontAwesomeIcon size="lg" icon="exclamation-circle" />
                        </div>
                        <div className="content">
                            {alert.content}
                        </div>
                        <div className="text-right" onClick={() => this.props.dismiss(alert.id)}>
                            DISMISS
                        </div>
                    </div>);
            default:
                return (
                    <div className={`${commonClass}`}>
                        {alert.content}
                    </div>);
        }
    }
}