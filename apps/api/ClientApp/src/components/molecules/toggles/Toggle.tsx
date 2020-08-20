import * as React from 'react';
import { SecondaryButton } from '../buttons/SecondaryButton';
import './toggles.css';
import { ButtonGroup } from 'reactstrap';

export interface ToggleOption {
    id: string;
    text: string;
}
interface ToggleProps {
    default?: string;
    options: ToggleOption[];
    onOptionSelected: (option: string) => void;
}
interface ToggleState {
    activeOption: string;
}

export class Toggle extends React.PureComponent<ToggleProps, ToggleState> {
    /**
     *
     */
    constructor(props: ToggleProps) {
        super(props);
        this.state = {
            activeOption: props.default || props.options[0].id
        }
    }
    private isActive(v: ToggleOption): boolean {
        return this.state.activeOption === v.id;
    }
    private handleSelect(v: ToggleOption) {
        this.setState({ activeOption: v.id });
        this.props.onOptionSelected(v.id);
    }

    private renderOption(v: ToggleOption): JSX.Element {
        return (
            <SecondaryButton key={v.id} onClick={() => this.handleSelect(v)} className={`toggle-option ${this.isActive(v) ? "active" : ""}`} variant="slim" >
                {v.text}
            </SecondaryButton>
        )
    }
    render() {
        return (
            <ButtonGroup className="toggle txt-xs">
                {this.props.options.map(o => this.renderOption(o))}
            </ButtonGroup>
        )
    }
}