import React from "react";
import { OneAmphora } from "./detail/props";
import { PurchaseButton } from "react-amphora";
import { parseServerError } from "../../utilities/errors";
import { success, error } from "../molecules/toasts";

interface PurchaseButtonProps extends OneAmphora {
    onPurchased: (amphoraId: string) => void;
}
export const PurchaseButtonComponent: React.FunctionComponent<PurchaseButtonProps> = (
    props
) => {
    const amphoraId = props.amphora.id;

    if (!amphoraId) {
        return <div>Loading...</div>;
    }

    const onError = (e: any) => {
        error(parseServerError(e, "Purchase Failed"));
        console.log(e);
    };

    const onPurchased = (amphoraId: string) => {
        success(
            {
                text: "Amphora Purchased!",
                path: `/amphora/detail/${amphoraId}`,
            },
            {
                autoClose: 5000,
            }
        );
        props.onPurchased(amphoraId);
    };
    return (
        <PurchaseButton
            onError={onError}
            onPurchased={onPurchased}
            amphoraId={amphoraId}
        ></PurchaseButton>
    );
};
