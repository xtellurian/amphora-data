import * as React from 'react';

export interface ValidateResult {
    isValid: boolean;
    message?: string;
}

export interface InputProps<T> {
    className?: string;
    label: string;
    value?: T;
    placeholder?: string;
    helpText?: (value?: T) => string | undefined;
    validator?: (value?: T) => ValidateResult;
    onComplete?: (value?: T) => void;
}
