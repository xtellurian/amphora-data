import * as React from 'react';
import { InputProps, ValidateResult } from './inputProps';
import { Label } from 'reactstrap';

import './inputs.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

interface TextInputState {
    value: string;
    validation: ValidateResult;
}

export class TextInput extends React.PureComponent<InputProps<string>, TextInputState> {

    componentDidMount() {
        this.setState({ value: this.props.value || "", validation: { isValid: true } });
    }

    private handleKeyDown(event: React.KeyboardEvent<HTMLInputElement>): void {
        if (event.key === 'Enter') {
            event.currentTarget.blur();
        }
    }

    private handleChange(event: React.ChangeEvent<HTMLInputElement>): void {
        if (this.props.validator) {
            const res = this.props.validator(event.target.value);
            this.setState({ value: event.target.value, validation: res });
        } else {
            this.setState({ value: event.target.value, validation: { isValid: true } });
        }
        event.preventDefault();
    }

    private handleBlur(event: React.FocusEvent<HTMLInputElement>): void {
        if (this.props.onComplete) {
            if (this.props.validator) {
                const validatorResult = this.props.validator(this.state.value);
                this.setState({ validation: validatorResult })
                if (validatorResult.isValid) {
                    this.props.onComplete(this.state.value);
                }
            }
            else {
                this.props.onComplete(this.state.value);
            }
        }
    }

    private value(): string | undefined {
        if (this.state) {
            return this.state.value;
        }
    }

    private support(): React.ReactElement | undefined {
        if (this.props.support) {
            return <Label>{this.props.support}</Label>
        }
        else {
            return <React.Fragment></React.Fragment>;
        }
    }

    private validation(): React.ReactElement | null {
        if (!this.state) {
            return null;
        }
        else if (this.state && this.state.validation && !this.state.validation.isValid) {
            return <Label className="text-danger">{this.state.validation.message || "Invalid Input"}</Label>;
        }
        else {
            return null;
        }
    }

    private clearButton(): React.ReactElement | null | undefined {
        if (this.state && this.state.value) {
            return (
                <button onClick={(e) => this.setState({ value: "", validation: { isValid: true } })}>
                    <FontAwesomeIcon icon="times-circle" />
                </button>
            )
        }
    }

    public render() {
        return (
            <div>
                <Label>{this.props.label}</Label>
                <div className="input">
                    <input
                        type="text"
                        value={this.value()}
                        onChange={(e) => this.handleChange(e)}
                        onKeyDown={(e) => this.handleKeyDown(e)}
                        onBlur={(e) => this.handleBlur(e)}
                    />
                    {this.clearButton()}
                </div>
                {this.validation() || this.support()}
            </div>
        );
    }
}