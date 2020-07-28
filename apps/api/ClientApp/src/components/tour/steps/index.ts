import { Steps as Collection } from "./collection";
import { Steps as Search } from "./search";
import { Steps as Create } from "./create";
import { Steps as Request } from "./request";

const allSteps = [...Collection, ...Create, ...Search, ...Request];
export default allSteps;
