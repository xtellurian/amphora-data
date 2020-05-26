import * as React from 'react';
import { ToastContainer } from 'react-toastify';
import './toasts.css';

export class Toaster extends React.PureComponent {
    public render() {
        return (
            <ToastContainer
                position="top-right"
                autoClose={false}
                hideProgressBar={true}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable={false}
                pauseOnHover
            />
        )
    }
}