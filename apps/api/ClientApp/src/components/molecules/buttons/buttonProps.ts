export interface ButtonProps {
    style?: React.CSSProperties;
    className?: string | undefined;
    disabled?: boolean | undefined;
    onClick?: ((event?: React.MouseEvent<HTMLButtonElement, MouseEvent> | undefined) => void);
}