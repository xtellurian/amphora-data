import { PermissionsState } from "../redux/state/permissions";

interface I {
    state: PermissionsState;
    amphoraId: string;
}
function canPurchase({ state, amphoraId }: I): boolean {
    if (state.purchase.lastUpdated) {
        return state.purchase.store[amphoraId];
    } else {
        return false;
    }
}
function canReadContents({ state, amphoraId }: I): boolean {
    if (state.readContents.lastUpdated) {
        return state.readContents.store[amphoraId];
    } else {
        return false;
    }
}
function canWriteContents({ state, amphoraId }: I): boolean {
    if (state.writeContents.lastUpdated) {
        return state.writeContents.store[amphoraId];
    } else {
        return false;
    }
}

export function getPermissions(state: PermissionsState, amphoraId: string) {
    const canWrite = canWriteContents({ state, amphoraId });
    const canRead = canWrite ? canWrite : canReadContents({ state, amphoraId });
    const canPurch = canWrite ? canRead : canPurchase({ state, amphoraId });

    return {
        canPurchase: canPurch,
        canReadContents: canRead,
        canWriteContents: canWrite,
    };
}
