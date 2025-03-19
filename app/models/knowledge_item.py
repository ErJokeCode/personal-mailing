from sqlalchemy import ForeignKey, Text
from sqlalchemy.orm import Mapped, mapped_column, relationship

from app.models.base import Base
from app.models.category import Category


class KnowledgeItem(Base):
    __tablename__: str = "knowledge_items"

    question: Mapped[str] = mapped_column(
        Text,
        default="",
        server_default="",
    )
    answer: Mapped[str] = mapped_column(
        Text,
        default="",
        server_default="",
    )
    tutor_id: Mapped[int] = mapped_column(index=True, nullable=False)
    category_id: Mapped[int] = mapped_column(
        ForeignKey("categories.id"),
        nullable=False,
        index=True,
    )

    category: Mapped["Category"] = relationship(
        "Category", back_populates="knowledge_items"
    )
