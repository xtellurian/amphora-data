import * as React from 'react';
import { ToastContent } from './model';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconProp } from '@fortawesome/fontawesome-svg-core';

export interface ToastProps {
    content?: ToastContent;
}

export abstract class ToastBase extends React.PureComponent<ToastProps> {

    protected outerDivClass = "text-sm txt-ebony message row justify-content-between";

    protected renderIconColumn(iconName: IconProp) {
        return (
            <div className="icon col-1">
                <FontAwesomeIcon size="lg" icon={iconName} />
            </div>
        )
    }

    protected renderContent(): JSX.Element | undefined {
        if (this.props.content) {
            return (
                <div className="content col d-flex align-items-center">
                    {this.props.content.text}
                </div>
            );
        }
    }

    private renderLink(): JSX.Element | undefined {
        if (this.props.content && this.props.content.path) {
            const path = this.props.content.path;
            return (
                <React.Fragment>
                    <Link to={path}>
                        View
                    </Link>
                </React.Fragment>);
        }
    }

    protected renderActionsColumn(): JSX.Element | undefined {
        if (this.props.content) {
            return (<div className="col-3 d-flex align-items-center actions" onClick={() => 1}>
                <span className="action">
                    {this.renderLink()}
                </span>
            </div>);
        }
    }
}