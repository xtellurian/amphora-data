import * as React from 'react';
import ConnectedAmphoraModal from './ConnectedAmphoraModal';
import { DetailedAmphora } from 'amphoradata';
import { Button } from 'reactstrap';


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
                    {this.props.amphora.name}
                    {this.props.amphora.price}
                    <Button color="primary" onClick={() => this.openModal()}>Show</Button>
                    <ConnectedAmphoraModal />
                </div>
            );
        }
        else {
            return <div>None</div>
        }
    }
}
