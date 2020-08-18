import * as React from "react";
import { Application } from "amphoradata";
import { Row, Col } from "reactstrap";

interface ApplicationsListProps {
    applications: Application[];
}
interface ApplicationRowProps {
    application: Application;
}

const ApplicationsRow: React.FC<ApplicationRowProps> = ({ application }) => {
    return (
        <Row>
            <Col>{application.name}</Col>
        </Row>
    );
};
export const ApplicationsList: React.FC<ApplicationsListProps> = ({
    applications,
}) => {
    return (
        <React.Fragment>
            {applications.map((a, i) => (
                <ApplicationsRow key={i} application={a} />
            ))}
        </React.Fragment>
    );
};
