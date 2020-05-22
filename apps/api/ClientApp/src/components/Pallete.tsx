import * as React from 'react';
import { PrimaryButton, SecondaryButton } from './molecules/buttons';
import { TextInput } from './molecules/inputs';
import { ValidateResult } from './molecules/inputs/inputProps';
import {} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';


export default class Pallete extends React.PureComponent {

  public validateText(t?: string): ValidateResult {
    if (t && t.length < 10) {
      return {
        isValid: false,
        message: "Text must be more than 10 chars."
      }
    } else {
      return { isValid: true }
    }
  }
  public render() {
    return (
      <React.Fragment>
        <h2>Icons</h2>
        <FontAwesomeIcon icon="times-circle" />
        <hr/>
      
        <h2>Buttons</h2>
        <PrimaryButton onClick={(e) => alert("Primary Button was clicked")}>
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
        <hr/>
        <h2> Inputs </h2>
        <TextInput 
          label="An Input Label" 
          helpText={(v) => "Some support text"} 
          onComplete={(v) => alert("Input completed: " + v)} 
          validator={(v) => this.validateText(v)} />

      </React.Fragment>
    );
  }
}