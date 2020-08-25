import * as React from "react";
import { Application } from "amphoradata";
import { Row, Col } from "reactstrap";
import { useAmphoraClients } from "react-amphora";
import { LoadingState, ErrorState, EmptyState } from "../molecules/empty";
import { SecondaryButton, OverflowMenuButton } from "../molecules/buttons";
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
            <Col lg={5} className="d-none d-lg-block">
                <div className="txt-med">Allowed Origins</div>
            </Col>
            <Col xs={4} lg={3}></Col>
        </Row>
    );
};
const ApplicationsRow: React.FC<ApplicationRowProps> = ({ application }) => {
    return (
        <Row className="p-3 mt-1 align-items-center border rounded">
            <Col>{application.name}</Col>
            <Col lg={5} xs={5} className="d-none d-lg-block align-middle">
                {application.locations &&
                    application.locations.map((l) => `${l.origin},`)}
            </Col>
            <Col lg={3} className="d-none d-lg-block">
                <Link to={`applications/edit?id=${application.id}`}>
                    <SecondaryButton>Edit</SecondaryButton>
                </Link>
            </Col>
            <Col lg={1} className="d-none d-lg-block text-right">
                <OverflowMenuButton>
                    <Link to={`applications/delete?id=${application.id}`}>
                        <SecondaryButton className="w-100">
                            Delete
                        </SecondaryButton>
                    </Link>
                </OverflowMenuButton>
            </Col>
            <Col xs={4} className="d-lg-none text-right pr-5">
                <OverflowMenuButton>
                    <Link to={`applications/edit?id=${application.id}`}>
                        <SecondaryButton className="w-100">
                            Edit
                        </SecondaryButton>
                    </Link>
                    <Link to={`applications/delete?id=${application.id}`}>
                        <SecondaryButton className="w-100">
                            Delete
                        </SecondaryButton>
                    </Link>
                </OverflowMenuButton>
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
        if (clients.isAuthenticated ) {
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
                        applications: [],
                    });
                });
        }
    }, [clients.isAuthenticated, clients.applicationsApi]);

    if (state.loading) {
        return <LoadingState />;
    } else if (state.error) {
        return <ErrorState>{`${state.error}`}</ErrorState>;
    } else if (state.applications.length === 0) {
        return <EmptyState>No Applications</EmptyState>;
    } else {
        return (
            <div className="pb-5">
                <ApplicationsRowHeader />
                {state.applications.map((a, i) => (
                    <ApplicationsRow key={i} application={a} />
                ))}
            </div>
        );
    }
};
