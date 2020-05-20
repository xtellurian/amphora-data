import * as React from 'react';
import { TextInputBase } from './TextInputBase';


export class TextInput extends TextInputBase {

    private handleKeyDown(event: React.KeyboardEvent<HTMLInputElement>): void {
        if (event.key === 'Enter') {
            event.currentTarget.blur();
        }
    }

    public render() {
        const invalidClassName = this.state.validation.isValid ? "" : "input-invalid";
        return (
            <div className="input-outer">
                <span>
                    <strong>{this.props.label}</strong>
                </span>
                <div className={`input-inner ${invalidClassName}`}>
                    <input
                        type="text"
                        placeholder={this.props.placeholder}
                        value={this.value()}
                        onChange={(e) => this.handleChange(e)}
                        onKeyDown={(e) => this.handleKeyDown(e)}
                        onBlur={(e) => this.handleBlur(e)}
                    />
                    {this.clearButton()}
                </div>
                <span className="text-muted">
                    {this.validation() || this.helpText()}
                </span>
            </div>
        );
    }
}