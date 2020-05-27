import * as React from 'react';
import './tabs.css';

import { Link } from 'react-router-dom';
import { RouteComponentProps, withRouter } from 'react-router';
import { activeTab } from './activeTab';

export interface Tab {
    text?: string;
    id: string;
}

type TabsProps =
    {
        tabs: Tab[];
        default: string;
    }
    & RouteComponentProps<{ tab: string }>;


class Tabs extends React.PureComponent<TabsProps> {

    /**
     *
     */
    constructor(props: TabsProps) {
        super(props);
    }

    private getActiveTabId(): string {
        return activeTab(this.props.location.search) || this.props.default;
    }

    private isTabActive(t: Tab) {
        return this.getActiveTabId() == t.id;
    }

    renderTab(t: Tab) {
        const commonClassNames = "tab col-3"
        const className = this.isTabActive(t) ? `${commonClassNames} active txt-bold` : commonClassNames
        return (
            <div key={t.id} className={className} >
                <Link to={`${this.props.location.pathname}?tab=${t.id}`}>
                    {t.text || t.id}
                </Link>
            </div>
        )
    }

    render() {
        return (
            <React.Fragment>
                <div className="tabs row no-gutters text-sm">
                    {this.props.tabs.map(t => this.renderTab(t))}
                </div>
            </React.Fragment>
        )
    }
}

export default withRouter(Tabs);
