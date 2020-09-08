import * as React from "react";
import { success, error } from "../../molecules/toasts";
import { parseServerError } from "../../../utilities";
import {
    ModalContents,
    ModalWrapper,
    ModalHeading,
} from "../../molecules/modal";
import { PrimaryButton } from "../../molecules/buttons";
import { Row, Col } from "reactstrap";
import { useAmphoraClients } from "react-amphora";

type Plan = "PAYG" | "Glaze";

interface SelectPlanModalState {
    loading: boolean;
    error?: any;
    isOpen: boolean;
}
export const SelectPlanModal: React.FC = (props) => {
    const [state, setState] = React.useState<SelectPlanModalState>({
        isOpen: true,
        loading: false,
    });

    const clients = useAmphoraClients();

    const selectPlan = (plan: Plan) => {
        setState({
            loading: true,
            isOpen: true,
        });

        clients.accountApi
            .planSetPlan(plan)
            .then((r) => {
                success(
                    { text: `Plan set to  ${r.data.friendlyName}` },
                    {
                        autoClose: 2000,
                    }
                );
                setState({ loading: false, isOpen: false });
            })
            .catch((e) => {
                const text = parseServerError(e);
                error({ text });
                setState({
                    loading: false,
                    isOpen: true,
                    error: e,
                });
            });
    };
    return (
        <ModalWrapper onCloseRedirectTo="/account/plan" isOpen={state.isOpen}>
            <ModalContents>
                <ModalHeading>Select your new Plan</ModalHeading>
                <hr />
                <p>Amphora Data has two plans:</p>
                <ul>
                    <li>A Pay as you Go (PAYG) plan</li>
                    <li>A Glaze subscription plan</li>
                </ul>

                <Row className="mt-5 m-auto">
                    <Col sm={3}>
                        <PrimaryButton
                            className="w-100"
                            onClick={() => selectPlan("PAYG")}
                        >
                            PAYG Plan
                        </PrimaryButton>
                    </Col>
                    <Col sm={3}>
                        <PrimaryButton
                            className="w-100"
                            onClick={() => selectPlan("Glaze")}
                        >
                            Glaze
                        </PrimaryButton>
                    </Col>
                </Row>
            </ModalContents>
        </ModalWrapper>
    );
};
