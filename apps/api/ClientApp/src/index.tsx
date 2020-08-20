import "bootstrap/dist/css/bootstrap.css";

import * as React from "react";
import * as ReactDOM from "react-dom";
import * as amphoradata from "amphoradata";
import { AmphoraProvider } from "react-amphora";
import userManager from "./userManager";
import { Provider } from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import { createBrowserHistory } from "history";
import configureStore from "./redux/configureStore";
import App from "./App";
import registerServiceWorker from "./registerServiceWorker";
import { getHostUrl } from "./utilities";
import { toastOnActionResult } from "./utilities/toasts";
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

ReactDOM.render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <AmphoraProvider
                userManager={userManager}
                configuration={
                    new amphoradata.Configuration({ basePath: host })
                }
                onActionResult={(r) => toastOnActionResult(r, store.getState().settings as Settings)}
            >
                <App />
            </AmphoraProvider>
        </ConnectedRouter>
    </Provider>,
    document.getElementById("root")
);

registerServiceWorker();
