import * as React from "react";
import { Card, CardBody, CardHeader, CardFooter } from "reactstrap";
import { PlanInformation } from "amphoradata";
import { PrimaryButton } from "../../molecules/buttons";
import { Link } from "react-router-dom";
interface PlanInformtionCardProps {
    plan: PlanInformation;
}
export const PlanInformationCard: React.FC<PlanInformtionCardProps> = ({
    plan,
}) => {
    return (
        <Card>
            <CardHeader>
                <h3>{plan.friendlyName} Plan</h3>
            </CardHeader>
            <CardBody>
                For more plan information,{" "}
                <a href="https://www.amphoradata.com/pricing">click here</a>
            </CardBody>
            <CardFooter>
                <Link to="/account/select-plan">
                    <PrimaryButton>Select Plan</PrimaryButton>
                </Link>
            </CardFooter>
        </Card>
    );
};
