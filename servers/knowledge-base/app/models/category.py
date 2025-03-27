from typing import TYPE_CHECKING

from sqlalchemy import String
from sqlalchemy.orm import Mapped, mapped_column, relationship

from app.models.base import Base

if TYPE_CHECKING:
    from .knowledge_item import KnowledgeItem


class Category(Base):
    __tablename__: str = "categories"

    name: Mapped[str] = mapped_column(String(150), unique=True)
    tutor_id: Mapped[int] = mapped_column(index=True, nullable=False)

    knowledge_items: Mapped[list["KnowledgeItem"]] = relationship(
        "KnowledgeItem",
        back_populates="category",
        cascade="all, delete-orphan",
    )
