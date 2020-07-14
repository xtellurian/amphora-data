
export interface MagicProps<V extends string | number> {
    onSave: (value: V) => void;
    initialValue: V;
    viewParent?: JSX.Element;
    disableEditing?: boolean;
}
