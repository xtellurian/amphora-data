import * as React from "react";
import { connect } from "react-redux";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../../redux/state";
import { TermsOfUseState } from "../../redux/state/terms";
import { actionCreators as createActions } from "../../redux/actions/terms/create";

import { PrimaryButton } from "../molecules/buttons";
import { emptyCache } from "../../redux/state/common";
import { TermsOfUse, CreateTermsOfUse } from "amphoradata";
import { ModalWrapper } from "../molecules/modal/ModalWrapper";
import { TextInput, TextAreaInput } from "../molecules/inputs";
import { ValidateResult } from "../molecules/inputs/inputProps";

const buttonDivStyle: React.CSSProperties = {
  width: "100%",
  position: "absolute",
  right: "5px",
  bottom: "5px",
  textAlign: "right",
};
// At runtime, Redux will merge together...
type CreateTermsOfUseComponentProps = TermsOfUseState &
  typeof createActions &
  RouteComponentProps<{}>; // ... plus incoming routing parameters

interface CreateTermsComponentState {
  terms: CreateTermsOfUse;
  isOpen: boolean;
}

class CreateTermsOfUseComponent extends React.Component<
  CreateTermsOfUseComponentProps,
  CreateTermsComponentState
> {
  /**
   *
   */
  constructor(props: CreateTermsOfUseComponentProps) {
    super(props);
    this.state = {
      isOpen: true,
      terms: {
        name: "",
        contents: "",
      },
    };
  }

  private setName(name?: string) {
    this.setState({
      terms: {
        name: name || "",
        contents: this.state.terms.contents,
      },
    });
  }
  private validateName(name?: string): ValidateResult {
    if (name && name.length > 5) {
      return {
        isValid: true,
      };
    } else {
      return {
        isValid: false,
        message: "Name must have more than 5 characters.",
      };
    }
  }
  private setContents(contents?: string) {
    this.setState({
      terms: {
        name: this.state.terms.name,
        contents: contents || "",
      },
    });
  }
  private validateContents(contents?: string): ValidateResult {
    if (contents && contents.length > 20) {
      return {
        isValid: true,
      };
    } else {
      return {
        isValid: false,
        message: "Contents must have more than 20 characters.",
      };
    }
  }
  private isCreateDisabled(): boolean {
    if (!this.state.terms) {
      return false;
    }
    return (
      !this.validateName(this.state.terms.name).isValid ||
      !this.validateContents(this.state.terms.contents).isValid
    );
  }

  private completeModal() {
    this.props.createNewTerms(this.state.terms);
    this.setState({ isOpen: false });
  }

  // rendering methods
  public render() {
    console.log("rerendering the thing");
    console.log(this.state.isOpen);
    return (
      <React.Fragment>
        <ModalWrapper isOpen={this.state.isOpen} onCloseRedirectTo="/terms">
          <div className="m-3">
            <div className="txt-lg">Add Terms of Use</div>
            <hr />
            <div>
              <TextInput
                className="mb-4"
                label="Title"
                placeholder="Enter a name for your new Terms of Use"
                validator={this.validateName}
                onComplete={(n) => this.setName(n)}
              ></TextInput>
              <TextAreaInput
                label="Details"
                placeholder="Write or copy and paste the Terms of Use"
                helpText={(v) => "Markdown Supported"}
                rows={8}
                validator={this.validateContents}
                onComplete={(c) => this.setContents(c)}
              ></TextAreaInput>
            </div>
            <div style={buttonDivStyle}>
              <PrimaryButton
                disabled={this.isCreateDisabled()}
                className="w-100"
                onClick={(e) => this.completeModal()}
              >
                Save
              </PrimaryButton>
            </div>
          </div>
        </ModalWrapper>
      </React.Fragment>
    );
  }
}

function mapStateToProps(state: ApplicationState): TermsOfUseState {
  if (state.terms) {
    return state.terms;
  } else {
    return {
      cache: emptyCache<TermsOfUse>(),
      isLoading: true,
      termIds: [],
    };
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  createActions // Selects which action creators are merged into the component's props
)(CreateTermsOfUseComponent);
