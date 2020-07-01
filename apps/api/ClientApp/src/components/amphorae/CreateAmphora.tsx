import * as React from "react";
import { connect } from "react-redux";
import { RouteComponentProps } from "react-router";
import { CreateAmphora as Model, Result, Position } from "amphoradata";
import { PrimaryButton, SecondaryButton } from "../molecules/buttons";
import {
    TextInput,
    TextAreaInput,
    FloatInput,
    Dropdown,
} from "../molecules/inputs";
import { GeoLookupComponent } from "react-amphora";
import { actionCreators as createActions } from "../../redux/actions/amphora/create";
import { actionCreators as listTermsActions } from "../../redux/actions/terms/list";
import { ValidateResult } from "../molecules/inputs/inputProps";
import { TermsOfUseState } from "../../redux/state/terms";
import { ApplicationState } from "../../redux/state";
import { LoadingState } from "../molecules/empty/LoadingState";

const TERMS_DEFAULT = "1";

// At runtime, Redux will merge together...
type CreateAmphoraProps = typeof createActions & // ... plus action creators we've requested
    typeof listTermsActions &
    TermsOfUseState &
    RouteComponentProps<{
        startDateIndex: string;
    }>; // ... plus incoming routing parameters

interface CreateAmphoraComponentState {
    model: Model;
    freetextAddress?: string | null | undefined;
    manualPositionEntry: boolean;
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
            manualPositionEntry: false,
        };
    }

    componentDidMount() {
        this.ensureTerms();
    }

    private ensureTerms() {
        if (!this.props.cache.lastUpdated) {
            this.props.listTerms();
        } else {
            console.log("not downloading terms");
        }
    }

    private validateDescription(s?: string): ValidateResult {
        if (s && s.length > 620) {
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
        return [
            { value: TERMS_DEFAULT, text: "None" },
            ...this.props.termIds.map((t) => {
                return {
                    value: t,
                    text: this.props.cache.store[t].name,
                };
            }),
        ];
    }
    private setTerms(t: string) {
        const model = this.state.model;
        if (model.termsOfUseId !== t) {
            model.termsOfUseId = t === TERMS_DEFAULT ? null : t;
            this.setState({ model });
        }
    }
    private setPosition(position?: Position | null | undefined) {
        const model = this.state.model;
        if (position) {
            model.lat = position.lat;
            model.lon = position.lon;
            this.setState({ model });
        }
    }
    private setLat(lat?: number) {
        const model = this.state.model;
        model.lat = lat;
        if (lat === undefined) {
            model.lon = null;
        }
        this.setState({ model });
    }
    private setLon(lon?: number) {
        const model = this.state.model;
        model.lon = lon;
        if (lon === undefined) {
            model.lat = null;
        }
        this.setState({ model });
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
                            helpText={(value) =>
                                ` ${value ? value.length : ""}/120`
                            }
                            validator={(value) => this.validateName(value)}
                            onComplete={(name) => this.setName(name)}
                        />
                        <TextAreaInput
                            label="Description"
                            placeholder="Provide more details"
                            helpText={(value) =>
                                `${value ? value.length : 0}/620`
                            }
                            validator={(s) => this.validateDescription(s)}
                            onComplete={(d) => this.setDescription(d)}
                        />

                        <FloatInput
                            label="Price"
                            onComplete={(p) => this.setPrice(p)}
                        />
                        <div
                            className={`row ${
                                this.state.manualPositionEntry && "d-none"
                            }`}
                        >
                            <div className="col-8">
                                <GeoLookupComponent
                                    heading={
                                        <div>
                                            <strong>Geo Location</strong>
                                        </div>
                                    }
                                    buttonClassName="d-none"
                                    loadingPlaceholder={<LoadingState/>}
                                    onResultSelected={(r: Result) => {
                                        this.setPosition(r.position);
                                        this.setState({
                                            freetextAddress: r.address
                                                ? r.address.freeformAddress
                                                : "",
                                        });
                                    }}
                                />
                            </div>
                        </div>
                        <div
                            className={`row ${
                                this.state.manualPositionEntry && "d-none"
                            }`}
                        >
                            <div className="col">
                                {this.state.freetextAddress}
                                {this.state.model.lat && this.state.model.lon &&
                                (`  (${this.state.model.lat}, ${this.state.model.lon})`)}
                            </div>
                        </div>
                        <div
                            className={`row ${
                                !this.state.manualPositionEntry && "d-none"
                            }`}
                        >
                            <div className="col">
                                <FloatInput
                                    className="w-50"
                                    label="Latitude"
                                    value={this.state.model.lat || 0}
                                    onComplete={(p) => this.setLat(p)}
                                />
                            </div>
                            <div className="col">
                                <FloatInput
                                    label="Longitude"
                                    value={this.state.model.lon || 0}
                                    onComplete={(p) => this.setLon(p)}
                                />
                            </div>
                        </div>
                        <SecondaryButton
                            onClick={(e) =>
                                this.setState({
                                    manualPositionEntry: !this.state
                                        .manualPositionEntry,
                                })
                            }
                        >
                            {this.state.manualPositionEntry
                                ? "Search Locations"
                                : "Enter Location Manually"}
                        </SecondaryButton>

                        <Dropdown
                            label="Terms of Use"
                            onChange={(v) => this.setTerms(v)}
                            options={this.termOptions()}
                        />

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
