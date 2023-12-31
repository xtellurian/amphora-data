export interface ValidateResult {
    isValid: boolean;
    message?: string;
}

export interface InputProps<T> {
    id?: string; // the id of the outer component
    className?: string;
    label: string;
    value?: T;
    placeholder?: string;
    focusOnMount?: boolean;
    type?: string;
    helpText?: (value?: T) => string | undefined;
    validator?: (value?: T) => ValidateResult;
    onComplete?: (value?: T) => void;
}
