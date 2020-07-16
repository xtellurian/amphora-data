import * as React from "react";
import { TermsOfUseContext } from "react-amphora";
import {TermsListItem} from './TermsListItem'

export const TermsOfUseList: React.FunctionComponent = (props) => {
    const context = TermsOfUseContext.useTermsState();
    const actions = TermsOfUseContext.useTermsDispatch();

    const [dispatched, setDispatched] = React.useState(false);
    React.useEffect(() => {
        if (context.isAuthenticated && !dispatched) {
            actions.dispatch({ type: "fetch-terms" });
            setDispatched(true);
        }
    }, [actions, context.isAuthenticated, dispatched]);

    return (
        <React.Fragment>
            {context.results.map((t) => (
                <TermsListItem key={t.id || ""} terms={t} />
            ))}
            {props.children}
        </React.Fragment>
    );
};
