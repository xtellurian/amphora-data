import React from "react";
import { FeedItem, PostSubjectType } from "amphoradata";
import { Container, Row, Col } from "reactstrap";
import { SecondaryButton } from "../molecules/buttons";
import { Link } from "react-router-dom";

interface FeedListProps {
    items: FeedItem[];
}

interface FeedItemRowProps {
    index: number;
    item: FeedItem;
}
const FeedItemRow: React.FC<FeedItemRowProps> = ({ item, index }) => {
    let bg = "";
    if (index % 2 === 1) {
        bg = "bg-light";
    }
    if (item.subjectType === PostSubjectType.Amphora) {
        return (
            <Row className={`pt-2 pb-2 align-items-center ${bg}`}>
                <Col className="text-center" xs={2}>
                    <img
                        style={{ maxHeight: "35px" }}
                        className="img-fluid"
                        src="/_content/sharedui/images/Amphora_Black.svg"
                    />
                </Col>
                <Col className="txt-med">{item.text}</Col>
                <Col xs={2}>
                    <Link to={`amphora/detail/${item.subjectId}`}>
                        <SecondaryButton>View</SecondaryButton>
                    </Link>
                </Col>
            </Row>
        );
    } else {
        return (
            <Row>
                <Col>{item.text}</Col>
                <Col>{item.subjectType}</Col>
            </Row>
        );
    }
};
export const FeedList: React.FC<FeedListProps> = ({ items }) => {
    return (
        <Container>
            {items.map((i, j) => (
                <FeedItemRow key={j} item={i} index={j} />
            ))}
        </Container>
    );
};
