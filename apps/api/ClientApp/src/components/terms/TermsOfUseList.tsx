import * as React from "react";
import { TermsOfUseContext } from "react-amphora";
import { TermsOfUse } from "amphoradata";
import ReactMarkdown from "react-markdown";
import { Link } from "react-router-dom";
import { Row, Col } from "reactstrap";
import { SecondaryButton, OverflowMenuButton } from "../molecules/buttons";

interface TermsListItemProps {
    terms: TermsOfUse;
}

const TermsRowHeader: React.FC = () => {
    return (
        <Row className="m-3 align-items-center">
            <Col>
                <div className="txt-med">Name</div>
            </Col>
            <Col lg={5} xs={5} className="d-md-block d-none">
                <div className="txt-med">Content Preview</div>
            </Col>
            <Col xs={2} lg={2}></Col>
        </Row>
    );
};
const MAX_TERMS_LENGTH = 100;
const TermsRow: React.FC<TermsListItemProps> = ({ terms }) => {
    let contents = terms.contents;
    if (contents.length > MAX_TERMS_LENGTH) {
        const indexOfNewline = contents.indexOf("\n");
        if (indexOfNewline > 0 && indexOfNewline < MAX_TERMS_LENGTH) {
            contents = contents.substr(0, indexOfNewline);
        } else {
            contents = contents.substr(0, MAX_TERMS_LENGTH);
        }
    }
    return (
        <Row className="p-3 mt-1 align-items-center border rounded">
            <Col>{terms.name}</Col>
            <Col lg={5} xs={5} className="d-none d-md-block overflow-hidden">
                <ReactMarkdown>{contents}</ReactMarkdown>
            </Col>
            <Col lg={2} className="d-none d-lg-block">
                <Link to={`/terms/detail/${terms.id}`}>
                    <SecondaryButton className="w-100 m-4">
                        View
                    </SecondaryButton>
                </Link>
            </Col>
            <Col xs={2} className="d-lg-none d-block">
                <OverflowMenuButton>
                    <Link to={`/terms/detail/${terms.id}`}>
                        <SecondaryButton className="w-100">
                            View
                        </SecondaryButton>
                    </Link>
                </OverflowMenuButton>
            </Col>
        </Row>
    );
};

export const TermsOfUseList: React.FunctionComponent = (props) => {
    const context = TermsOfUseContext.useTermsState();
    const actions = TermsOfUseContext.useTermsDispatch();

    const [dispatched, setDispatched] = React.useState(false);
    React.useEffect(() => {
        if (context.isAuthenticated && !dispatched) {
            actions.dispatch({ type: "terms:fetch-list" });
            setDispatched(true);
        }
    }, [actions, context.isAuthenticated, dispatched]);

    return (
        <React.Fragment>
            <TermsRowHeader />
            {context.results.map((t) => (
                <TermsRow key={t.id || ""} terms={t} />
            ))}
            {props.children}
        </React.Fragment>
    );
};
