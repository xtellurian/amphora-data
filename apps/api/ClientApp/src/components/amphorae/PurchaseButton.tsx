import * as React from "react";
import { PrimaryButton } from "../molecules/buttons/PrimaryButton";
import { OneAmphora } from "./detail/props";
import { useAmphoraClients } from "react-amphora";
import { LoadingState } from "../molecules/empty/LoadingState";

interface PurchaseButtonProps {
    id: string;
    price: number;
}

export const PurchaseButtonComponent: React.FunctionComponent<OneAmphora> = (
    props
) => {
    const [loading, setLoading] = React.useState(false);
    const [canPurchase, setCanPurchase] = React.useState(
        props.maxPermissionLevel && props.maxPermissionLevel >= 32
    );
    const clients = useAmphoraClients();
    const amphoraId = props.amphora.id;

    if (!amphoraId) {
        return <div>Loading...</div>;
    }

    const purchase = (
        e: React.MouseEvent<HTMLButtonElement, MouseEvent> | undefined
    ): void => {
        setLoading(true);
        clients.amphoraeApi
            .purchasesPurchase(amphoraId)
            .then((p) => {
                setLoading(false);
                setCanPurchase(false);
            })
            .catch((e) => setLoading(false));
        setTimeout(() => setLoading(false), 1000);
    };

    if (loading) {
        return (
            <PrimaryButton>
                <LoadingState />
            </PrimaryButton>
        );
    }
    else if (canPurchase) {
        return (
            <PrimaryButton onClick={(e) => purchase(e)}>
                Get Data for $ {props.amphora.price}
            </PrimaryButton>
        );
    } else {
        // can already read, no need to purchase
        return (
            <React.Fragment>
                <small>You have access to this data.</small>
            </React.Fragment>
        );
    }
};
