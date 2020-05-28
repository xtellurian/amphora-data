import * as React from 'react';
import { ButtonProps } from './buttonProps';

import './buttons.css';

export abstract class Button extends React.PureComponent<ButtonProps> {

    abstract classNames(): string; // must be implemented in derived classes

    protected commonClasses = "btn button txt-sm txt-bold";
    protected disabledClass = "button-disabled";
    public render() {
        let classNames = `${this.commonClasses} ${this.classNames()} ${this.props.className}`;
        if (this.props.variant === "slim") {
            classNames += " slim";
        }
        return (
            <React.Fragment>
                <button
                    onClick={this.props.onClick}
                    className={classNames}
                    disabled={this.props.disabled || false}
                    style={this.props.style}>
                    {this.props.children}
                </button>
            </React.Fragment>
        );
    }
}