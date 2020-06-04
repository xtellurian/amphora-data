import * as React from "react";

interface HeaderProps {
    title: string;
    children?: React.ReactNode;
}
export const Header = (props: HeaderProps) => {
    return (
        <React.Fragment>
            <div className="mr-1 row">
                <div className="col-6">
                    <h3>{props.title}</h3>
                </div>
                <div className="col-6 pr-5 text-right">{props.children}</div>
            </div>
            <hr />
        </React.Fragment>
    );
};
