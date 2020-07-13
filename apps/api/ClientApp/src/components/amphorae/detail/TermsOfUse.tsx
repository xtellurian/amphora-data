import * as React from "react";
import { OneAmphora } from "./props";
import ReactMarkdown from "react-markdown";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { EmptyState } from "../../molecules/empty/EmptyState";
import { Header } from "./Header";

export const TermsOfUsePage: React.FunctionComponent<OneAmphora> = (props) => {
    if (props.isLoading) {
        return (
            <React.Fragment>
                <LoadingState />
            </React.Fragment>
        );
    } else if (props.terms) {
        return (
            <React.Fragment>
                <Header title="Terms of Use" />
                <div className="txt-med">{props.terms.name}</div>
                <hr />
                <ReactMarkdown>{props.terms.contents}</ReactMarkdown>
            </React.Fragment>
        );
    } else {
        return (
            <React.Fragment>
                <EmptyState>There are no terms</EmptyState>
            </React.Fragment>
        );
    }
};
