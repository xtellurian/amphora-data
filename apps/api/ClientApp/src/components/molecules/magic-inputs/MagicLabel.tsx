import * as React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { TextInput } from "../inputs/TextInput";
import { MagicProps } from "./MagicProps";
import "./magic.css";

export const MagicLabel: React.FunctionComponent<MagicProps<string>> = (
    props
) => {
    const [state, setState] = React.useState({
        value: props.initialValue,
        isEditing: false,
    });

    const onComplete = (value: string) => {
        setState({ isEditing: false, value: value || state.value });
        props.onSave(value);
    };

    return (
        <React.Fragment>
            {state.isEditing ? (
                <TextInput
                    focusOnMount={true}
                    value={state.value}
                    onComplete={(v) => onComplete(v || "")}
                    label=""
                />
            ) : (
                <React.Fragment>
                    {props.children}
                    <FontAwesomeIcon
                        icon="pencil-alt"
                        className="edit-button"
                        onClick={() =>
                            setState({ value: state.value, isEditing: true })
                        }
                    />
                </React.Fragment>
            )}
        </React.Fragment>
    );
};
