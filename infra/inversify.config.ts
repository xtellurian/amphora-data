import "reflect-metadata"; // only import this once in the whole application
import { Container } from "inversify";
import { IComponentParams, COMPONENTS, COMPONENT_PARAMS } from "./components";
import { IMonitoring, Monitoring, MonitoringParams } from "./components/monitoring/monitoring";
import { IApplication, Application, ApplicationParams } from "./components/application/application";
import { IState, State, StateParams } from "./components/state/state";

const amphoraContainer = new Container({ skipBaseClassChecks: true });
// load the components
amphoraContainer.bind<IMonitoring>(COMPONENTS.Monitoring).to(Monitoring).inSingletonScope();
amphoraContainer.bind<IState>(COMPONENTS.State).to(State).inSingletonScope();
amphoraContainer.bind<IApplication>(COMPONENTS.Application).to(Application);

// load the parameters for each component
amphoraContainer.bind<IComponentParams>(COMPONENT_PARAMS.MonitoringParams).to(MonitoringParams);
amphoraContainer.bind<IComponentParams>(COMPONENT_PARAMS.StateParams).to(StateParams);
amphoraContainer.bind<IComponentParams>(COMPONENT_PARAMS.ApplicationParams).to(ApplicationParams);

export { amphoraContainer };
