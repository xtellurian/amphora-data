export function activeTab(queryString: string, name = "tab"): string | null {
    return new URLSearchParams(queryString).get(name);
}
