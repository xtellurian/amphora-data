import * as React from 'react';
import { Alert } from '../../../redux/state/ui';

import './messages.css';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconProp } from '@fortawesome/fontawesome-svg-core';

interface MessageProps {
    alert?: Alert;
    dismiss: (id: string) => void;
}
export abstract class MessageBase extends React.PureComponent<MessageProps> {

    protected outerDivClass = "message row justify-content-between";

    protected renderIconColumn(iconName: IconProp) {
        return (
            <div className="icon col-1">
                <FontAwesomeIcon size="lg" icon={iconName} />
            </div>
        )
    }

    protected renderContent(): JSX.Element | undefined {
        if(this.props.alert) {
            return (
                <div className="content col d-flex align-items-center">
                    {this.props.alert.content}
                </div>
            );
        }
    }

    private renderDismiss(): JSX.Element | undefined {
        if (this.props.alert) {
            return (
                <React.Fragment>
                    Dismiss
                </React.Fragment>)
        }
    }

    private renderLink(): JSX.Element | undefined {
        if (this.props.alert && this.props.alert.path) {
            const path = this.props.alert.path;
            return (
                <React.Fragment>
                    <Link to={path}>
                        View
                    </Link>
                </React.Fragment>);
        }
    }

    protected renderActionsColumn(): JSX.Element | undefined {
        if (this.props.alert) {
            const alertId = this.props.alert.id;
            return (<div className="col-3 d-flex align-items-center actions" onClick={() => this.props.dismiss(alertId)}>
                <span className="action">
                    {this.renderLink()}
                </span>
                <span className="action">
                    {this.renderDismiss()}
                </span>

            </div>);
        }
    }
}