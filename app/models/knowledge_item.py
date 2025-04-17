from sqlalchemy import ForeignKey, Text, ARRAY, String
from sqlalchemy.dialects.postgresql import UUID
from sqlalchemy.orm import Mapped, mapped_column, relationship
import uuid
from typing import List, TYPE_CHECKING

from app.models.base import Base
from app.models.category import Category

if TYPE_CHECKING:
    from app.models.knowledge_item_version import KnowledgeItemVersion


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
    tutor_id: Mapped[uuid.UUID] = mapped_column(
        UUID(as_uuid=True),
        index=True,
        nullable=False,
    )
    category_id: Mapped[uuid.UUID] = mapped_column(
        UUID(as_uuid=True),
        ForeignKey("categories.id"),
        nullable=False,
        index=True,
    )
    question_tags: Mapped[List[str]] = mapped_column(
        ARRAY(String(255)),
        default=lambda: [],
        server_default="{}",
    )
    answer_tags: Mapped[List[str]] = mapped_column(
        ARRAY(String(255)),
        default=lambda: [],
        server_default="{}",
    )

    category: Mapped["Category"] = relationship(
        "Category", back_populates="knowledge_items"
    )
    
    versions: Mapped[list["KnowledgeItemVersion"]] = relationship(
        "KnowledgeItemVersion",
        back_populates="knowledge_item",
        cascade="all, delete-orphan",
    )
