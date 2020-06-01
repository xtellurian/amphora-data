import * as React from "react";
import { connect } from "react-redux";
import { RouteComponentProps } from "react-router";
import { CreateAmphora as Model } from "amphoradata";
import { PrimaryButton } from "../molecules/buttons";
import { TextInput, TextAreaInput, FloatInput, Dropdown } from "../molecules/inputs";
import { actionCreators as createActions } from "../../redux/actions/amphora/create";
import { actionCreators as listTermsActions } from "../../redux/actions/terms/list";
import { ValidateResult } from "../molecules/inputs/inputProps";
import { TermsOfUseState } from "../../redux/state/terms";
import { ApplicationState } from "../../redux/state";

// At runtime, Redux will merge together...
type CreateAmphoraProps = typeof createActions & // ... plus action creators we've requested
  typeof listTermsActions &
  TermsOfUseState &
  RouteComponentProps<{
    startDateIndex: string;
  }>; // ... plus incoming routing parameters

interface CreateAmphoraComponentState {
  model: Model;
}

class CreateAmphora extends React.PureComponent<
  CreateAmphoraProps,
  CreateAmphoraComponentState
> {
  /**
   *
   */
  constructor(props: CreateAmphoraProps) {
    super(props);
    this.state = {
      model: {
        name: "",
        description: "",
        price: 0,
      },
    };
  }

  componentDidMount() {
    this.ensureTerms();
  }

  private ensureTerms() {
    if (!this.props.lastLoaded) {
      this.props.listTerms();
    } else {
      console.log("not downloading terms");
    }
  }

  private validateDescription(s?: string): ValidateResult {
    if (s && s.length > 62) {
      return {
        isValid: false,
        message: "Description must be less than 620 characters",
      };
    }
    return {
      isValid: true,
    };
  }

  private validateName(name?: string): ValidateResult {
    if (name && name.length > 120) {
      return {
        isValid: false,
        message: "Name must be less than 120 characters",
      };
    }
    return {
      isValid: true,
    };
  }

  private setName(name?: string) {
    const model = this.state.model;
    if (name) {
      model.name = name;
      this.setState({
        model,
      });
    }
  }
  private setPrice(price?: number) {
    const model = this.state.model;
    if (price) {
      model.price = price;
      this.setState({
        model,
      });
    }
  }
  private setDescription(description?: string) {
    const model = this.state.model;
    if (description) {
      model.description = description;
      this.setState({
        model,
      });
    }
  }

  private createAmphora() {
    this.props.createNewAmphora(this.state.model);
  }
  private termOptions() {
    return this.props.termIds.map(t => {
      return {
        value: t,
        text: this.props.cache[t].name
      }
    });
  }

  public render() {
    return (
      <React.Fragment>
        <h1>Create Amphora</h1>
        <h3>Package your data</h3>
        <div className="row">
          <div className="col-8">
            <TextInput
              label="Name"
              placeholder="What data are you packaging?"
              helpText={(value) => ` ${value ? value.length : ""}/120`}
              validator={(value) => this.validateName(value)}
              onComplete={(name) => this.setName(name)}
            />
            <TextAreaInput
              label="Description"
              placeholder="Provide more details"
              helpText={(value) => `${value ? value.length : 0}/620`}
              validator={(s) => this.validateDescription(s)}
              onComplete={(d) => this.setDescription(d)}
            />

            <FloatInput label="Price" onComplete={(p) => this.setPrice(p)} />

            <Dropdown onChange={v => alert(v)} options={this.termOptions()} />

            <PrimaryButton
              onClick={() => this.createAmphora()}
              className="mt-5 w-100"
            >
              Create Amphora
            </PrimaryButton>
          </div>
        </div>
      </React.Fragment>
    );
  }
}

function mapStateToProps(state: ApplicationState) {
  if (state && state.terms) {
    return state.terms;
  } else {
    return {};
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  { ...createActions, ...listTermsActions } // Selects which action creators are merged into the component's props
)(CreateAmphora);
