import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { warning } from "../../molecules/toasts";
import {
    ModalContents,
    ModalWrapper,
    ModalHeading,
} from "../../molecules/modal";
import { ModalFooter } from "../../molecules/modal/ModalContents";
import { PrimaryButton } from "../../molecules/buttons";
import { Row, Col } from "reactstrap";

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

    const selectPlan = (plan: any) => {
        warning(
            {
                text: "Plan select is currently unavailable",
            },
            {
                autoClose: 2000,
            }
        );
        setState({
            loading: false,
            isOpen: true,
            error: "Can't select a plan yet.",
        });
        // clients.accountApi
        //     .planGetPlan("")
        //     .then((r) => {

        //     })
        //     .catch((e) => warning({ text: `${e}` }));
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
                        <PrimaryButton className="w-100" onClick={() => selectPlan("PAYG")}>
                            PAYG Plan
                        </PrimaryButton>
                    </Col>
                    <Col sm={3}>
                        <PrimaryButton className="w-100" onClick={() => selectPlan("Glaze")}>
                            Glaze
                        </PrimaryButton>
                    </Col>
                </Row>
            </ModalContents>
        </ModalWrapper>
    );
};
