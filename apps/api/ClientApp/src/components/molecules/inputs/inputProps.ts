import * as React from 'react';

export interface ValidateResult {
    isValid: boolean;
    message?: string;
}

export interface InputProps<T> {
    label: string;
    value?: T;
    placeholder?: string;
    helpText(value: string): string;
    onComplete?: (value: T) => void;
    validator?: (value: T) => ValidateResult;
}
