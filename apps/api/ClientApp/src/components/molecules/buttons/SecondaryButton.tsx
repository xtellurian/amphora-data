import { Button } from './Button';

export class SecondaryButton extends Button {

    classNames(): string {
        let classNames = "button";
        if (this.props.disabled) {
            classNames += " button-disabled";
        } else {
            classNames += " button-secondary";
        }
        return classNames;
    }
}