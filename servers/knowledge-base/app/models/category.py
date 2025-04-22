from typing import TYPE_CHECKING
import uuid

from sqlalchemy import String, ForeignKey, Text, Enum
from sqlalchemy.dialects.postgresql import UUID
from sqlalchemy.orm import Mapped, mapped_column, relationship

from app.models.base import Base
from app.models.version_types import ChangeType

if TYPE_CHECKING:
    from .knowledge_item import KnowledgeItem


class Category(Base):
    __tablename__: str = "categories"

    name: Mapped[str] = mapped_column(String(150), unique=True)
    tutor_id: Mapped[uuid.UUID] = mapped_column(
        UUID(as_uuid=True),
        index=True,
        nullable=False,
    )

    knowledge_items: Mapped[list["KnowledgeItem"]] = relationship(
        "KnowledgeItem",
        back_populates="category",
        cascade="all, delete-orphan",
    )
    
    versions: Mapped[list["CategoryVersion"]] = relationship(
        "CategoryVersion",
        back_populates="category",
        cascade="all, delete-orphan",
    )


class CategoryVersion(Base):
    __tablename__: str = "category_versions"

    category_id: Mapped[uuid.UUID] = mapped_column(
        UUID(as_uuid=True),
        ForeignKey("categories.id", ondelete="SET NULL"),
        nullable=True,
        index=True,
    )
    name: Mapped[str] = mapped_column(
        Text,
        default="",
        server_default="",
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
    
    category: Mapped["Category"] = relationship(
        "Category",
        back_populates="versions",
    )
