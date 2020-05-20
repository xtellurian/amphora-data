import * as React from 'react';
import { DetailedAmphora } from 'amphoradata';
import * as ReactMarkdown from 'react-markdown';

interface AmphoraListItemProps {
    amphora: DetailedAmphora;
}

export class AmphoraDetail extends React.PureComponent<AmphoraListItemProps> {

    // This method is called when the route parameters change
    public componentDidUpdate(prevProps: AmphoraListItemProps) {
        // this.ensureDataFetched();
    }

    public render() {
        return (
            <div>
                <h3>{this.props.amphora.name}</h3>
                <div>
                    Description: 
                    <ReactMarkdown.default>
                    {this.props.amphora.description}
                    </ReactMarkdown.default>
                </div>
                <div>
                    Price: {this.props.amphora.price}
                </div>
            </div>
        );
    }
}


