import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { PrimaryButton, SecondaryButton } from '../molecules/buttons';
import { TextInput, TextAreaInput } from '../molecules/inputs';
import { ApplicationState } from '../../redux/state';
import { AmphoraState } from '../../redux/state/amphora';
// import { actionCreators } from '../../redux/actions/amphorae';
import { actionCreators as listActions } from "../../redux/actions/amphora/list";
import { actionCreators as modalActions } from "../../redux/actions/ui";
import { ValidateResult } from '../molecules/inputs/inputProps';

// At runtime, Redux will merge together...
type CreateAmphoraProps =
  AmphoraState // ... state we've requested from the Redux store
  & (typeof listActions & typeof modalActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class CreateAmphora extends React.PureComponent<CreateAmphoraProps> {

  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.setMyView();
  }

  // This method is called when the route parameters change
  public componentDidUpdate(prevProps: CreateAmphoraProps) {
    // this.ensureDataFetched();
  }

  private validateDescription(s: string): ValidateResult {
    if(s && s.length > 62) {
      return {
        isValid: false,
        message: "Description must be less than 620 characters"
      }
    }
    return {
      isValid: true,
    }
  }

  private validateName(name: string): ValidateResult {
    if(name && name.length > 120) {
      return {
        isValid: false,
        message: "Name must be less than 120 characters"
      }
    }
    return {
      isValid: true,
    }
  }

  public render() {
    return (
      <React.Fragment>
        <h1>Create Amphora</h1>
        <h3>Package your data</h3>
        <div className="row">
          <div className="col-6">
            <TextInput label="Name" 
              placeholder="What data are you packaging?" 
              helpText={(value) => `${value.length}/120`}
              validator={(value) => this.validateName(value)}/>
            <TextAreaInput 
              label="Description" 
              placeholder="Provide more details" 
              helpText={(value) => "620 characters"}
              validator={(s) => this.validateDescription(s)} />
          </div>
        </div>
      </React.Fragment>
    );
  }

  private setMyView(): void {
    this.props.listMyCreatedAmphora();
  }
}

function mapStateToProps(state: ApplicationState): AmphoraState {
  if (state.amphora) {
    return state.amphora;
  } else {
    return {
      isLoading: true,
      list: [],
      cache: {}
    }
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  { ...listActions, ...modalActions } // Selects which action creators are merged into the component's props
)(CreateAmphora);
