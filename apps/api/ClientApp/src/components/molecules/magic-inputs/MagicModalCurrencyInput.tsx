import * as React from "react";

import {
    ModalCurrencyInput,
    CurrencyInputProps,
} from "../inputs/ModalCurrencyInput";
import classNames from "classnames";

const style: React.CSSProperties = {
    backgroundColor: "var(--amphora-white)",
    border: "2px solid var(--stone)",
    borderRadius: "5%",
    textAlign: "center",
    height: "75%",
    justifyContent: "center",
    marginLeft: "2rem",
    marginRight: "2rem",
    cursor: "pointer",
};

export const MagicModalCurrencyInput: React.FunctionComponent<CurrencyInputProps> = (
    props
) => {
    const [editing, setEditing] = React.useState(false);
    const [value, setValue] = React.useState(props.initialValue);

    const onSave = (v: number) => {
        props.onSave(v);
        setValue(v);
        setEditing(false);
    };
    return (
        <React.Fragment>
            {editing ? (
                <ModalCurrencyInput
                    className={props.className}
                    initialValue={value}
                    onSave={onSave}
                    onCancel={() => setEditing(false)}
                />
            ) : (
                <div
                    className={classNames(props.className)}
                    style={style}
                    onClick={() => setEditing(true)}
                >
                    ${value}
                </div>
            )}
        </React.Fragment>
    );
};
