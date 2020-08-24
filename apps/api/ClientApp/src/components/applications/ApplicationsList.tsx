import * as React from "react";
import { Application } from "amphoradata";
import { Row, Col } from "reactstrap";
import { useAmphoraClients } from "react-amphora";
import { LoadingState, ErrorState, EmptyState } from "../molecules/empty";
import { SecondaryButton } from "../molecules/buttons";
import { Link } from "react-router-dom";

interface ApplicationsListState {
    loading: boolean;
    error?: any;
    applications: Application[];
}

interface ApplicationRowProps {
    application: Application;
}

const ApplicationsRowHeader: React.FC = () => {
    return (
        <Row className="m-3">
            <Col>
                <div className="txt-med">Name</div>
            </Col>
            <Col>
                <div className="txt-med">Allowed Origins</div>
            </Col>
            <Col xs={2}></Col>
        </Row>
    );
};
const ApplicationsRow: React.FC<ApplicationRowProps> = ({ application }) => {
    return (
        <Row className="p-3 mt-1 align-items-center border rounded">
            <Col>{application.name}</Col>
            <Col className="align-middle">
                {application.locations &&
                    application.locations.map((l) => `${l.origin},`)}
            </Col>
            <Col xs={2}>
                <Link to={`applications/edit?id=${application.id}`}>
                    <SecondaryButton>Edit</SecondaryButton>
                </Link>
            </Col>
        </Row>
    );
};
export const ApplicationsList: React.FC = () => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<ApplicationsListState>({
        loading: true,
        applications: [],
    });

    React.useEffect(() => {
        if (clients.isAuthenticated) {
            console.log("Loading applications...");
            clients.applicationsApi
                .applicationsGetApplications()
                .then((r) => {
                    setState({
                        loading: false,
                        applications: r.data.items || [],
                    });
                })
                .catch((e) => {
                    setState({
                        loading: false,
                        error: e,
                        applications: state.applications,
                    });
                });
        }
    }, [clients.isAuthenticated]);

    if (state.loading) {
        return <LoadingState />;
    } else if (state.error) {
        return <ErrorState>{`${state.error}`}</ErrorState>;
    } else if (state.applications.length === 0) {
        return <EmptyState>No Applications</EmptyState>;
    } else {
        return (
            <React.Fragment>
                <ApplicationsRowHeader />
                {state.applications.map((a, i) => (
                    <ApplicationsRow key={i} application={a} />
                ))}
            </React.Fragment>
        );
    }
};
