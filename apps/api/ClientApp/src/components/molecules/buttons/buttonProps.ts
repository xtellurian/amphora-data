export interface ButtonProps {
    style?: React.CSSProperties;
    className?: string | undefined;
    disabled?: boolean | undefined;
    variant?: Variant;
    onClick?: ((event?: React.MouseEvent<HTMLButtonElement, MouseEvent> | undefined) => void);
}

const normalVariant = "normal";
const slimVariant = "slim";
export type Variant = typeof normalVariant | typeof slimVariant;