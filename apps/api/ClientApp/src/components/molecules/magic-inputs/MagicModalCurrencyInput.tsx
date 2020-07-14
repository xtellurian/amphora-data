import * as React from "react";

import {
    ModalCurrencyInput,
    CurrencyInputProps,
} from "../inputs/ModalCurrencyInput";

const style: React.CSSProperties = {
    cursor: "pointer",
};

export const MagicModalCurrencyInput: React.FunctionComponent<CurrencyInputProps> = (
    props
) => {
    const [editing, setEditing] = React.useState(false);

    return (
        <React.Fragment>
            {editing ? (
                <ModalCurrencyInput {...props} onCancel={() => setEditing(false)} />
            ) : (
                <div style={style} onClick={() => setEditing(true)}>
                    ${props.initialValue}
                </div>
            )}
        </React.Fragment>
    );
};
