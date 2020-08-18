import * as React from "react";
import "./tabs.css";

import { Link } from "react-router-dom";
import { RouteComponentProps, withRouter } from "react-router";
import { activeTab } from "./activeTab";

export interface Tab {
    text?: string;
    id: string;
}

type TabsProps = {
    name?: string;
    tabs: Tab[];
    default: string;
} & RouteComponentProps<{ tab: string }>;

class Tabs extends React.PureComponent<TabsProps> {
    private getTabName(): string {
        return this.props.name || "tab";
    }

    private getActiveTabId(): string {
        const tab = activeTab(this.props.location.search, this.getTabName());
        const qs =
            this.props.location.search.length > 0
                ? `${this.props.location.search}&`
                : "?";
        if (!tab) {
            this.props.history.push({
                pathname: this.props.location.pathname,
                search: `${qs}${this.getTabName()}=${
                    this.props.default
                }`,
            });
            return this.props.default;
        } else {
            return tab;
        }
    }

    private isTabActive(t: Tab) {
        return this.getActiveTabId() === t.id;
    }

    renderTab(t: Tab) {
        const commonClassNames = "tab col-3";
        const className = this.isTabActive(t)
            ? `${commonClassNames} active txt-bold`
            : commonClassNames;
        return (
            <div key={t.id} className={className}>
                <Link
                    to={`${this.props.location.pathname}?${this.getTabName()}=${
                        t.id
                    }`}
                >
                    {t.text || t.id}
                </Link>
            </div>
        );
    }

    render() {
        return (
            <React.Fragment>
                <div className="tabs row no-gutters text-sm">
                    {this.props.tabs.map((t) => this.renderTab(t))}
                </div>
            </React.Fragment>
        );
    }
}

export default withRouter(Tabs);
