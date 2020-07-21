import * as React from "react";
import { TextInputBase } from "./TextInputBase";
import { InputProps } from "./inputProps";

export class TextInput extends TextInputBase<InputProps<string>> {
    private input?: HTMLInputElement | null;

    componentDidMount() {
        if (this.props.focusOnMount && this.input) {
            this.input.focus();
        }
    }

    private handleKeyDown(event: React.KeyboardEvent<HTMLInputElement>): void {
        if (event.key === "Enter") {
            event.currentTarget.blur();
        }
    }

    public render() {
        const invalidClassName = this.state.validation.isValid
            ? ""
            : "input-invalid";
        return (
            <div id={this.props.id} className={`input-outer ${this.props.className || ""}`}>
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
                        ref={(input) => (this.input = input)}
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
