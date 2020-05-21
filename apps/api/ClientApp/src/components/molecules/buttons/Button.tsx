import * as React from 'react';
import { ButtonProps } from './buttonProps';

import './buttons.css';

export abstract class Button extends React.PureComponent<ButtonProps> {

    abstract classNames(): string; // must be implemented in derived classes

    public render() {
        return (
            <React.Fragment>
                <button 
                    onClick={this.props.onClick} 
                    className={this.classNames()} 
                    disabled={this.props.disabled || false}
                    style={this.props.style}>
                    {this.props.children}
                </button>
            </React.Fragment>
        );
    }
}