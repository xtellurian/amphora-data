import * as React from "react";
import ReactMarkdown from "react-markdown";
import { OneAmphora } from "./props";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";

export const Description: React.FunctionComponent<OneAmphora> = (props) => {
    if (props.isLoading) {
        return <LoadingState />;
    } else if (props.amphora) {
        return (
            <React.Fragment>
                <Header title="Description">
                    <div>Price: ${props.amphora.price}</div>
                </Header>

                <ReactMarkdown>{props.amphora.description}</ReactMarkdown>
            </React.Fragment>
        );
    } else {
        return <LoadingState />;
    }
};
