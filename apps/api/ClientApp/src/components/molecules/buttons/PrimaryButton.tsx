import * as React from 'react';
import { ButtonProps } from './buttonProps';

import './buttons.css';

export class PrimaryButton extends React.PureComponent<ButtonProps> {

    public render() {
        let classNames = "button";
        if (this.props.disabled) {
            classNames += " button-disabled";
        } else {
            classNames += " button-primary";
        }

        return (
            <React.Fragment>
                <button className={classNames} disabled={this.props.disabled || false}>
                    {this.props.children}
                </button>
            </React.Fragment>
        );
    }
}