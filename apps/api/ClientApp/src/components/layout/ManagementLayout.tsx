import * as React from "react";
import { Row, Col } from "reactstrap";

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
