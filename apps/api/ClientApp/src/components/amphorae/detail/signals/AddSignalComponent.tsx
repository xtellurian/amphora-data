import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { Link } from "react-router-dom";
import { OneAmphora } from "../props";
import { SecondaryButton, PrimaryButton } from "../../../molecules/buttons";
import { TextInput, Radio } from "../../../molecules/inputs";
import { InfoPane } from "../../../molecules/info/InfoPane";
import { CreateSignal } from "amphoradata";
import { RouteComponentProps, withRouter } from "react-router";

const ValueTypeOptions = [
    {
        text: "Numeric",
        value: "Numeric",
    },
    {
        text: "String",
        value: "String",
    },
];

interface AddSignalState {
    signal: CreateSignal;
}

const AddSignalComponent: React.FunctionComponent<
    OneAmphora & RouteComponentProps
> = (props) => {
    const [state, setState] = React.useState<AddSignalState>({
        signal: {
            valueType: "Numeric", // default to numeric
        },
    });

    const clients = useAmphoraClients();

    const createSignal = (amphoraId: string) => {
        clients.amphoraeApi
            .amphoraeSignalsCreateSignal(amphoraId, state.signal)
            .then((s) => props.history.goBack())
            .catch((e) => alert("error creating signal"));
    };
    const setProperty = (name?: string) => {
        const signal = state.signal;
        signal.property = name;
        setState({ signal });
    };
    const setValueType = (valueType: string) => {
        const signal = state.signal;
        signal.valueType = valueType;
        setState({ signal });
    };

    return (
        <React.Fragment>
            <div className="row justify content-around">
                <div className="col txt-lg">Add Signal</div>
                <div className="col text-right">
                    <Link to={`/amphora/detail/${props.amphora.id}/signals`}>
                        <SecondaryButton variant="slim">Back</SecondaryButton>
                    </Link>
                </div>
            </div>
            <hr />
            <div className="row justify-content-around">
                <div className="col-6">
                    <small>
                        Start adding a new signal by giving it a property name.
                    </small>
                    <TextInput
                        label="Property Name"
                        onComplete={(p) => setProperty(p)}
                        helpText={(v) => "Lowercase alpha, 3-20 characters"}
                    />
                    <Radio
                        defaultValue="Numeric"
                        name="signalType"
                        label="Value Type"
                        options={ValueTypeOptions}
                        onChange={(v) => setValueType(v)}
                    />
                </div>
                <div className="col-4 align-items-right">
                    <InfoPane title="What is a signal?">
                        A signal represents time series data stored in an
                        Amphora.
                    </InfoPane>
                </div>
            </div>
            <hr />
            <div className="text-right">
                <Link to={`/amphora/detail/${props.amphora.id}/signals`}>
                    <SecondaryButton>Cancel</SecondaryButton>
                </Link>
                <PrimaryButton onClick={(e) => createSignal(props.amphora.id || '')}>
                    Save
                </PrimaryButton>
            </div>
        </React.Fragment>
    );
};

export default withRouter(AddSignalComponent);
