import * as React from "react";
import { TermsOfUse } from "amphoradata";
import { Link } from "react-router-dom";

interface TermsListItemProps {
    terms: TermsOfUse;
}

export const TermsListItem: React.FunctionComponent<TermsListItemProps> = (
    props
) => {
    return (
        <React.Fragment>
            <Link to={`/terms/detail/${props.terms.id}`}>
                <div>{props.terms.name}</div>
            </Link>
        </React.Fragment>
    );
};
