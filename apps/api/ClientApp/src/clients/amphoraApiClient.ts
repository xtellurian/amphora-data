import axios from 'axios';

export const axiosClient = axios;
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