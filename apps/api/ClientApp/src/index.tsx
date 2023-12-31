import "bootstrap/dist/css/bootstrap.css";

import * as React from "react";
import { render } from "react-dom";
import { Configuration } from "amphoradata";
import { AmphoraProvider } from "react-amphora";
import userManager from "./userManager";
import { Provider } from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import { createBrowserHistory } from "history";
import configureStore from "./redux/configureStore";
import App from "./App";
import registerServiceWorker from "./registerServiceWorker";
import { getHostUrl } from "./utilities";
import { toastOnActionResult, toastOnAction } from "./utilities/toasts";
import Modal from "react-modal";
import { Settings } from "./redux/state/Settings";
// Create browser history to use in the Redux store
const baseUrl = document
    .getElementsByTagName("base")[0]
    .getAttribute("href") as string;
const history = createBrowserHistory({ basename: baseUrl });
const store = configureStore(history);
const host = getHostUrl();
Modal.setAppElement("#root");

render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <AmphoraProvider
                userManager={userManager}
                configuration={new Configuration({ basePath: host })}
                onAction={(a) =>
                    toastOnAction(a, store.getState().settings as Settings)
                }
                onActionResult={(r) =>
                    toastOnActionResult(
                        r,
                        store.getState().settings as Settings
                    )
                }
            >
                <App />
            </AmphoraProvider>
        </ConnectedRouter>
    </Provider>,
    document.getElementById("root")
);

registerServiceWorker();
