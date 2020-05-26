import * as React from 'react';
import { InputProps, ValidateResult } from './inputProps';
import { Label } from 'reactstrap';

import './inputs.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

interface TextInputState {
    value: string;
    validation: ValidateResult;
}

export abstract class TextInputBase extends React.PureComponent<InputProps<string>, TextInputState> {

    /**
     *
     */
    constructor(props: InputProps<string>) {
        super(props);
        this.state = {
            validation: {
                isValid: true,
                message: ""
            },
            value: ""
        }
    }
    componentDidMount() {
        this.setState({ value: this.props.value || "", validation: { isValid: true } });
    }

    protected handleChange(event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>): void {
        if (this.props.validator) {
            const res = this.props.validator(event.target.value);
            this.setState({ value: event.target.value, validation: res });
        } else {
            this.setState({ value: event.target.value, validation: { isValid: true } });
        }
        event.preventDefault();
    }

    protected handleBlur(event: React.FocusEvent<HTMLInputElement| HTMLTextAreaElement>): void {
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

    protected value(): string | undefined {
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
                <button onClick={(e) => this.setState({ value: "", validation: { isValid: true } })} tabIndex={-1}>
                    <FontAwesomeIcon icon="times-circle" />
                </button>
            )
        }
    }
}