import * as React from "react";
import { Container } from "reactstrap";

export const PageContainer: React.FC = ({ children }) => {
    return <Container>{children}</Container>;
};
