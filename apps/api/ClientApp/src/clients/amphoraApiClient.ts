import * as amphoradata from 'amphoradata';
import axios from 'axios';

const configuration = new amphoradata.Configuration({ basePath: "." });
export const amphoraApiClient = new amphoradata.AmphoraeApi(configuration);
export const termsOfUseApiClient = new amphoradata.TermsOfUseApi(configuration);
export const usersClient = new amphoradata.UsersApi(configuration);
export const searchClient = new amphoradata.SearchApi(configuration);

function setDates(e: any | undefined) {
    if (e && e.createdDate) {
        e.createdDate = new Date(e.createdDate);
    }
    return e;
}

axios.interceptors.response.use((response) => {
    if (Array.isArray(response.data)) {
        response.data = response.data.map((a) => setDates(a))
    } else {
        response.data = setDates(response.data)
    }
    return response;
})