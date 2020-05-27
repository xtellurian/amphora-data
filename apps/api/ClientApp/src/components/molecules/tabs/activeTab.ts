import qs from 'qs';

export function activeTab(queryString: string): string {
    const search = qs.parse(queryString, { ignoreQueryPrefix: true });
    return search.tab as string;
}