import * as React from "react";
import { connect } from "react-redux";
import { Link } from "react-router-dom";
import { AmphoraDetailProps, mapStateToProps } from "../props";
import { LoadingState } from "../../../molecules/empty/LoadingState";
import { SecondaryButton, PrimaryButton } from "../../../molecules/buttons";
import { TextInput, Radio } from "../../../molecules/inputs";
import { InfoPane } from "../../../molecules/info/InfoPane";
import { actionCreators } from "../../../../redux/actions/signals/create";
import { Signal } from "amphoradata";

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

type AddSignalProps = typeof actionCreators & AmphoraDetailProps;
interface AddSignalState {
    signal: Signal;
}

class AddSignal extends React.PureComponent<AddSignalProps, AddSignalState> {
    /**
     *
     */
    constructor(props: AddSignalProps) {
        super(props);
        this.state = {
            signal: {
                valueType: "Numeric", // default to numeric
            },
        };
    }
    private createSignal(amphoraId: string) {
        this.props.createSignal(amphoraId, this.state.signal);
        this.props.history.goBack();
    }
    private setProperty(name?: string) {
        const signal = this.state.signal;
        signal.property = name;
        this.setState({ signal });
    }
    private setValueType(valueType: string) {
        const signal = this.state.signal;
        signal.valueType = valueType;
        this.setState({ signal });
    }

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <div className="row justify content-around">
                        <div className="col txt-lg">Add Signal</div>
                        <div className="col text-right">
                            <Link to={`/amphora/detail/${id}/signals`}>
                                <SecondaryButton variant="slim">
                                    Back
                                </SecondaryButton>
                            </Link>
                        </div>
                    </div>
                    <hr />
                    <div className="row justify-content-around">
                        <div className="col-6">
                            <small>
                                Start adding a new signal by giving it a
                                property name.
                            </small>
                            <TextInput
                                label="Property Name"
                                onComplete={(p) => this.setProperty(p)}
                                helpText={(v) =>
                                    "Lowercase alpha, 3-20 characters"
                                }
                            />
                            <Radio
                                defaultValue="Numeric"
                                name="signalType"
                                label="Value Type"
                                options={ValueTypeOptions}
                                onChange={(v) => this.setValueType(v)}
                            />
                        </div>
                        <div className="col-4 align-items-right">
                            <InfoPane title="What is a signal?">
                                A signal represents time series data stored in
                                an Amphora.
                            </InfoPane>
                        </div>
                    </div>
                    <hr />
                    <div className="text-right">
                        <Link to={`/amphora/detail/${id}/signals`}>
                            <SecondaryButton>Cancel</SecondaryButton>
                        </Link>
                        <PrimaryButton onClick={(e) => this.createSignal(id)}>
                            Save
                        </PrimaryButton>
                    </div>
                </React.Fragment>
            );
        } else {
            return <LoadingState />;
        }
    }
}

export default connect(mapStateToProps, actionCreators)(AddSignal as any);
