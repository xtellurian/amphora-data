import * as React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { TextAreaInput } from "../inputs/TextAreaInput";
import { MagicProps } from "./MagicProps";
import "./magic.css";

const descriptionStyle: React.CSSProperties = {
    width: "90%",
};
export const MagicTextArea: React.FunctionComponent<MagicProps<string>> = (
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

    const startEditing = () => {
        setState({
            value: state.value,
            isEditing: true,
        });
    };

    return (
        <React.Fragment>
            <div style={descriptionStyle}>
                {state.isEditing ? (
                    <React.Fragment>
                        <TextAreaInput
                            focusOnMount={true}
                            value={state.value}
                            onComplete={(v) => onComplete(v || "")}
                            label=""
                        />
                        <FontAwesomeIcon
                            icon="save"
                            style={{ color: "blue" }}
                            className="float-right"
                            onClick={() =>
                                setState({
                                    value: state.value,
                                    isEditing: true,
                                })
                            }
                        />
                    </React.Fragment>
                ) : (
                    <div onClick={startEditing}>
                        {!props.disableEditing && (
                            <FontAwesomeIcon
                                icon="pencil-alt"
                                className="float-right edit-button"
                                onClick={startEditing}
                            />
                        )}
                        {props.children}
                    </div>
                )}
            </div>
        </React.Fragment>
    );
};
