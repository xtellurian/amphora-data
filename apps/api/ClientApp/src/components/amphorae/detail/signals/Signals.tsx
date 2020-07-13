import * as React from "react";
import { Link } from "react-router-dom";
import { OneAmphora } from "../props";
import { LoadingState } from "../../../molecules/empty/LoadingState";
import { PrimaryButton } from "../../../molecules/buttons";
import { SignalsGraph } from "./SignalsGraph";
import { Header } from "../Header";

export const Signals: React.FunctionComponent<OneAmphora> = (props) => {
    const amphoraId = props.amphora.id;
    if (amphoraId) {
        return (
            <React.Fragment>
                <Header title="Signals">
                    <Link
                        to={`/amphora/detail/${props.amphora.id}/signals/add`}
                    >
                        <PrimaryButton>Add Signal</PrimaryButton>
                    </Link>
                </Header>
                <SignalsGraph amphora={props.amphora} />
            </React.Fragment>
        );
    } else {
        return <LoadingState />;
    }
};
