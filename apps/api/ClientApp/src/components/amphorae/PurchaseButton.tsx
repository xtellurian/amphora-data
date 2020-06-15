import * as React from "react";
import { PrimaryButton } from "../molecules/buttons/PrimaryButton";
import { LoadingState } from "../molecules/empty/LoadingState";
import { amphoraApiClient } from "../../clients/amphoraApiClient";
import { permissionClient } from "../../clients/amphoraApiClient";
import { AxiosResponse } from "axios";
import { PermissionsResponse } from "amphoradata";

interface PurchaseButtonProps {
    id: string;
    price: number;
}

interface PurchaseButtonState {
    canPurchase?: boolean;
    isLoading?: boolean;
    isError?: boolean;
}

export class PurchaseButton extends React.PureComponent<
    PurchaseButtonProps,
    PurchaseButtonState
> {
    constructor(props: PurchaseButtonProps) {
        super(props);
        this.state = {
            isLoading: false,
            isError: false,
        };
    }

    componentDidMount() {
        this.setState({ isLoading: true });
        permissionClient
            .permissionGetPermissions({
                accessQueries: [{ accessLevel: 32, amphoraId: this.props.id }],
            })
            .then((r) => this.handlePermission(r))
            .catch((e) => this.handleError(e));
    }

    handleError(e: any): any {
        this.setState({ isError: true });
    }
    handlePermission(r: AxiosResponse<PermissionsResponse>): any {
        if (r.data.accessResponses) {
            const p = r.data.accessResponses.find(
                (_) => _.amphoraId === this.props.id
            );
            this.setState({
                isLoading: false,
                isError: false,
                canPurchase: !(p && p.isAuthorized),
            });
        } else {
            this.setState({ isLoading: false, isError: true });
        }
    }

    private purchase(
        e: React.MouseEvent<HTMLButtonElement, MouseEvent> | undefined
    ): void {
        this.setState({ isLoading: true });
        amphoraApiClient
            .purchasesPurchase(this.props.id)
            .then((p) =>
                this.setState({ isLoading: false, canPurchase: false })
            )
            .catch((e) => this.setState({ isLoading: false, isError: true }));
        setTimeout(() => this.setState({ isLoading: false }), 1000);
    }

    render() {
        if (this.state.isLoading) {
            return <LoadingState />;
        } else if (this.state.canPurchase) {
            return (
                <PrimaryButton onClick={(e) => this.purchase(e)}>
                    Get Data for $ {this.props.price}
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
    }
}
