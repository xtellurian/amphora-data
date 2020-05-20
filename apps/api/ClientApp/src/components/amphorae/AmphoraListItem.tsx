import * as React from 'react';
import { DetailedAmphora } from 'amphoradata';
import { Link } from 'react-router-dom';
import { SecondaryButton } from '../molecules/buttons'


interface AmphoraeListItemProps {
    amphora: DetailedAmphora;
    openModal(amphora: DetailedAmphora): void;
}


export class AmphoraListItem extends React.PureComponent<AmphoraeListItemProps> {

    private openModal = () => {
        console.log("Opening Modal for amphora", this.props.amphora);
        this.props.openModal(this.props.amphora);
    }


    public render() {
        if (this.props.amphora) {
            return (
                <div>
                    {this.props.amphora.name} :: 
                    {this.props.amphora.price}
                    <Link to={`/amphora/detail/${this.props.amphora.id}`}>
                        <SecondaryButton>
                            View  
                        </SecondaryButton>
                    </Link>
                </div>
            );
        }
        else {
            return <div>None</div>
        }
    }
}
