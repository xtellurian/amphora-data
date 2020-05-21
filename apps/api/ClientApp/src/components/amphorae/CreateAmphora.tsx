import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { CreateAmphora as Model } from 'amphoradata';
import { PrimaryButton } from '../molecules/buttons';
import { TextInput, TextAreaInput } from '../molecules/inputs';
import { ApplicationState } from '../../redux/state';
import { AmphoraState } from '../../redux/state/amphora';
import { actionCreators as createActions } from "../../redux/actions/amphora/create";
import { ValidateResult } from '../molecules/inputs/inputProps';

// At runtime, Redux will merge together...
type CreateAmphoraProps =
  AmphoraState // ... state we've requested from the Redux store
  & (typeof createActions)  // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters

interface CreateAmphoraComponentState {
  model: Model;
}

class CreateAmphora extends React.PureComponent<CreateAmphoraProps, CreateAmphoraComponentState> {

  /**
   *
   */
  constructor(props: CreateAmphoraProps) {
    super(props);
    this.state = {
      model: {
        name: "",
        description: "",
        price: 0
      }
    };
  }

  private validateDescription(s: string): ValidateResult {
    if (s && s.length > 62) {
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
    if (name && name.length > 120) {
      return {
        isValid: false,
        message: "Name must be less than 120 characters"
      }
    }
    return {
      isValid: true,
    }
  }

  private setName(name: string) {
    const model = this.state.model;
    model.name = name;
    this.setState({
      model
    });
  }
  private setPrice(price: number) {
    const model = this.state.model;
    model.price = price;
    this.setState({
      model
    });
  }
  private setDescription(description: string) {
    const model = this.state.model;
    model.description = description;
    this.setState({
      model
    });
  }

  private createAmphora() {
    this.props.createNewAmphora(this.state.model);
  }

  public render() {
    return (
      <React.Fragment>
        <h1>Create Amphora</h1>
        <h3>Package your data</h3>
        <div className="row">
          <div className="col-8">
            <TextInput label="Name"
              placeholder="What data are you packaging?"
              helpText={(value) => `${value.length}/120`}
              validator={(value) => this.validateName(value)}
              onComplete={(name) => this.setName(name)} />
            <TextAreaInput
              label="Description"
              placeholder="Provide more details"
              helpText={(value) => `${value.length}/620`}
              validator={(s) => this.validateDescription(s)}
              onComplete={(d) => this.setDescription(d)} />

            <PrimaryButton onClick={() => this.createAmphora()} className="w-100">
              Create Amphora
              </PrimaryButton>
          </div>
        </div>
      </React.Fragment>
    );
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
  { ...createActions } // Selects which action creators are merged into the component's props
)(CreateAmphora);
