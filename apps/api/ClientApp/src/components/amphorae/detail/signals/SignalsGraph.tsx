import * as React from "react";

import { Components } from "react-amphora";
import { Signal, DetailedAmphora } from "amphoradata";
import { EmptyState } from "../../../molecules/empty/EmptyState";
import { LoadingState } from "../../../molecules/empty/LoadingState";

interface SignalProps {
    amphora: DetailedAmphora;
    signals?: Signal[] | undefined;
}

export const SignalsGraph: React.FunctionComponent<SignalProps> = (props) => {
    const amphoraId = props.amphora.id;
    if (amphoraId) {
        return (
            <React.Fragment>
                <Components.SignalsChart
                    emptyComponent={<EmptyState >There are no Signals in this Amphora</EmptyState>}
                    loadingComponent={<LoadingState > Just getting things ready...</LoadingState>}
                    amphoraId={amphoraId}
                    signals={props.signals}
                    legend="shown"
                    noAnimate={true}
                />
                {props.children}
            </React.Fragment>
        );
    } else {
        return <EmptyState />;
    }
};
