import * as React from "react";
import { OneAmphora } from "./detail/props";
import { PurchaseButton } from "react-amphora";

export const PurchaseButtonComponent: React.FunctionComponent<OneAmphora> = (
    props
) => {
    const amphoraId = props.amphora.id;

    if (!amphoraId) {
        return <div>Loading...</div>;
    }
    return (
        <PurchaseButton
            onError={(e) => console.log(e)}
            amphoraId={amphoraId}
        ></PurchaseButton>
    );
};
