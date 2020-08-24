import * as React from "react";
import { Row, Col } from "reactstrap";
import { InfoIcon } from "../molecules/info/InfoIcon";
interface HeaderProps {
    title: string;
    helpText?: string;
}

export const Header: React.FC<HeaderProps> = ({
    title,
    helpText,
    children,
}) => {
    return (
        <Row>
            <Col lg={5}>
                <div className="txt-xxl">
                    {title}
                    {helpText && <InfoIcon content={helpText} />}
                </div>
            </Col>
            <Col lg={7} className="text-right">
                {children}
            </Col>
        </Row>
    );
};
