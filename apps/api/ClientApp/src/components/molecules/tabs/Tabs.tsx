import * as React from 'react';

import './tabs.css';

interface TabProp {
    isActive?: boolean;
    text: string;
    onClick: () => void;
}

interface TabsProps {
    tabs: TabProp[];
}
export class Tabs extends React.PureComponent<TabsProps> {

    renderTab(t: TabProp) {
        const className = t.isActive ? "tab col-3 active txt-bold" : "tab col-3"
        return (
            <div key={t.text} className={className} onClick={t.onClick}>
                {t.text}
            </div>
        )
    }

    render() {

        return (
            <div className="tabs row text-sm">
                {this.props.tabs.map(t => this.renderTab(t))}
            </div>
        )
    }
}