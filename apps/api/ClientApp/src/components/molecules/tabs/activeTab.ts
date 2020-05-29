import qs from 'qs';

export function activeTab(queryString: string, name = "tab"): string {
    const search = qs.parse(queryString, { ignoreQueryPrefix: true });
    return search[name] as string;
}