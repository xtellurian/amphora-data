import * as React from "react";
import { Row, Col } from "reactstrap";
import { Link } from "react-router-dom";
import { TabContainer, Tab } from "../molecules/tabs/Tabs";
import { useLocation } from "react-router";

interface HeaderProps {
    title: string;
}

export const Header: React.FC<HeaderProps> = (props) => {
    return (
        <Row>
            <Col lg={5}>
                <div className="txt-xxl">{props.title}</div>
            </Col>
            <Col lg={7} className="text-right">
                {props.children}
            </Col>
        </Row>
    );
};

const AccountTab: React.FC<{ name: string }> = ({ name }) => {
    const location = useLocation();
    return (
        <Tab isActive={() => location.pathname === `/account/${name}`}>
            <Link to={`/account/${name}`}>
                <div className="text-capitalize">{name}</div>
            </Link>
        </Tab>
    );
};

export const AccountNav: React.FC = () => {
    return (
        <TabContainer>
            <AccountTab name="members" />
            <AccountTab name="transactions" />
            <AccountTab name="invoices" />
            <AccountTab name="plan" />
        </TabContainer>
    );
};
