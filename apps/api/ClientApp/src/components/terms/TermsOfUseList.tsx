import * as React from "react";
import { TermsOfUseContext } from "react-amphora";
import { TermsOfUse } from "amphoradata";
import { Link } from "react-router-dom";
import { Row, Col } from "reactstrap";
import { SecondaryButton, OverflowMenuButton } from "../molecules/buttons";

interface TermsListItemProps {
    terms: TermsOfUse;
}

const TermsRowHeader: React.FC = () => {
    return (
        <Row className="m-3">
            <Col>
                <div className="txt-med">Name</div>
            </Col>
            <Col lg={5} className="d-none d-lg-block">
                <div className="txt-med">Content</div>
            </Col>
            <Col xs={4} lg={3}></Col>
        </Row>
    );
};
const TermsRow: React.FC<TermsListItemProps> = ({ terms }) => {
    return (
        <Row className="p-3 mt-1 align-items-center border rounded">
            <Col>{terms.name}</Col>
            <Col lg={5} xs={5} className="d-none d-md-block overflow-hidden">
                {terms.contents}
            </Col>
            <Col lg={2} className="d-none d-lg-block">
                <Link to={`/terms/detail/${terms.id}`}>
                    <SecondaryButton className="w-100 m-4">View</SecondaryButton>
                </Link>
            </Col>
            <Col xs={2} className="d-lg-none d-block">
                <OverflowMenuButton>
                    <Link to={`/terms/detail/${terms.id}`}>
                        <SecondaryButton className="w-100">View</SecondaryButton>
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
            {context.results.map((t) => (
                <TermsRow key={t.id || ""} terms={t} />
            ))}
            {props.children}
        </React.Fragment>
    );
};
