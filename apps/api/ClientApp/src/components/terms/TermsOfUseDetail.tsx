import * as React from "react";
import { TermsOfUse } from "amphoradata";
import { TermsOfUseContext } from "react-amphora";
import ReactMarkdown from "react-markdown";
import { useParams } from "react-router";
import { EmptyState } from "../molecules/empty/EmptyState";
import { ModalWrapper } from "../molecules/modal/ModalWrapper";
interface TermsOfUseDetailProps {
    terms: TermsOfUse;
}
export const ConnectedTermsOfUseDetail: React.FunctionComponent = () => {
    const params = useParams<{ id: string }>();
    const context = TermsOfUseContext.useTermsState();
    const actions = TermsOfUseContext.useTermsDispatch();
    const [loaded, setLoaded] = React.useState(false);
    React.useEffect(() => {
        if (!loaded) {
            actions.dispatch({
                type: "terms:fetch-single",
                payload: {
                    id: params.id,
                },
            });
            setLoaded(true);
        }
    }, [actions, params.id, loaded]);

    const terms = context.results.find((t) => t.id === params.id);
    if (terms) {
        return (
            <ModalWrapper isOpen={true} onCloseRedirectTo="/terms">
                <div className="modal-inner">
                    <TermsOfUseDetail terms={terms} />
                </div>
            </ModalWrapper>
        );
    } else {
        return (
            <React.Fragment>
                <EmptyState>Terms not found</EmptyState>
            </React.Fragment>
        );
    }
};

export const TermsOfUseDetail: React.FunctionComponent<TermsOfUseDetailProps> = ({
    terms,
}) => {
    return (
        <React.Fragment>
            <h3>{terms.name}</h3>
            <hr />
            <ReactMarkdown>{terms.contents}</ReactMarkdown>
        </React.Fragment>
    );
};
