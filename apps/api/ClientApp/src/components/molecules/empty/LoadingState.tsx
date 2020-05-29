import * as React from 'react';
import { Spinner } from 'reactstrap';
import './empty.css';

interface LoadingStateProps {
    isLoading?: boolean | undefined;
}

export class LoadingState extends React.PureComponent<LoadingStateProps> {
    render() {
        return (<React.Fragment>
            <div className="emptystate text-center">
                <div className="loading-spinner">
                    <Spinner color="secondary"/>
                </div>
                <div>
                    {this.props.children}
                </div>
            </div>
        </React.Fragment>)
    }
}