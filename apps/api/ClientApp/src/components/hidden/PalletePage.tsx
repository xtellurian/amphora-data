import * as React from "react";
import { PrimaryButton, SecondaryButton } from "../molecules/buttons";
import { TextInput } from "../molecules/inputs";
import { ValidateResult } from "../molecules/inputs/inputProps";
import { Toggle } from "../molecules/toggles/Toggle";
import { Tabs, activeTab } from "../molecules/tabs";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { RouteComponentProps } from "react-router";

interface TabState {
    created?: boolean;
    requested?: boolean;
    purchased?: boolean;
}

type PalleteProps = RouteComponentProps<{ tab: string }>;

export default class Pallete extends React.PureComponent<
    PalleteProps,
    TabState
> {
    /**
     *
     */
    constructor(props: PalleteProps) {
        super(props);
        this.state = {
            created: true,
        };
    }

    public validateText(t?: string): ValidateResult {
        if (t && t.length < 10) {
            return {
                isValid: false,
                message: "Text must be more than 10 chars.",
            };
        } else {
            return { isValid: true };
        }
    }

    public render() {
        return (
            <React.Fragment>
                <h2>Tabs</h2>
                <Tabs
                    default="created"
                    tabs={[
                        { id: "created" },
                        { text: "Data Requested", id: "requested" },
                        { id: "purchased" },
                    ]}
                />
                The active tab is {activeTab(this.props.location.search)}
                <h2>Toggles</h2>
                <div className="w-100">
                    <Toggle
                        options={[
                            { text: "Option One", id: "1" },
                            { text: "Option Two", id: "2" },
                        ]}
                        onOptionSelected={(v) => alert(v)}
                    />
                </div>
                <h2>Icons</h2>
                <FontAwesomeIcon icon="times-circle" />
                <hr />
                <h2>Buttons</h2>
                <PrimaryButton
                    onClick={(e) => alert("Primary Button was clicked")}
                >
                    Primary Button
                </PrimaryButton>
                <PrimaryButton disabled={true}>Primary Disabled</PrimaryButton>
                <SecondaryButton>Secondary Button</SecondaryButton>
                <SecondaryButton disabled={true}>
                    Secondary Disabled
                </SecondaryButton>
                <hr />
                <PrimaryButton variant="slim">
                    Primary Slim Button
                </PrimaryButton>
                <SecondaryButton variant="slim">
                    Secondary Slim Button
                </SecondaryButton>
                <hr />
                <h2> Inputs </h2>
                <TextInput
                    label="An Input Label"
                    helpText={(v) => "Some support text"}
                    onComplete={(v) => alert("Input completed: " + v)}
                    validator={(v) => this.validateText(v)}
                />
            </React.Fragment>
        );
    }
}
