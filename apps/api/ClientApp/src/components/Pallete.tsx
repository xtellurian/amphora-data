import * as React from 'react';
import { PrimaryButton, SecondaryButton } from './molecules/buttons';


export default class Pallete extends React.PureComponent {

  public render() {
    return (
      <React.Fragment>
        <h1>Buttons</h1>
        <PrimaryButton>
          Primary Button
        </PrimaryButton>
        <PrimaryButton disabled={true}>
          Primary Disabled
        </PrimaryButton>
        <SecondaryButton>
          Secondary Button
        </SecondaryButton>
        <SecondaryButton disabled={true}>
          Secondary Disabled
        </SecondaryButton>

        <h2> Inputs </h2>
        
      </React.Fragment>
    );
  }
}