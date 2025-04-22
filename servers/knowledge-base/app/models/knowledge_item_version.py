from sqlalchemy import ForeignKey, Text, Enum, ARRAY, String
from sqlalchemy.dialects.postgresql import UUID
from sqlalchemy.orm import Mapped, mapped_column, relationship
import uuid
from datetime import datetime
from typing import List, Optional, TYPE_CHECKING

from app.models.base import Base
from app.models.version_types import ChangeType

if TYPE_CHECKING:
    from app.models.knowledge_item import KnowledgeItem

class KnowledgeItemVersion(Base):
    __tablename__: str = "knowledge_item_versions"

    knowledge_item_id: Mapped[uuid.UUID] = mapped_column(
        UUID(as_uuid=True),
        ForeignKey("knowledge_items.id", ondelete="SET NULL"),
        nullable=True,
        index=True,
    )
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
    question_tags: Mapped[List[str]] = mapped_column(
        ARRAY(String(255)),
        default=[],
        server_default="{}",
    )
    answer_tags: Mapped[List[str]] = mapped_column(
        ARRAY(String(255)),
        default=[],
        server_default="{}",
    )
    category_id: Mapped[uuid.UUID] = mapped_column(
        UUID(as_uuid=True),
        ForeignKey("categories.id", ondelete="CASCADE"),
        nullable=False,
        index=True,
    )
    tutor_id: Mapped[uuid.UUID] = mapped_column(
        UUID(as_uuid=True),
        nullable=False,
        index=True,
    )
    change_type: Mapped[str] = mapped_column(
        Enum(ChangeType),
        nullable=False,
        index=True,
    )
    deleted_with_category_version_id: Mapped[Optional[uuid.UUID]] = mapped_column(
        UUID(as_uuid=True),
        ForeignKey("category_versions.id", ondelete="SET NULL"),
        nullable=True,
        index=True,
    )
    
    knowledge_item: Mapped["KnowledgeItem"] = relationship(
        "KnowledgeItem",
        back_populates="versions",
    )
