import * as listActions from "../actions/amphora/list";
import * as signalsActions from "../actions/signals/fetch";
import { AmphoraState } from "../state/amphora";
import { Reducer, Action } from "redux";
import { DetailedAmphora, Signal } from "amphoradata";
import { emptyCache } from "../state/common";

export const reducer: Reducer<AmphoraState> = (
  state: AmphoraState | undefined,
  incomingAction: Action
): AmphoraState => {
  if (state === undefined) {
    return {
      collections: {
        self: {
          created: [],
          purchased: [],
        },
        organisation: {
          created: [],
          purchased: [],
        },
      },
      isLoading: false,
      metadata: emptyCache<DetailedAmphora>(),
      signals: emptyCache<Signal[]>(),
    };
  }

  switch (incomingAction.type) {
    case listActions.LIST_AMPHORAE:
      return {
        isLoading: true,
        metadata: state.metadata,
        collections: state.collections,
        signals: state.signals,
      };

    case listActions.LIST_AMPHORAE_SUCCESS:
      const recieveAmphoraAction = incomingAction as listActions.RecieveAmphoraListAction;
      const allAmphora = recieveAmphoraAction.payload;
      const metadata = state.metadata || emptyCache<DetailedAmphora>();
      // always update the cache
      allAmphora.forEach((a) => {
        if (a.id) {
          metadata.store[a.id] = a;
        }
      });
      if (recieveAmphoraAction.payload) {
        switch (recieveAmphoraAction.scope) {
          case "self":
            console.log(
              `adding to self, accessType: ${recieveAmphoraAction.accessType}`
            );
            if (recieveAmphoraAction.accessType === "created") {
              return {
                isLoading: false,
                collections: {
                  organisation: state.collections
                    ? state.collections.self
                    : { created: [], purchased: [] },
                  self: {
                    created: recieveAmphoraAction.payload
                      .map((a) => a.id)
                      .filter((i) => i) as string[],
                    purchased:
                      state.collections && state.collections.self
                        ? state.collections.self.purchased
                        : [],
                  },
                },
                metadata,
                signals: state.signals,
              };
            } else if (recieveAmphoraAction.accessType === "purchased") {
              return {
                isLoading: false,
                collections: {
                  organisation: state.collections
                    ? state.collections.self
                    : { created: [], purchased: [] },
                  self: {
                    created:
                      state.collections && state.collections.self
                        ? state.collections.self.created
                        : [],
                    purchased: recieveAmphoraAction.payload
                      .map((a) => a.id)
                      .filter((i) => i) as string[],
                  },
                },
                metadata,
                signals: state.signals,
              };
            } else {
              return state;
            }
          case "organisation":
            if (recieveAmphoraAction.accessType === "created") {
              return {
                isLoading: false,
                collections: {
                  self: state.collections
                    ? state.collections.organisation
                    : { created: [], purchased: [] },
                  organisation: {
                    created: recieveAmphoraAction.payload
                      .map((a) => a.id)
                      .filter((i) => i) as string[],
                    purchased:
                      state.collections && state.collections.organisation
                        ? state.collections.organisation.purchased
                        : [],
                  },
                },
                metadata,
                signals: state.signals,
              };
            } else if (recieveAmphoraAction.accessType === "purchased") {
              return {
                isLoading: false,
                collections: {
                  self: state.collections
                    ? state.collections.organisation
                    : { created: [], purchased: [] },
                  organisation: {
                    created:
                      state.collections && state.collections.organisation
                        ? state.collections.organisation.created
                        : [],
                    purchased: recieveAmphoraAction.payload
                      .map((a) => a.id)
                      .filter((i) => i) as string[],
                  },
                },
                metadata,
                signals: state.signals,
              };
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
              purchased: [],
            },
          },
          metadata: emptyCache<DetailedAmphora>(),
          signals: state.signals,
        };
      }

    case signalsActions.FETCH_SIGNALS_SUCCESS:
        const fetchSignalsAction = incomingAction as signalsActions.FetchSignalsSuccessAction;
        const signals = state.signals || emptyCache<Signal[]>();
        signals.store[fetchSignalsAction.amphoraId] = fetchSignalsAction.payload;
        signals.lastUpdated = new Date();
        return {
            collections: state.collections,
            isLoading: false,
            metadata: state.metadata,
            signals: signals
        }
      break;

    default:
      return state;
  }
};
