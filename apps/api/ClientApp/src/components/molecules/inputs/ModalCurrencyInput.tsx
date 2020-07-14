import * as React from "react";
import Modal from "react-modal";
import { Row, Col, Button, Container } from "reactstrap";

export interface CurrencyInputProps {
    initialValue: number;
    values?: number[];
    onSave: (amount: number) => void;
    onCancel?: () => void;
}

const customStyles = {
    overlay: {
        zIndex: 3000, // this should be on top of the main modal window.
    },
    content: {
        minWidth: "20rem",
        top: "50%",
        left: "50%",
        right: "auto",
        bottom: "auto",
        marginRight: "-50%",
        transform: "translate(-50%, -50%)",
    },
};

const btnStyle: React.CSSProperties = {
    width: "5rem",
    height: "5rem",
};

export const ModalCurrencyInput: React.FunctionComponent<CurrencyInputProps> = (
    props
) => {
    const [amount, setAmount] = React.useState(props.initialValue);
    const [enterManually, setEnterManually] = React.useState(false);
    const [modalIsOpen, setIsOpen] = React.useState(true);

    const currencyValues = props.values || [0, 5, 10, 20, 50, 100];
    function afterOpenModal() {
        // references are now sync'd and can be accessed.
    }

    const closeModal = () => {
        setEnterManually(false);
        setIsOpen(false);
    };

    const handleSave = (value?: number | null | undefined) => {
        props.onSave(value == null ? amount : value);
        closeModal();
    };

    const handleSelect = (value: number) => {
        setAmount(value);
        handleSave(value);
    };

    const handleCancel = () => {
        if (props.onCancel) {
            props.onCancel();
        }
        closeModal();
    };

    const currencyButton = (value: number) => (
        <Button
            style={btnStyle}
            color="primary"
            block={true}
            onClick={() => handleSelect(value)}
        >
            {value}
        </Button>
    );

    const renderRowOfValues = (row: number) => (
        <Row noGutters={true}>
            <Col>{currencyButton(currencyValues[row * 3])}</Col>
            <Col>{currencyButton(currencyValues[row * 3 + 1])}</Col>
            <Col>{currencyButton(currencyValues[row * 3 + 2])}</Col>
        </Row>
    );

    return (
        <React.Fragment>
            <Modal
                isOpen={modalIsOpen}
                onAfterOpen={afterOpenModal}
                onRequestClose={closeModal}
                style={customStyles}
                contentLabel="Example Modal"
            >
                <Container>
                    <div className="m-2">
                        {enterManually ? (
                            <div className="input-group mb-3">
                                <input
                                    className="form-control"
                                    type="number"
                                    value={amount}
                                    onChange={(e) =>
                                        setAmount(
                                            isNaN(parseFloat(e.target.value))
                                                ? 0
                                                : parseFloat(e.target.value)
                                        )
                                    }
                                />
                                <Button
                                    color="primary"
                                    block={true}
                                    onClick={() => handleSave()}
                                >
                                    Save {amount}
                                </Button>
                            </div>
                        ) : (
                            <React.Fragment>
                                {renderRowOfValues(0)}
                                {renderRowOfValues(1)}
                                <Button
                                    color="link"
                                    block={true}
                                    onClick={() => setEnterManually(true)}
                                >
                                    Manual
                                </Button>
                            </React.Fragment>
                        )}
                        <Button block={true} onClick={handleCancel}>
                            Cancel
                        </Button>
                    </div>
                </Container>
            </Modal>
        </React.Fragment>
    );
};
