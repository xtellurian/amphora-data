import { Button } from './Button';

export class SecondaryButton extends Button {

    classNames(): string {
        return this.props.disabled ? this.disabledClass : "button-secondary";
    }
}