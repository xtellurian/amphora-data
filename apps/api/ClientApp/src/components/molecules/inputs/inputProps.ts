import * as React from 'react';

export interface ValidateResult {
    isValid: boolean;
    message?: string;
}

export interface InputProps<T> {
    label: string;
    support?: string;
    value?: T;
    onComplete?: (value: T) => void;
    validator?: (value: T) => ValidateResult;
}
