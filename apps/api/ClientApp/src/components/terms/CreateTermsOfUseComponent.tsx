import * as React from "react";
import { TermsOfUseContext } from "react-amphora";
import { TextInput, TextAreaInput } from "../molecules/inputs";
import { PrimaryButton } from "../molecules/buttons";
import { CreateTermsOfUse } from "amphoradata";
import { ValidateResult } from "../molecules/inputs/inputProps";
import { useHistory } from "react-router";
const buttonDivStyle: React.CSSProperties = {
    width: "100%",
    position: "absolute",
    right: "5px",
    bottom: "5px",
    textAlign: "right",
};

export const CreateTermsOfUseComponent: React.FunctionComponent = (props) => {
    const actions = TermsOfUseContext.useTermsDispatch()
    const history = useHistory();
    const [model, setModel] = React.useState<CreateTermsOfUse>({
        name: "",
        contents: "",
    });
    const complete = () => {
        actions.dispatch({type: 'create-terms', payload: {
            model: model
        }})
        history.replace("/terms");
    };

    const setName = (name?: string) => {
        setModel({
            name: name || '',
            contents: model.contents,
        });
    };
    const setContents = (contents?: string) => {
        setModel({
            name: model.name,
            contents: contents || '',
        });
    };
    const validateName = (name?: string): ValidateResult => {
        if (name && name.length > 5) {
            return {
                isValid: true,
            };
        } else {
            return {
                isValid: false,
                message: "Name must be greater than 5 characters",
            };
        }
    };
    const validateContents = (contents?: string): ValidateResult => {
        if (contents && contents.length > 20) {
            return {
                isValid: true,
            };
        } else {
            return {
                isValid: false,
                message: "Contens must be greater than 20 characters",
            };
        }
    };

    const saveDisabled =
        !validateName(model.name).isValid ||
        !validateContents(model.contents).isValid;

    return (
        <React.Fragment>
            <div className="m-3">
                <div className="txt-lg">Add Terms of Use</div>
                <hr />
                <div>
                    <TextInput
                        className="mb-4"
                        label="Title"
                        placeholder="Enter a name for your new Terms of Use"
                        validator={validateName}
                        onComplete={(n) => setName(n)}
                    ></TextInput>
                    <TextAreaInput
                        label="Details"
                        placeholder="Write or copy and paste the Terms of Use"
                        helpText={(v) => "Markdown Supported"}
                        rows={8}
                        validator={validateContents}
                        onComplete={(c) => setContents(c)}
                    ></TextAreaInput>
                </div>
                <div style={buttonDivStyle}>
                    <PrimaryButton
                        disabled={saveDisabled}
                        className="w-100"
                        onClick={(e) => complete()}
                    >
                        Save
                    </PrimaryButton>
                </div>
            </div>
        </React.Fragment>
    );
};
