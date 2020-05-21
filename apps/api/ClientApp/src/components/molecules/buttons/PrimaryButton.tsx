import { Button } from './Button';

export class PrimaryButton extends Button {
    classNames(): string {
        let classNames = `button ${this.props.className}`;
        if (this.props.disabled) {
            classNames += " button-disabled";
        } else {
            classNames += " button-primary";
        }
        return classNames;
    }
}