import React from "react";
import {
    Pagination,
    PaginationItem,
    PaginationLink,
} from "reactstrap";
import { Link } from "react-router-dom";

const paramsForPage = (page: number, qs?: string): string => {
    const urlSearchParams = new URLSearchParams(qs);
    urlSearchParams.set("page", `${page}`);
    return urlSearchParams.toString();
};

interface PaginationComponentProps {
    baseTo: string;
    className?: string;
    nPages: number;
    page: number;
    qs?: string;
}

interface PageItemProps {
    pageNumber: number;
    baseTo: string;
    qs: URLSearchParams;
    first?: boolean | undefined;
    last?: boolean | undefined;
    next?: boolean | undefined;
    previous?: boolean | undefined;
}
const PageItem: React.FC<PageItemProps> = ({
    baseTo,
    pageNumber,
    qs,
    first,
    next,
    previous,
    last,
}) => {
    const any = first || next || previous || last;
    return (
        <PaginationItem>
            <PaginationLink
                tag={Link}
                to={`${baseTo}?${paramsForPage(
                    Math.max(pageNumber, 0),
                    qs.toString()
                )}`}
                first={first}
                last={last}
                next={next}
                previous={previous}
            >
                {!any && pageNumber}
            </PaginationLink>
        </PaginationItem>
    );
};
export const PaginationComponent: React.FC<PaginationComponentProps> = ({
    baseTo,
    className,
    nPages,
    page,
    qs,
}) => {
    const nearbyPages = [page - 2, page - 1, page, page + 1, page + 2].filter(
        (p) => p > 0 && p <= nPages
    );
    const firstQsParams = new URLSearchParams(qs);
    firstQsParams.set("page", "1");
    const previousQsParams = new URLSearchParams(qs);
    previousQsParams.set("page", `${Math.max(page - 1, 0)}`);
    return (
        <div className={className}>
            <Pagination
                listClassName="justify-content-center"
                aria-label="Page navigation"
            >
                <PageItem
                    first
                    qs={new URLSearchParams(qs)}
                    baseTo={baseTo}
                    pageNumber={1}
                />
                <PageItem
                    previous
                    qs={new URLSearchParams(qs)}
                    baseTo={baseTo}
                    pageNumber={Math.max(page - 1, 1)}
                />
                {nearbyPages.map((p) => (
                    <PageItem
                        key={p}
                        qs={new URLSearchParams(qs)}
                        baseTo={baseTo}
                        pageNumber={p}
                    />
                ))}
                <PageItem
                    next
                    qs={new URLSearchParams(qs)}
                    baseTo={baseTo}
                    pageNumber={Math.min(page + 1, nPages)}
                />
                <PageItem
                    last
                    qs={new URLSearchParams(qs)}
                    baseTo={baseTo}
                    pageNumber={nPages}
                />
            </Pagination>
        </div>
    );
};
