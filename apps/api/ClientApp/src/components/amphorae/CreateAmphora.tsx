import * as React from "react";
import { TermsOfUseContext, AmphoraOperationsContext } from "react-amphora";
import { CreateAmphora as Model, Result, Position } from "amphoradata";
import { PrimaryButton, SecondaryButton } from "../molecules/buttons";
import {
    TextInput,
    TextAreaInput,
    FloatInput,
    Dropdown,
} from "../molecules/inputs";
import { GeoLookupComponent } from "react-amphora";
import { ValidateResult } from "../molecules/inputs/inputProps";
import { LoadingState } from "../molecules/empty/LoadingState";

const TERMS_DEFAULT = "1";

interface CreateAmphoraComponentState {
    model: Model;
    freetextAddress?: string | null | undefined;
    manualPositionEntry: boolean;
}

export const CreateAmphoraPage: React.FunctionComponent = (props) => {
    const termsContext = TermsOfUseContext.useTermsState();
    const termsDispatch = TermsOfUseContext.useTermsDispatch();

    const actions = AmphoraOperationsContext.useAmphoraOperationsDispatch();

    const [attempts, setAttempts] = React.useState(0)
    // ensure terms are fetched
    React.useEffect(() => {
        if (termsContext.isAuthenticated && attempts < 1) {
            termsDispatch.dispatch({ type: "fetch-terms" });
            setAttempts(attempts + 1)
        }
    }, [termsContext.isAuthenticated, termsDispatch, attempts, setAttempts]);

    const [state, setState] = React.useState<CreateAmphoraComponentState>({
        model: {
            name: "",
            price: 0,
            description: "",
        },
        manualPositionEntry: false,
    });

    const validateDescription = (s?: string): ValidateResult => {
        if (s && s.length > 620) {
            return {
                isValid: false,
                message: "Description must be less than 620 characters",
            };
        }
        return {
            isValid: true,
        };
    };

    const validateName = (name?: string): ValidateResult => {
        if (name && name.length > 120) {
            return {
                isValid: false,
                message: "Name must be less than 120 characters",
            };
        }
        return {
            isValid: true,
        };
    };

    const setName = (name?: string) => {
        const model = state.model;
        if (name) {
            model.name = name;
            setState({
                ...state,
                model,
            });
        }
    };
    const setPrice = (price?: number) => {
        const model = state.model;
        if (price) {
            model.price = price;
            setState({
                model,
                manualPositionEntry: state.manualPositionEntry,
                freetextAddress: state.freetextAddress,
            });
        }
    };
    const setDescription = (description?: string) => {
        const model = state.model;
        if (description) {
            model.description = description;
            setState({
                model,
                manualPositionEntry: state.manualPositionEntry,
                freetextAddress: state.freetextAddress,
            });
        }
    };

    const createAmphora = () => {
        actions.dispatch({
            type: "amphora-operation-create",
            payload: {
                model: state.model,
            },
        });
    };
    const setTerms = (t: string) => {
        const model = state.model;
        if (model.termsOfUseId !== t) {
            model.termsOfUseId = t === TERMS_DEFAULT ? null : t;
            setState({
                model,
                manualPositionEntry: state.manualPositionEntry,
                freetextAddress: state.freetextAddress,
            });
        }
    };
    const setPosition = (position?: Position | null | undefined) => {
        const model = state.model;
        if (position) {
            model.lat = position.lat;
            model.lon = position.lon;
            setState({ ...state, model });
        }
    };
    const setLat = (lat?: number) => {
        const model = state.model;
        model.lat = lat;
        if (lat === undefined) {
            model.lon = null;
        }
        setState({ ...state, model });
    };
    const setLon = (lon?: number) => {
        const model = state.model;
        model.lon = lon;
        if (lon === undefined) {
            model.lat = null;
        }
        setState({ ...state, model });
    };

    const termsOptions = termsContext.results.map((t) => {
        return { value: t.id || "", text: t.name || "" };
    });

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
                        validator={(value) => validateName(value)}
                        onComplete={(name) => setName(name)}
                    />
                    <TextAreaInput
                        label="Description"
                        placeholder="Provide more details"
                        helpText={(value) => `${value ? value.length : 0}/620`}
                        validator={(s) => validateDescription(s)}
                        onComplete={(d) => setDescription(d)}
                    />

                    <FloatInput label="Price" onComplete={(p) => setPrice(p)} />
                    <div
                        className={`row ${
                            state.manualPositionEntry && "d-none"
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
                                loadingPlaceholder={<LoadingState />}
                                onResultSelected={(r: Result) => {
                                    setPosition(r.position);
                                    setState({
                                        model: state.model,
                                        manualPositionEntry:
                                            state.manualPositionEntry,
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
                            state.manualPositionEntry && "d-none"
                        }`}
                    >
                        <div className="col">
                            {state.freetextAddress}
                            {state.model.lat &&
                                state.model.lon &&
                                `  (${state.model.lat}, ${state.model.lon})`}
                        </div>
                    </div>
                    <div
                        className={`row ${
                            !state.manualPositionEntry && "d-none"
                        }`}
                    >
                        <div className="col">
                            <FloatInput
                                className="w-50"
                                label="Latitude"
                                value={state.model.lat || 0}
                                onComplete={(p) => setLat(p)}
                            />
                        </div>
                        <div className="col">
                            <FloatInput
                                label="Longitude"
                                value={state.model.lon || 0}
                                onComplete={(p) => setLon(p)}
                            />
                        </div>
                    </div>
                    <SecondaryButton
                        onClick={(e) =>
                            setState({
                                model: state.model,
                                freetextAddress: state.freetextAddress,
                                manualPositionEntry: !state.manualPositionEntry,
                            })
                        }
                    >
                        {state.manualPositionEntry
                            ? "Search Locations"
                            : "Enter Location Manually"}
                    </SecondaryButton>

                    <Dropdown
                        label="Terms of Use"
                        onChange={(v) => setTerms(v)}
                        options={termsOptions}
                    />

                    <PrimaryButton
                        onClick={() => createAmphora()}
                        className="mt-5 w-100"
                    >
                        Create Amphora
                    </PrimaryButton>
                </div>
            </div>
        </React.Fragment>
    );
};
