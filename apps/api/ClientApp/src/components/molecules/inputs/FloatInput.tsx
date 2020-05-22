import * as React from 'react';
import { InputProps, ValidateResult } from './inputProps';
import { Label } from 'reactstrap';

import './inputs.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

interface NumberInputState {
    value?: number;
    validation: ValidateResult;
}

export abstract class FloatInput extends React.PureComponent<InputProps<number>, NumberInputState> {

    /**
     *
     */
    constructor(props: InputProps<number>) {
        super(props);
        this.state = {
            validation: {
                isValid: true,
                message: ""
            },
            value: 0
        }
    }

    private valueFromString(value?: string): number | undefined {
        if (value) {
            return Number.parseFloat(value);
        }
    }

    protected handleChange(event: React.ChangeEvent<HTMLInputElement>): void {
        const n = this.valueFromString(event.target.value);
        if (this.props.validator) {
            if (n) {
                const res = this.props.validator(n);
                this.setState({ value: n, validation: res });
            }
        } else {
            this.setState({ value: n, validation: { isValid: true } });
        }
        event.preventDefault();
    }

    protected handleBlur(event: React.FocusEvent<HTMLInputElement | HTMLTextAreaElement>): void {
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

    protected value(): number | undefined {
        if (this.state) {
            return this.state.value;
        }
    }

    protected helpText(): React.ReactElement | undefined {
        if (this.props.helpText) {
            return <Label>{this.props.helpText(this.state.value)}</Label>
        }
        else {
            return <React.Fragment></React.Fragment>;
        }
    }

    protected validation(): React.ReactElement | null {
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

    protected clearButton(): React.ReactElement | null | undefined {
        if (this.state && this.state.value) {
            return (
                <button onClick={(e) => this.setState({ value: 0, validation: { isValid: true } })}>
                    <FontAwesomeIcon icon="times-circle" />
                </button>
            )
        }
    }

    render() {
        const invalidClassName = this.state.validation.isValid ? "" : "input-invalid";
        return (
            <React.Fragment>
                <div className="input-outer">
                    <span>
                        <strong>{this.props.label}</strong>
                    </span>
                    <div className={`input-inner ${invalidClassName}`}>
                        <input
                            type="number"
                            placeholder={this.props.placeholder}
                            value={this.value()}
                            onChange={(e) => this.handleChange(e)}
                            onBlur={(e) => this.handleBlur(e)}
                        />
                        {this.clearButton()}
                    </div>
                    <span className="text-muted">
                        {this.validation() || this.helpText()}
                    </span>
                </div>
            </React.Fragment>
        )
    }
}