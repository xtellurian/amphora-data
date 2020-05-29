import * as listActions from "../actions/amphora/list";
import { AmphoraState } from "../state/amphora";
import { Reducer, Action } from "redux";
import { DetailedAmphora } from "amphoradata";
import { emptyCache } from "../state/common";

export const reducer: Reducer<AmphoraState> = (state: AmphoraState | undefined, incomingAction: Action): AmphoraState => {

    if (state === undefined) {
        return {
            collections: {
                self: {
                    created: [],
                    purchased: []
                },
                organisation: {
                    created: [],
                    purchased: []
                }
            },
            isLoading: false,
            cache: {}
        };
    }

    switch (incomingAction.type) {

        case listActions.LIST_AMPHORAE:
            return {
                isLoading: true,
                cache: state.cache,
                collections: state.collections
            }

        case listActions.LIST_AMPHORAE_SUCCESS:
            const recieveAmphoraAction = incomingAction as listActions.RecieveAmphoraListAction;
            const allAmphora = recieveAmphoraAction.payload;
            const cache = state.cache || emptyCache<DetailedAmphora>();
            // always update the cache
            allAmphora.forEach((a) => {
                if (a.id) {
                    cache[a.id] = a;
                }
            });
            if (recieveAmphoraAction.payload) {
                switch (recieveAmphoraAction.scope) {
                    case "self":
                        console.log(`adding to self, accessType: ${recieveAmphoraAction.accessType}`);
                        if (recieveAmphoraAction.accessType === "created") {
                            return {
                                isLoading: false,
                                collections: {
                                    organisation: state.collections ? state.collections.self : { created: [], purchased: [] },
                                    self: {
                                        created: recieveAmphoraAction.payload.map(a => a.id).filter(i => i) as string[],
                                        purchased: state.collections && state.collections.self ? state.collections.self.purchased : []
                                    }
                                },
                                cache
                            }
                        } else if (recieveAmphoraAction.accessType === "purchased") {
                            return {
                                isLoading: false,
                                collections: {
                                    organisation: state.collections ? state.collections.self : { created: [], purchased: [] },
                                    self: {
                                        created: state.collections && state.collections.self ? state.collections.self.created : [],
                                        purchased: recieveAmphoraAction.payload.map(a => a.id).filter(i => i) as string[],
                                    }
                                },
                                cache
                            }
                        } else {
                            return state;
                        }
                    case "organisation":
                        if (recieveAmphoraAction.accessType === "created") {
                            return {
                                isLoading: false,
                                collections: {
                                    self: state.collections ? state.collections.organisation : { created: [], purchased: [] },
                                    organisation: {
                                        created: recieveAmphoraAction.payload.map(a => a.id).filter(i => i) as string[],
                                        purchased: state.collections && state.collections.organisation ? state.collections.organisation.purchased : []
                                    }
                                },
                                cache
                            }
                        } else if (recieveAmphoraAction.accessType === "purchased") {
                            return {
                                isLoading: false,
                                collections: {
                                    self: state.collections ? state.collections.organisation : { created: [], purchased: [] },
                                    organisation: {
                                        created: state.collections && state.collections.organisation ? state.collections.organisation.created : [],
                                        purchased: recieveAmphoraAction.payload.map(a => a.id).filter(i => i) as string[],
                                    }
                                },
                                cache
                            }
                        } else {
                            return state;
                        }

                    default:
                        return state;
                }

            } else {
                return {
                    isLoading: false,
                    collections: {
                        self: {
                            created: [],
                            purchased: [],
                        },
                        organisation: {
                            created: [],
                            purchased: []
                        }
                    },
                    cache: {}
                }
            }

        default:
            return state;
    }
}