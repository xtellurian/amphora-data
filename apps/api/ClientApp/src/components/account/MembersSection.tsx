import * as React from "react";
import { Container, Row, Col } from "reactstrap";
import { Membership } from "amphoradata";
import { useAmphoraClients } from "react-amphora";
import { LoadingState } from "../molecules/empty/LoadingState";
import { EmptyState } from "../molecules/empty/EmptyState";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface MembersSectionState {
    loading: boolean;
    members?: Membership[];
    error?: any;
}

interface MemberRowProps {
    member: Membership;
}
const MemberRow: React.FC<MemberRowProps> = ({ member }) => {
    return (
        <Row>
            <Col xs={2}>
                <FontAwesomeIcon icon="user" />
            </Col>
            <Col xs={4}>{member.username}</Col>
            <Col>{member.userId}</Col>
        </Row>
    );
};
export const MembersSection: React.FC = () => {
    const [state, setState] = React.useState<MembersSectionState>({
        loading: true,
    });
    const clients = useAmphoraClients();
    React.useEffect(() => {
        setState({ loading: true });
        clients.accountApi
            .membershipGetMemberships()
            .then((r) => {
                setState({ members: r.data.items || [], loading: false });
            })
            .catch((e) => {
                setState({ error: e, loading: false });
            });
    }, []);

    if (state.loading) {
        return <LoadingState />;
    } else if (state.error) {
        return (
            <React.Fragment>
                An Error Occurred
                <div>{`${state.error}`}</div>
            </React.Fragment>
        );
    } else if (state.members) {
        return (
            <Container>
                <div className="mt-3">
                    {state.members.map((m, i) => (
                        <MemberRow key={i} member={m} />
                    ))}
                </div>
            </Container>
        );
    } else {
        return <EmptyState>No Members Found</EmptyState>;
    }
};
