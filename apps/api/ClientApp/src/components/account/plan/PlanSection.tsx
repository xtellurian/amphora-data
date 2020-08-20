import * as React from "react";
import { PlanInformation } from "amphoradata";
import { useAmphoraClients } from "react-amphora";
import { LoadingState, ErrorState } from "../../molecules/empty";
import { PlanInformationCard } from "./PlanInformtionCard";

interface PlanSectionState {
    loading: boolean;
    error?: any;
    information?: PlanInformation;
}

export const PlanSection: React.FC = (props) => {
    const [state, setState] = React.useState<PlanSectionState>({
        loading: true,
    });

    const clients = useAmphoraClients();

    React.useEffect(() => {
        if (clients.isAuthenticated) {
            setState({
                ...state,
                loading: true,
            });
            clients.accountApi
                .planGetPlan2("")
                .then((r) => {
                    setState({
                        loading: false,
                        information: r.data,
                    });
                })
                .catch((e) => {
                    setState({
                        loading: false,
                        error: e,
                    });
                });
        }
    }, [clients.isAuthenticated]);

    if (state.loading) {
        return <LoadingState />;
    } else if (state.information) {
        console.log('plan loaded')
        console.log(state.information)
        return (
            <React.Fragment>
                <PlanInformationCard plan={state.information} />
            </React.Fragment>
        );
    } else {
        return <ErrorState>Error fetching plan.</ErrorState>;
    }
};
