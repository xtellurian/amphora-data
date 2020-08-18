import * as React from "react";
import { UserInformationComponent } from "react-amphora";
import { Row, Col } from "reactstrap";
import { User } from "oidc-client";
import { PrimaryButton } from "../molecules/buttons";
import { Header } from "../layout/AccountLayout";
import { PageContainer } from "../layout/PageContainer";

interface ProfilePageProps {
    user?: User;
}

export const ProfilePage: React.FunctionComponent<ProfilePageProps> = (
    props
) => {
    const [showTokenInfo, setShowTokenInfo] = React.useState(false);

    return (
        <PageContainer>
            <Header title="User Profile" />

            <Row>
                <Col xs="8">
                    <UserInformationComponent />
                </Col>
            </Row>
            <Row className="mt-3">
                {showTokenInfo && props.user ? (
                    <Col xs="8">
                        <div className="txt-lg">Technical Details</div>
                        <table className="table">
                            <tbody>
                                <tr>
                                    <td>token_type</td>
                                    <td>{props.user.token_type}</td>
                                </tr>
                                <tr>
                                    <td>access_token</td>
                                    <td>{props.user.access_token}</td>
                                </tr>
                                <tr>
                                    <td>refresh_token</td>
                                    <td>{props.user.refresh_token}</td>
                                </tr>
                                <tr>
                                    <td>expires_at</td>
                                    <td>{props.user.expires_at}</td>
                                </tr>
                                <tr>
                                    <td>scope</td>
                                    <td>{props.user.scope}</td>
                                </tr>
                                {Object.keys(props.user.profile).map((key) => (
                                    <tr key={key}>
                                        <td>{key}</td>
                                        <td>{props.user?.profile[key]}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </Col>
                ) : (
                    <PrimaryButton onClick={() => setShowTokenInfo(true)}>
                        Show More
                    </PrimaryButton>
                )}
            </Row>

            {props.children}
        </PageContainer>
    );
};
