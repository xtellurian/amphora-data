import { Button } from './Button';

export class PrimaryButton extends Button {
    classNames(): string {
        return this.props.disabled ? this.disabledClass : "button-primary";
    }
}