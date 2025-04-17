"""initial

Revision ID: 858ea5668483
Revises:
Create Date: 2025-04-15 21:53:36.291175

"""

from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = "858ea5668483"
down_revision: Union[str, None] = None
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    op.create_table(
        "categories",
        sa.Column("name", sa.String(length=150), nullable=False),
        sa.Column("tutor_id", sa.UUID(), nullable=False),
        sa.Column(
            "id", sa.UUID(), server_default=sa.text("gen_random_uuid()"), nullable=False
        ),
        sa.Column(
            "created_at", sa.DateTime(), server_default=sa.text("now()"), nullable=False
        ),
        sa.Column(
            "updated_at", sa.DateTime(), server_default=sa.text("now()"), nullable=False
        ),
        sa.PrimaryKeyConstraint("id"),
        sa.UniqueConstraint("name"),
    )
    op.create_index(
        op.f("ix_categories_tutor_id"), "categories", ["tutor_id"], unique=False
    )
    op.create_table(
        "knowledge_items",
        sa.Column("question", sa.Text(), server_default="", nullable=False),
        sa.Column("answer", sa.Text(), server_default="", nullable=False),
        sa.Column("tutor_id", sa.UUID(), nullable=False),
        sa.Column("category_id", sa.UUID(), nullable=False),
        sa.Column(
            "id", sa.UUID(), server_default=sa.text("gen_random_uuid()"), nullable=False
        ),
        sa.Column(
            "created_at", sa.DateTime(), server_default=sa.text("now()"), nullable=False
        ),
        sa.Column(
            "updated_at", sa.DateTime(), server_default=sa.text("now()"), nullable=False
        ),
        sa.ForeignKeyConstraint(
            ["category_id"],
            ["categories.id"],
        ),
        sa.PrimaryKeyConstraint("id"),
    )
    op.create_index(
        op.f("ix_knowledge_items_category_id"),
        "knowledge_items",
        ["category_id"],
        unique=False,
    )
    op.create_index(
        op.f("ix_knowledge_items_tutor_id"),
        "knowledge_items",
        ["tutor_id"],
        unique=False,
    )


def downgrade() -> None:
    """Downgrade schema."""
    op.drop_index(op.f("ix_knowledge_items_tutor_id"), table_name="knowledge_items")
    op.drop_index(op.f("ix_knowledge_items_category_id"), table_name="knowledge_items")
    op.drop_table("knowledge_items")
    op.drop_index(op.f("ix_categories_tutor_id"), table_name="categories")
    op.drop_table("categories")
