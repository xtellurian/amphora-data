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
        disableEditing: props.disableEditing,
    });

    React.useEffect(() => {
        if(state.disableEditing !== props.disableEditing) {
            setState({
                ...state,
                disableEditing: props.disableEditing,
            });
        }
    }, [props.disableEditing, state]);

    const onComplete = (value: string) => {
        setState({
            isEditing: false,
            value: value || state.value,
            disableEditing: state.disableEditing,
        });
        props.onSave(value);
    };

    if (state.disableEditing) {
        return <React.Fragment>{props.children}</React.Fragment>;
    }

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
                            setState({
                                value: state.value,
                                isEditing: true,
                                disableEditing: state.disableEditing,
                            })
                        }
                    />
                </React.Fragment>
            )}
        </React.Fragment>
    );
};
